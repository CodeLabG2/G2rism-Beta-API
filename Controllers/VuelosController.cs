using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using G2rismBeta.API.DTOs.Vuelo;
using G2rismBeta.API.Interfaces;
using FluentValidation;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de vuelos
/// Requiere autenticación. Accesible para empleados y clientes.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VuelosController : ControllerBase
{
    private readonly IVueloService _service;
    private readonly IValidator<VueloCreateDto> _createValidator;
    private readonly IValidator<VueloUpdateDto> _updateValidator;
    private readonly ILogger<VuelosController> _logger;

    public VuelosController(
        IVueloService service,
        IValidator<VueloCreateDto> createValidator,
        IValidator<VueloUpdateDto> updateValidator,
        ILogger<VuelosController> logger)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los vuelos registrados
    /// </summary>
    /// <returns>Lista de vuelos</returns>
    /// <response code="200">Lista de vuelos obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VueloResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VueloResponseDto>>> GetAll()
    {
        try
        {
            _logger.LogInformation("📋 Obteniendo todos los vuelos");
            var vuelos = await _service.GetAllAsync();
            return Ok(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener vuelos");
            return StatusCode(500, new { message = "Error al obtener los vuelos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un vuelo por su ID
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <returns>Vuelo encontrado</returns>
    /// <response code="200">Vuelo encontrado</response>
    /// <response code="404">Vuelo no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VueloResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VueloResponseDto>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Buscando vuelo con ID: {Id}", id);
            var vuelo = await _service.GetByIdAsync(id);
            if (vuelo == null)
            {
                _logger.LogWarning("⚠️ Vuelo con ID {Id} no encontrado", id);
                return NotFound(new { message = $"Vuelo con ID {id} no encontrado" });
            }

            return Ok(vuelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener vuelo con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el vuelo", error = ex.Message });
        }
    }

    /// <summary>
    /// Busca vuelos por múltiples criterios (origen, destino, fecha)
    /// </summary>
    /// <param name="origen">Ciudad o aeropuerto de origen (opcional)</param>
    /// <param name="destino">Ciudad o aeropuerto de destino (opcional)</param>
    /// <param name="fecha">Fecha de salida (opcional, formato: yyyy-MM-dd)</param>
    /// <returns>Lista de vuelos que cumplen los criterios</returns>
    /// <response code="200">Vuelos encontrados</response>
    /// <response code="500">Error interno del servidor</response>
    /// <remarks>
    /// Ejemplo de solicitud:
    ///
    ///     GET /api/vuelos/buscar?origen=Bogota&destino=Cartagena&fecha=2025-12-15
    ///
    /// Todos los parámetros son opcionales. Puedes combinarlos según tus necesidades.
    /// </remarks>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<VueloResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VueloResponseDto>>> Buscar(
        [FromQuery] string? origen,
        [FromQuery] string? destino,
        [FromQuery] DateTime? fecha)
    {
        try
        {
            _logger.LogInformation("🔍 Buscando vuelos - Origen: {Origen}, Destino: {Destino}, Fecha: {Fecha}",
                origen ?? "todos", destino ?? "todos", fecha?.ToString("yyyy-MM-dd") ?? "todas");

            var vuelos = await _service.BuscarAsync(origen, destino, fecha);
            return Ok(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al buscar vuelos");
            return StatusCode(500, new { message = "Error al buscar vuelos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los vuelos disponibles (con cupos y activos)
    /// </summary>
    /// <returns>Lista de vuelos disponibles</returns>
    /// <response code="200">Lista de vuelos disponibles obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(IEnumerable<VueloResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VueloResponseDto>>> GetDisponibles()
    {
        try
        {
            _logger.LogInformation("✈️ Obteniendo vuelos disponibles");
            var vuelos = await _service.GetDisponiblesAsync();
            return Ok(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener vuelos disponibles");
            return StatusCode(500, new { message = "Error al obtener vuelos disponibles", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo vuelo
    /// </summary>
    /// <param name="createDto">Datos del vuelo a crear</param>
    /// <returns>Vuelo creado</returns>
    /// <response code="201">Vuelo creado exitosamente</response>
    /// <response code="400">Datos inválidos o número de vuelo duplicado</response>
    /// <response code="404">Aerolínea o proveedor no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [Authorize(Roles = "Super Administrador,Administrador,Empleado")]
    [ProducesResponseType(typeof(VueloResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VueloResponseDto>> Create([FromBody] VueloCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("📝 Creando nuevo vuelo: {NumeroVuelo}", createDto.NumeroVuelo);

            var vuelo = await _service.CreateAsync(createDto);
            _logger.LogInformation("✅ Vuelo creado exitosamente con ID: {Id}", vuelo.IdVuelo);

            return CreatedAtAction(nameof(GetById), new { id = vuelo.IdVuelo }, vuelo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear vuelo");
            return StatusCode(500, new { message = "Error al crear el vuelo", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un vuelo existente
    /// </summary>
    /// <param name="id">ID del vuelo a actualizar</param>
    /// <param name="updateDto">Datos a actualizar (solo campos proporcionados)</param>
    /// <returns>Vuelo actualizado</returns>
    /// <response code="200">Vuelo actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Vuelo no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Administrador,Administrador,Empleado")]
    [ProducesResponseType(typeof(VueloResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VueloResponseDto>> Update(int id, [FromBody] VueloUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("🔄 Actualizando vuelo con ID: {Id}", id);

            var vuelo = await _service.UpdateAsync(id, updateDto);
            _logger.LogInformation("✅ Vuelo actualizado exitosamente: {Id}", id);

            return Ok(vuelo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar vuelo con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar el vuelo", error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un vuelo (soft delete - cambia estado a inactivo)
    /// </summary>
    /// <param name="id">ID del vuelo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Vuelo eliminado exitosamente</response>
    /// <response code="404">Vuelo no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Administrador,Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando vuelo con ID: {Id}", id);

            var resultado = await _service.DeleteAsync(id);
            _logger.LogInformation("✅ Vuelo eliminado exitosamente: {Id}", id);

            return Ok(new { message = "Vuelo eliminado exitosamente", id });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar vuelo con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar el vuelo", error = ex.Message });
        }
    }
}

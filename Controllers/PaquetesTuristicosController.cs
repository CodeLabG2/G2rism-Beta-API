using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using G2rismBeta.API.DTOs.PaqueteTuristico;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de paquetes turísticos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaquetesTuristicosController : ControllerBase
{
    private readonly IPaqueteTuristicoService _paqueteService;
    private readonly ILogger<PaquetesTuristicosController> _logger;

    public PaquetesTuristicosController(
        IPaqueteTuristicoService paqueteService,
        ILogger<PaquetesTuristicosController> logger)
    {
        _paqueteService = paqueteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los paquetes turísticos
    /// </summary>
    /// <returns>Lista de todos los paquetes turísticos</returns>
    /// <response code="200">Retorna la lista de paquetes turísticos</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>>> GetAll()
    {
        _logger.LogInformation("📋 Obteniendo todos los paquetes turísticos");

        var paquetes = await _paqueteService.GetAllAsync();

        return Ok(new ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {paquetes.Count()} paquetes turísticos",
            Data = paquetes
        });
    }

    /// <summary>
    /// Obtiene un paquete turístico por su ID
    /// </summary>
    /// <param name="id">ID del paquete turístico</param>
    /// <returns>Paquete turístico encontrado</returns>
    /// <response code="200">Retorna el paquete turístico</response>
    /// <response code="404">Si el paquete turístico no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaqueteTuristicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PaqueteTuristicoResponseDto>>> GetById(int id)
    {
        _logger.LogInformation("🔍 Buscando paquete turístico con ID: {Id}", id);

        try
        {
            var paquete = await _paqueteService.GetByIdAsync(id);

            return Ok(new ApiResponse<PaqueteTuristicoResponseDto>
            {
                Success = true,
                Message = "Paquete turístico encontrado",
                Data = paquete
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ Paquete turístico con ID {Id} no encontrado", id);
            return NotFound(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 404
            });
        }
    }

    /// <summary>
    /// Busca paquetes turísticos por destino principal
    /// </summary>
    /// <param name="destino">Nombre del destino</param>
    /// <returns>Lista de paquetes para el destino especificado</returns>
    /// <response code="200">Retorna la lista de paquetes</response>
    [HttpGet("destino/{destino}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>>> GetByDestino(string destino)
    {
        _logger.LogInformation("🗺️ Buscando paquetes por destino: {Destino}", destino);

        var paquetes = await _paqueteService.GetByDestinoAsync(destino);

        return Ok(new ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {paquetes.Count()} paquetes para {destino}",
            Data = paquetes
        });
    }

    /// <summary>
    /// Busca paquetes turísticos por tipo
    /// </summary>
    /// <param name="tipo">Tipo de paquete (aventura, familiar, empresarial, lujo, cultural, ecologico, romantico)</param>
    /// <returns>Lista de paquetes del tipo especificado</returns>
    /// <response code="200">Retorna la lista de paquetes</response>
    [HttpGet("tipo/{tipo}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>>> GetByTipo(string tipo)
    {
        _logger.LogInformation("🏷️ Buscando paquetes por tipo: {Tipo}", tipo);

        var paquetes = await _paqueteService.GetByTipoAsync(tipo);

        return Ok(new ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {paquetes.Count()} paquetes tipo {tipo}",
            Data = paquetes
        });
    }

    /// <summary>
    /// Obtiene todos los paquetes disponibles (activos y con cupos)
    /// </summary>
    /// <returns>Lista de paquetes disponibles</returns>
    /// <response code="200">Retorna la lista de paquetes disponibles</response>
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>>> GetDisponibles()
    {
        _logger.LogInformation("✅ Obteniendo paquetes disponibles");

        var paquetes = await _paqueteService.GetDisponiblesAsync();

        return Ok(new ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {paquetes.Count()} paquetes disponibles",
            Data = paquetes
        });
    }

    /// <summary>
    /// Busca paquetes por rango de duración
    /// </summary>
    /// <param name="min">Duración mínima en días</param>
    /// <param name="max">Duración máxima en días</param>
    /// <returns>Lista de paquetes en el rango de duración</returns>
    /// <response code="200">Retorna la lista de paquetes</response>
    /// <response code="400">Si los parámetros son inválidos</response>
    [HttpGet("duracion")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>>> GetByDuracion(
        [FromQuery] int min,
        [FromQuery] int max)
    {
        _logger.LogInformation("⏱️ Buscando paquetes por duración: {Min}-{Max} días", min, max);

        try
        {
            var paquetes = await _paqueteService.GetByDuracionAsync(min, max);

            return Ok(new ApiResponse<IEnumerable<PaqueteTuristicoResponseDto>>
            {
                Success = true,
                Message = $"Se encontraron {paquetes.Count()} paquetes de {min} a {max} días",
                Data = paquetes
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Parámetros inválidos para búsqueda por duración");
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    /// <summary>
    /// Crea un nuevo paquete turístico
    /// </summary>
    /// <param name="paqueteDto">Datos del paquete a crear</param>
    /// <returns>Paquete turístico creado</returns>
    /// <response code="201">Paquete turístico creado exitosamente</response>
    /// <response code="400">Si los datos son inválidos o ya existe un paquete con el mismo nombre</response>
    /// <response code="401">Si el usuario no está autenticado</response>
    /// <response code="403">Si el usuario no tiene el permiso paquetes.crear</response>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:paquetes.crear")]
    [ProducesResponseType(typeof(ApiResponse<PaqueteTuristicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaqueteTuristicoResponseDto>>> Create(
        [FromBody] PaqueteTuristicoCreateDto paqueteDto)
    {
        _logger.LogInformation("📝 Creando nuevo paquete turístico: {Nombre}", paqueteDto.Nombre);

        try
        {
            var paquete = await _paqueteService.CreateAsync(paqueteDto);

            _logger.LogInformation("✅ Paquete turístico creado exitosamente con ID: {Id}", paquete.IdPaquete);

            return CreatedAtAction(
                nameof(GetById),
                new { id = paquete.IdPaquete },
                new ApiResponse<PaqueteTuristicoResponseDto>
                {
                    Success = true,
                    Message = "Paquete turístico creado exitosamente",
                    Data = paquete
                });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Error al crear paquete turístico: {Message}", ex.Message);
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    /// <summary>
    /// Actualiza un paquete turístico existente
    /// </summary>
    /// <param name="id">ID del paquete a actualizar</param>
    /// <param name="paqueteDto">Datos a actualizar</param>
    /// <returns>Paquete turístico actualizado</returns>
    /// <response code="200">Paquete turístico actualizado exitosamente</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="404">Si el paquete turístico no existe</response>
    /// <response code="401">Si el usuario no está autenticado</response>
    /// <response code="403">Si el usuario no tiene el permiso paquetes.actualizar</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:paquetes.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<PaqueteTuristicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaqueteTuristicoResponseDto>>> Update(
        int id,
        [FromBody] PaqueteTuristicoUpdateDto paqueteDto)
    {
        _logger.LogInformation("📝 Actualizando paquete turístico con ID: {Id}", id);

        try
        {
            var paquete = await _paqueteService.UpdateAsync(id, paqueteDto);

            _logger.LogInformation("✅ Paquete turístico actualizado exitosamente");

            return Ok(new ApiResponse<PaqueteTuristicoResponseDto>
            {
                Success = true,
                Message = "Paquete turístico actualizado exitosamente",
                Data = paquete
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ Paquete turístico con ID {Id} no encontrado", id);
            return NotFound(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 404
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Error al actualizar paquete turístico: {Message}", ex.Message);
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400
            });
        }
    }

    /// <summary>
    /// Elimina un paquete turístico (soft delete)
    /// </summary>
    /// <param name="id">ID del paquete a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Paquete turístico eliminado exitosamente</response>
    /// <response code="404">Si el paquete turístico no existe</response>
    /// <response code="401">Si el usuario no está autenticado</response>
    /// <response code="403">Si el usuario no tiene el permiso paquetes.eliminar</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequirePermission:paquetes.eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        _logger.LogInformation("🗑️ Eliminando paquete turístico con ID: {Id}", id);

        try
        {
            await _paqueteService.DeleteAsync(id);

            _logger.LogInformation("✅ Paquete turístico eliminado exitosamente");

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Paquete turístico eliminado exitosamente",
                Data = true
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ Paquete turístico con ID {Id} no encontrado", id);
            return NotFound(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 404
            });
        }
    }
}

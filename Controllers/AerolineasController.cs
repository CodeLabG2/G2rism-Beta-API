using Microsoft.AspNetCore.Mvc;
using G2rismBeta.API.DTOs.Aerolinea;
using G2rismBeta.API.Interfaces;
using FluentValidation;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de aerolíneas
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AerolineasController : ControllerBase
{
    private readonly IAerolineaService _service;
    private readonly IValidator<AerolineaCreateDto> _createValidator;
    private readonly IValidator<AerolineaUpdateDto> _updateValidator;
    private readonly ILogger<AerolineasController> _logger;

    public AerolineasController(
        IAerolineaService service,
        IValidator<AerolineaCreateDto> createValidator,
        IValidator<AerolineaUpdateDto> updateValidator,
        ILogger<AerolineasController> logger)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las aerolíneas registradas
    /// </summary>
    /// <returns>Lista de aerolíneas</returns>
    /// <response code="200">Lista de aerolíneas obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AerolineaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AerolineaResponseDto>>> GetAll()
    {
        try
        {
            var aerolineas = await _service.GetAllAsync();
            return Ok(aerolineas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aerolíneas");
            return StatusCode(500, new { message = "Error al obtener las aerolíneas", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una aerolínea por su ID
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>Aerolínea encontrada</returns>
    /// <response code="200">Aerolínea encontrada</response>
    /// <response code="404">Aerolínea no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AerolineaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AerolineaResponseDto>> GetById(int id)
    {
        try
        {
            var aerolinea = await _service.GetByIdAsync(id);
            if (aerolinea == null)
            {
                return NotFound(new { message = $"Aerolínea con ID {id} no encontrada" });
            }

            return Ok(aerolinea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aerolínea con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al obtener la aerolínea", error = ex.Message });
        }
    }

    /// <summary>
    /// Busca una aerolínea por su código IATA
    /// </summary>
    /// <param name="code">Código IATA (2 caracteres, ej: AA, AV, LA)</param>
    /// <returns>Aerolínea encontrada</returns>
    /// <response code="200">Aerolínea encontrada</response>
    /// <response code="404">Aerolínea no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("iata/{code}")]
    [ProducesResponseType(typeof(AerolineaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AerolineaResponseDto>> GetByIata(string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
            {
                return BadRequest(new { message = "El código IATA debe tener exactamente 2 caracteres" });
            }

            var aerolinea = await _service.GetByCodigoIataAsync(code);
            if (aerolinea == null)
            {
                return NotFound(new { message = $"Aerolínea con código IATA '{code}' no encontrada" });
            }

            return Ok(aerolinea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar aerolínea con código IATA: {Code}", code);
            return StatusCode(500, new { message = "Error al buscar la aerolínea", error = ex.Message });
        }
    }

    /// <summary>
    /// Busca una aerolínea por su código ICAO
    /// </summary>
    /// <param name="code">Código ICAO (3 caracteres, ej: AAL, AVA, LAN)</param>
    /// <returns>Aerolínea encontrada</returns>
    /// <response code="200">Aerolínea encontrada</response>
    /// <response code="404">Aerolínea no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("icao/{code}")]
    [ProducesResponseType(typeof(AerolineaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AerolineaResponseDto>> GetByIcao(string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            {
                return BadRequest(new { message = "El código ICAO debe tener exactamente 3 caracteres" });
            }

            var aerolinea = await _service.GetByCodigoIcaoAsync(code);
            if (aerolinea == null)
            {
                return NotFound(new { message = $"Aerolínea con código ICAO '{code}' no encontrada" });
            }

            return Ok(aerolinea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar aerolínea con código ICAO: {Code}", code);
            return StatusCode(500, new { message = "Error al buscar la aerolínea", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva aerolínea
    /// </summary>
    /// <param name="createDto">Datos de la nueva aerolínea</param>
    /// <returns>Aerolínea creada</returns>
    /// <response code="201">Aerolínea creada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="409">Ya existe una aerolínea con ese código IATA o ICAO</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(AerolineaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AerolineaResponseDto>> Create([FromBody] AerolineaCreateDto createDto)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Errores de validación",
                    errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            var created = await _service.CreateAsync(createDto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.IdAerolinea },
                created
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear aerolínea");
            return StatusCode(500, new { message = "Error al crear la aerolínea", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una aerolínea existente
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Aerolínea actualizada</returns>
    /// <response code="200">Aerolínea actualizada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Aerolínea no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AerolineaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AerolineaResponseDto>> Update(int id, [FromBody] AerolineaUpdateDto updateDto)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Errores de validación",
                    errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            var updated = await _service.UpdateAsync(id, updateDto);
            if (updated == null)
            {
                return NotFound(new { message = $"Aerolínea con ID {id} no encontrada" });
            }

            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar aerolínea con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar la aerolínea", error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina (inactiva) una aerolínea
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Aerolínea eliminada (inactivada) exitosamente</response>
    /// <response code="404">Aerolínea no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Aerolínea con ID {id} no encontrada" });
            }

            return Ok(new { message = "Aerolínea eliminada (inactivada) exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar aerolínea con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar la aerolínea", error = ex.Message });
        }
    }
}

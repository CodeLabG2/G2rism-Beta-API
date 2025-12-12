using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using G2rismBeta.API.DTOs.FormaDePago;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Formas de Pago en el sistema de turismo.
/// Permite gestionar los métodos de pago disponibles (efectivo, tarjetas, transferencias, etc.).
/// Requiere autenticación JWT. Autorización basada en permisos granulares.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FormasDePagoController : ControllerBase
{
    private readonly IFormaDePagoService _formaDePagoService;
    private readonly ILogger<FormasDePagoController> _logger;

    public FormasDePagoController(
        IFormaDePagoService formaDePagoService,
        ILogger<FormasDePagoController> logger)
    {
        _formaDePagoService = formaDePagoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las formas de pago del sistema
    /// </summary>
    /// <returns>Lista de formas de pago</returns>
    /// <response code="200">Retorna la lista de formas de pago</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FormaDePagoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FormaDePagoResponseDto>>>> GetAll()
    {
        _logger.LogInformation("📋 Obteniendo todas las formas de pago");

        var formasDePago = await _formaDePagoService.GetAllFormasDePagoAsync();

        return Ok(new ApiResponse<IEnumerable<FormaDePagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {formasDePago.Count()} formas de pago",
            Data = formasDePago
        });
    }

    /// <summary>
    /// Obtener todas las formas de pago activas
    /// </summary>
    /// <returns>Lista de formas de pago activas</returns>
    /// <response code="200">Retorna la lista de formas de pago activas</response>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FormaDePagoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FormaDePagoResponseDto>>>> GetActivas()
    {
        _logger.LogInformation("📋 Obteniendo formas de pago activas");

        var formasDePago = await _formaDePagoService.GetFormasDePagoActivasAsync();

        return Ok(new ApiResponse<IEnumerable<FormaDePagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {formasDePago.Count()} formas de pago activas",
            Data = formasDePago
        });
    }

    /// <summary>
    /// Obtener formas de pago que requieren verificación
    /// </summary>
    /// <returns>Lista de formas de pago que requieren verificación</returns>
    /// <response code="200">Retorna la lista de formas de pago que requieren verificación</response>
    [HttpGet("requieren-verificacion")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FormaDePagoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FormaDePagoResponseDto>>>> GetQueRequierenVerificacion()
    {
        _logger.LogInformation("📋 Obteniendo formas de pago que requieren verificación");

        var formasDePago = await _formaDePagoService.GetFormasQueRequierenVerificacionAsync();

        return Ok(new ApiResponse<IEnumerable<FormaDePagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {formasDePago.Count()} formas de pago que requieren verificación",
            Data = formasDePago
        });
    }

    /// <summary>
    /// Obtener una forma de pago por su ID
    /// </summary>
    /// <param name="id">ID de la forma de pago</param>
    /// <returns>Forma de pago encontrada</returns>
    /// <response code="200">Retorna la forma de pago</response>
    /// <response code="404">Forma de pago no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FormaDePagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FormaDePagoResponseDto>>> GetById(int id)
    {
        _logger.LogInformation($"🔍 Buscando forma de pago con ID: {id}");

        var formaDePago = await _formaDePagoService.GetFormaDePagoByIdAsync(id);

        return Ok(new ApiResponse<FormaDePagoResponseDto>
        {
            Success = true,
            Message = $"Forma de pago encontrada: {formaDePago.Metodo}",
            Data = formaDePago
        });
    }

    /// <summary>
    /// Obtener una forma de pago por su método
    /// </summary>
    /// <param name="metodo">Método de pago (ej: "Efectivo", "Tarjeta de Crédito")</param>
    /// <returns>Forma de pago encontrada</returns>
    /// <response code="200">Retorna la forma de pago</response>
    /// <response code="404">Forma de pago no encontrada</response>
    [HttpGet("metodo/{metodo}")]
    [ProducesResponseType(typeof(ApiResponse<FormaDePagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FormaDePagoResponseDto>>> GetByMetodo(string metodo)
    {
        _logger.LogInformation($"🔍 Buscando forma de pago con método: {metodo}");

        var formaDePago = await _formaDePagoService.GetFormaDePagoPorMetodoAsync(metodo);

        return Ok(new ApiResponse<FormaDePagoResponseDto>
        {
            Success = true,
            Message = $"Forma de pago encontrada: {formaDePago.Metodo}",
            Data = formaDePago
        });
    }

    /// <summary>
    /// Crear una nueva forma de pago
    /// </summary>
    /// <param name="createDto">Datos de la nueva forma de pago</param>
    /// <returns>Forma de pago creada</returns>
    /// <response code="201">Forma de pago creada exitosamente</response>
    /// <response code="400">Datos inválidos o el método ya existe</response>
    /// <response code="403">Si el usuario no tiene el permiso formasdepago.crear</response>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:formasdepago.crear")]
    [ProducesResponseType(typeof(ApiResponse<FormaDePagoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<FormaDePagoResponseDto>>> Create([FromBody] FormaDePagoCreateDto createDto)
    {
        _logger.LogInformation($"📝 Creando nueva forma de pago: {createDto.Metodo}");

        var formaDePago = await _formaDePagoService.CreateFormaDePagoAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = formaDePago.IdFormaPago },
            new ApiResponse<FormaDePagoResponseDto>
            {
                Success = true,
                Message = $"Forma de pago '{formaDePago.Metodo}' creada exitosamente",
                Data = formaDePago
            });
    }

    /// <summary>
    /// Actualizar una forma de pago existente
    /// </summary>
    /// <param name="id">ID de la forma de pago a actualizar</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Forma de pago actualizada</returns>
    /// <response code="200">Forma de pago actualizada exitosamente</response>
    /// <response code="404">Forma de pago no encontrada</response>
    /// <response code="400">Datos inválidos o el nuevo método ya existe</response>
    /// <response code="403">Si el usuario no tiene el permiso formasdepago.actualizar</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:formasdepago.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<FormaDePagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<FormaDePagoResponseDto>>> Update(int id, [FromBody] FormaDePagoUpdateDto updateDto)
    {
        _logger.LogInformation($"📝 Actualizando forma de pago con ID: {id}");

        var formaDePago = await _formaDePagoService.UpdateFormaDePagoAsync(id, updateDto);

        return Ok(new ApiResponse<FormaDePagoResponseDto>
        {
            Success = true,
            Message = $"Forma de pago con ID {id} actualizada exitosamente",
            Data = formaDePago
        });
    }

    /// <summary>
    /// Eliminar una forma de pago
    /// </summary>
    /// <param name="id">ID de la forma de pago a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Forma de pago eliminada exitosamente</response>
    /// <response code="404">Forma de pago no encontrada</response>
    /// <response code="400">No se puede eliminar porque tiene pagos asociados</response>
    /// <response code="403">Si el usuario no tiene el permiso formasdepago.eliminar</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequirePermission:formasdepago.eliminar")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        _logger.LogInformation($"🗑️ Eliminando forma de pago con ID: {id}");

        await _formaDePagoService.DeleteFormaDePagoAsync(id);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = $"Forma de pago con ID {id} eliminada exitosamente",
            Data = null
        });
    }
}

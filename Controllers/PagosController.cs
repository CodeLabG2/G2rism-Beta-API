using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using G2rismBeta.API.DTOs.Pago;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Pagos en el sistema de turismo.
/// Permite registrar, consultar y gestionar los pagos realizados sobre las facturas.
/// FUNCIONALIDAD CLAVE: Actualiza automáticamente los montos de facturas y reservas.
/// Requiere autenticación JWT. Autorización basada en permisos granulares.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;
    private readonly ILogger<PagosController> _logger;

    public PagosController(
        IPagoService pagoService,
        ILogger<PagosController> logger)
    {
        _pagoService = pagoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los pagos del sistema
    /// </summary>
    /// <returns>Lista de todos los pagos con sus detalles</returns>
    /// <response code="200">Retorna la lista de pagos</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PagoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PagoResponseDto>>>> GetAll()
    {
        _logger.LogInformation("📋 Obteniendo todos los pagos");

        var pagos = await _pagoService.GetAllPagosAsync();

        return Ok(new ApiResponse<IEnumerable<PagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {pagos.Count()} pagos",
            Data = pagos
        });
    }

    /// <summary>
    /// Obtener un pago por su ID
    /// </summary>
    /// <param name="id">ID del pago</param>
    /// <returns>Pago encontrado con toda su información</returns>
    /// <response code="200">Retorna el pago</response>
    /// <response code="404">Pago no encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagoResponseDto>>> GetById(int id)
    {
        _logger.LogInformation($"🔍 Buscando pago con ID: {id}");

        var pago = await _pagoService.GetPagoByIdAsync(id);

        return Ok(new ApiResponse<PagoResponseDto>
        {
            Success = true,
            Message = $"Pago encontrado (${pago.Monto:N2} - {pago.Estado})",
            Data = pago
        });
    }

    /// <summary>
    /// Obtener todos los pagos de una factura específica
    /// </summary>
    /// <param name="idFactura">ID de la factura</param>
    /// <returns>Lista de pagos realizados a la factura</returns>
    /// <response code="200">Retorna la lista de pagos de la factura</response>
    /// <response code="404">Factura no encontrada</response>
    [HttpGet("factura/{idFactura}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PagoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PagoResponseDto>>>> GetByFactura(int idFactura)
    {
        _logger.LogInformation($"🔍 Obteniendo pagos de la factura ID: {idFactura}");

        var pagos = await _pagoService.GetPagosPorFacturaAsync(idFactura);

        var totalPagado = pagos.Where(p => p.EstaAprobado).Sum(p => p.Monto);

        return Ok(new ApiResponse<IEnumerable<PagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {pagos.Count()} pagos (Total aprobado: ${totalPagado:N2})",
            Data = pagos
        });
    }

    /// <summary>
    /// Obtener pagos por estado
    /// </summary>
    /// <param name="estado">Estado del pago (pendiente, aprobado, rechazado)</param>
    /// <returns>Lista de pagos con ese estado</returns>
    /// <response code="200">Retorna la lista de pagos con el estado especificado</response>
    /// <response code="400">Estado inválido</response>
    [HttpGet("estado/{estado}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PagoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PagoResponseDto>>>> GetByEstado(string estado)
    {
        _logger.LogInformation($"🔍 Obteniendo pagos con estado: {estado}");

        var pagos = await _pagoService.GetPagosPorEstadoAsync(estado);

        return Ok(new ApiResponse<IEnumerable<PagoResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {pagos.Count()} pagos con estado '{estado}'",
            Data = pagos
        });
    }

    /// <summary>
    /// Registrar un nuevo pago
    /// IMPORTANTE: Si el pago está aprobado, actualiza automáticamente:
    /// - MontoPagado y SaldoPendiente de la Reserva
    /// - Estado de la Factura (si se paga completamente)
    /// </summary>
    /// <param name="createDto">Datos del nuevo pago</param>
    /// <returns>Pago registrado</returns>
    /// <response code="201">Pago registrado exitosamente</response>
    /// <response code="400">Datos inválidos o monto excede saldo pendiente</response>
    /// <response code="404">Factura o forma de pago no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso pagos.crear</response>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pagos.crear")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagoResponseDto>>> Create([FromBody] PagoCreateDto createDto)
    {
        _logger.LogInformation($"💰 Registrando nuevo pago de ${createDto.Monto:N2} para factura ID: {createDto.IdFactura}");

        var pago = await _pagoService.CreatePagoAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = pago.IdPago },
            new ApiResponse<PagoResponseDto>
            {
                Success = true,
                Message = $"Pago de ${pago.Monto:N2} registrado exitosamente con estado '{pago.Estado}'",
                Data = pago
            });
    }

    /// <summary>
    /// Actualizar un pago existente
    /// IMPORTANTE: Si se cambia el monto o estado, se recalculan automáticamente
    /// los montos de la factura y reserva asociadas
    /// </summary>
    /// <param name="id">ID del pago a actualizar</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Pago actualizado</returns>
    /// <response code="200">Pago actualizado exitosamente</response>
    /// <response code="404">Pago no encontrado</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="403">Si el usuario no tiene el permiso pagos.actualizar</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:pagos.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagoResponseDto>>> Update(int id, [FromBody] PagoUpdateDto updateDto)
    {
        _logger.LogInformation($"📝 Actualizando pago con ID: {id}");

        var pago = await _pagoService.UpdatePagoAsync(id, updateDto);

        return Ok(new ApiResponse<PagoResponseDto>
        {
            Success = true,
            Message = $"Pago ID {id} actualizado exitosamente",
            Data = pago
        });
    }

    /// <summary>
    /// Cambiar el estado de un pago
    /// IMPORTANTE: Recalcula automáticamente los montos de factura y reserva
    /// </summary>
    /// <param name="id">ID del pago</param>
    /// <param name="nuevoEstado">Nuevo estado (pendiente, aprobado, rechazado)</param>
    /// <returns>Pago con estado actualizado</returns>
    /// <response code="200">Estado actualizado exitosamente</response>
    /// <response code="404">Pago no encontrado</response>
    /// <response code="400">Estado inválido</response>
    /// <response code="403">Si el usuario no tiene el permiso pagos.actualizar</response>
    [HttpPatch("{id}/estado")]
    [Authorize(Policy = "RequirePermission:pagos.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagoResponseDto>>> CambiarEstado(int id, [FromBody] string nuevoEstado)
    {
        _logger.LogInformation($"🔄 Cambiando estado del pago ID {id} a: {nuevoEstado}");

        var pago = await _pagoService.CambiarEstadoPagoAsync(id, nuevoEstado);

        return Ok(new ApiResponse<PagoResponseDto>
        {
            Success = true,
            Message = $"Estado del pago ID {id} cambiado a '{pago.Estado}' exitosamente",
            Data = pago
        });
    }

    /// <summary>
    /// Eliminar un pago
    /// RESTRICCIÓN: Solo se pueden eliminar pagos con estado 'pendiente'
    /// </summary>
    /// <param name="id">ID del pago a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Pago eliminado exitosamente</response>
    /// <response code="404">Pago no encontrado</response>
    /// <response code="400">No se puede eliminar (el pago no está pendiente)</response>
    /// <response code="403">Si el usuario no tiene el permiso pagos.eliminar</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequirePermission:pagos.eliminar")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        _logger.LogInformation($"🗑️ Eliminando pago ID: {id}");

        await _pagoService.DeletePagoAsync(id);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = $"Pago ID {id} eliminado exitosamente",
            Data = null
        });
    }
}
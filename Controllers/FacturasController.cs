using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using G2rismBeta.API.DTOs.Factura;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Facturas en el sistema de turismo.
/// Permite generar facturas desde reservas, consultar, actualizar y gestionar el estado de las facturas.
/// Requiere autenticación JWT. Autorización basada en permisos granulares.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IFacturaService _facturaService;
    private readonly ILogger<FacturasController> _logger;

    public FacturasController(
        IFacturaService facturaService,
        ILogger<FacturasController> _logger)
    {
        _facturaService = facturaService;
        this._logger = _logger;
    }

    /// <summary>
    /// Obtener todas las facturas del sistema
    /// </summary>
    /// <returns>Lista de facturas</returns>
    /// <response code="200">Retorna la lista de facturas</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FacturaResponseDto>>>> GetAll()
    {
        _logger.LogInformation("📋 Obteniendo todas las facturas");

        var facturas = await _facturaService.GetAllFacturasAsync();

        return Ok(new ApiResponse<IEnumerable<FacturaResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {facturas.Count()} facturas",
            Data = facturas
        });
    }

    /// <summary>
    /// Obtener una factura por su ID
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Factura encontrada</returns>
    /// <response code="200">Retorna la factura</response>
    /// <response code="404">Factura no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FacturaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacturaResponseDto>>> GetById(int id)
    {
        _logger.LogInformation($"🔍 Buscando factura con ID: {id}");

        var factura = await _facturaService.GetFacturaByIdAsync(id);

        return Ok(new ApiResponse<FacturaResponseDto>
        {
            Success = true,
            Message = $"Factura encontrada: {factura.NumeroFactura}",
            Data = factura
        });
    }

    /// <summary>
    /// Obtener facturas de una reserva específica
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Facturas de la reserva</returns>
    /// <response code="200">Retorna las facturas de la reserva</response>
    /// <response code="404">Reserva no encontrada</response>
    [HttpGet("reserva/{idReserva}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FacturaResponseDto>>>> GetByReserva(int idReserva)
    {
        _logger.LogInformation($"🔍 Buscando facturas para la reserva ID: {idReserva}");

        var facturas = await _facturaService.GetFacturasPorReservaAsync(idReserva);

        return Ok(new ApiResponse<IEnumerable<FacturaResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {facturas.Count()} facturas para la reserva {idReserva}",
            Data = facturas
        });
    }

    /// <summary>
    /// Obtener una factura por su número
    /// </summary>
    /// <param name="numeroFactura">Número de la factura (ejemplo: FAC-2025-00001)</param>
    /// <returns>Factura encontrada</returns>
    /// <response code="200">Retorna la factura</response>
    /// <response code="404">Factura no encontrada</response>
    [HttpGet("numero/{numeroFactura}")]
    [ProducesResponseType(typeof(ApiResponse<FacturaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacturaResponseDto>>> GetByNumero(string numeroFactura)
    {
        _logger.LogInformation($"🔍 Buscando factura con número: {numeroFactura}");

        var factura = await _facturaService.GetFacturaPorNumeroAsync(numeroFactura);

        return Ok(new ApiResponse<FacturaResponseDto>
        {
            Success = true,
            Message = $"Factura encontrada: {factura.NumeroFactura}",
            Data = factura
        });
    }

    /// <summary>
    /// Obtener facturas por estado
    /// </summary>
    /// <param name="estado">Estado de la factura (pendiente, pagada, cancelada, vencida)</param>
    /// <returns>Facturas con el estado especificado</returns>
    /// <response code="200">Retorna las facturas con ese estado</response>
    /// <response code="400">Estado inválido</response>
    [HttpGet("estado/{estado}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FacturaResponseDto>>>> GetByEstado(string estado)
    {
        _logger.LogInformation($"🔍 Buscando facturas con estado: {estado}");

        var facturas = await _facturaService.GetFacturasPorEstadoAsync(estado);

        return Ok(new ApiResponse<IEnumerable<FacturaResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {facturas.Count()} facturas con estado '{estado}'",
            Data = facturas
        });
    }

    /// <summary>
    /// Obtener facturas vencidas
    /// </summary>
    /// <returns>Facturas vencidas</returns>
    /// <response code="200">Retorna las facturas vencidas</response>
    [HttpGet("vencidas")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FacturaResponseDto>>>> GetVencidas()
    {
        _logger.LogInformation("🔍 Buscando facturas vencidas");

        var facturas = await _facturaService.GetFacturasVencidasAsync();

        return Ok(new ApiResponse<IEnumerable<FacturaResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {facturas.Count()} facturas vencidas",
            Data = facturas
        });
    }

    /// <summary>
    /// Obtener facturas próximas a vencer
    /// </summary>
    /// <param name="dias">Número de días para considerar próximo a vencer (por defecto 7)</param>
    /// <returns>Facturas próximas a vencer</returns>
    /// <response code="200">Retorna las facturas próximas a vencer</response>
    /// <response code="400">Número de días inválido</response>
    [HttpGet("proximas-vencer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FacturaResponseDto>>>> GetProximasAVencer([FromQuery] int dias = 7)
    {
        _logger.LogInformation($"🔍 Buscando facturas próximas a vencer (próximos {dias} días)");

        var facturas = await _facturaService.GetFacturasProximasAVencerAsync(dias);

        return Ok(new ApiResponse<IEnumerable<FacturaResponseDto>>
        {
            Success = true,
            Message = $"Se encontraron {facturas.Count()} facturas próximas a vencer",
            Data = facturas
        });
    }

    /// <summary>
    /// Crear una nueva factura desde una reserva
    /// </summary>
    /// <param name="dto">Datos para crear la factura</param>
    /// <returns>Factura creada</returns>
    /// <response code="201">Factura creada exitosamente</response>
    /// <response code="400">Datos inválidos o reserva sin confirmar</response>
    /// <response code="404">Reserva no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso facturas.crear</response>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:facturas.crear")]
    [ProducesResponseType(typeof(ApiResponse<FacturaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacturaResponseDto>>> Create([FromBody] FacturaCreateDto dto)
    {
        _logger.LogInformation($"📝 Creando factura para la reserva ID: {dto.IdReserva}");

        var factura = await _facturaService.CrearFacturaAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = factura.IdFactura },
            new ApiResponse<FacturaResponseDto>
            {
                Success = true,
                Message = $"Factura {factura.NumeroFactura} creada exitosamente. Total: ${factura.Total:N2}",
                Data = factura
            });
    }

    /// <summary>
    /// Actualizar una factura existente
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <param name="dto">Datos actualizados</param>
    /// <returns>Factura actualizada</returns>
    /// <response code="200">Factura actualizada exitosamente</response>
    /// <response code="400">Datos inválidos o factura ya pagada/cancelada</response>
    /// <response code="404">Factura no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso facturas.actualizar</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:facturas.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<FacturaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacturaResponseDto>>> Update(int id, [FromBody] FacturaUpdateDto dto)
    {
        _logger.LogInformation($"📝 Actualizando factura ID: {id}");

        var factura = await _facturaService.ActualizarFacturaAsync(id, dto);

        return Ok(new ApiResponse<FacturaResponseDto>
        {
            Success = true,
            Message = $"Factura {factura.NumeroFactura} actualizada exitosamente",
            Data = factura
        });
    }

    /// <summary>
    /// Cambiar el estado de una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <param name="request">Nuevo estado</param>
    /// <returns>Factura actualizada</returns>
    /// <response code="200">Estado actualizado exitosamente</response>
    /// <response code="400">Estado inválido</response>
    /// <response code="404">Factura no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso facturas.actualizar</response>
    [HttpPatch("{id}/estado")]
    [Authorize(Policy = "RequirePermission:facturas.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<FacturaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacturaResponseDto>>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoFacturaRequest request)
    {
        _logger.LogInformation($"📝 Cambiando estado de factura ID {id} a: {request.NuevoEstado}");

        var factura = await _facturaService.CambiarEstadoFacturaAsync(id, request.NuevoEstado);

        return Ok(new ApiResponse<FacturaResponseDto>
        {
            Success = true,
            Message = $"Estado de factura {factura.NumeroFactura} cambiado a '{request.NuevoEstado}'",
            Data = factura
        });
    }

    /// <summary>
    /// Eliminar (cancelar) una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Factura cancelada exitosamente</response>
    /// <response code="400">Factura ya pagada o tiene pagos aprobados</response>
    /// <response code="404">Factura no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso facturas.eliminar</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequirePermission:facturas.eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        _logger.LogInformation($"🗑️ Eliminando (cancelando) factura ID: {id}");

        var resultado = await _facturaService.EliminarFacturaAsync(id);

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Factura cancelada exitosamente",
            Data = resultado
        });
    }
}

/// <summary>
/// DTO para cambiar el estado de una factura
/// </summary>
public class CambiarEstadoFacturaRequest
{
    /// <summary>
    /// Nuevo estado de la factura (pendiente, pagada, cancelada, vencida)
    /// </summary>
    /// <example>pagada</example>
    public string NuevoEstado { get; set; } = string.Empty;
}
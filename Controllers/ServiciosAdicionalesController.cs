using AutoMapper;
using G2rismBeta.API.DTOs.ServicioAdicional;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de servicios adicionales (tours, guías, actividades, transporte interno)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ServiciosAdicionalesController : ControllerBase
{
    private readonly IServicioAdicionalService _servicioService;
    private readonly IMapper _mapper;
    private readonly ILogger<ServiciosAdicionalesController> _logger;

    public ServiciosAdicionalesController(
        IServicioAdicionalService servicioService,
        IMapper mapper,
        ILogger<ServiciosAdicionalesController> logger)
    {
        _servicioService = servicioService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los servicios adicionales registrados
    /// </summary>
    /// <returns>Lista de servicios con información del proveedor</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>>> GetAll()
    {
        _logger.LogInformation("🎯 GET /api/servicios-adicionales - Obteniendo todos los servicios");

        var servicios = await _servicioService.GetAllAsync();

        return Ok(new ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>
        {
            Success = true,
            Message = "Servicios adicionales obtenidos exitosamente",
            Data = servicios,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Obtiene un servicio adicional por su ID
    /// </summary>
    /// <param name="id">ID del servicio</param>
    /// <returns>Información completa del servicio</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ServicioAdicionalResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicioAdicionalResponseDto>>> GetById(int id)
    {
        _logger.LogInformation("🔍 GET /api/servicios-adicionales/{Id} - Buscando servicio", id);

        var servicio = await _servicioService.GetByIdAsync(id);

        return Ok(new ApiResponse<ServicioAdicionalResponseDto>
        {
            Success = true,
            Message = "Servicio adicional encontrado exitosamente",
            Data = servicio,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Busca servicios adicionales por tipo
    /// </summary>
    /// <param name="tipo">Tipo de servicio (tour, guia, actividad, transporte_interno)</param>
    /// <returns>Lista de servicios del tipo especificado</returns>
    [HttpGet("tipo/{tipo}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>>> GetByTipo(string tipo)
    {
        _logger.LogInformation("🔍 GET /api/servicios-adicionales/tipo/{Tipo} - Buscando servicios", tipo);

        var servicios = await _servicioService.GetByTipoAsync(tipo);

        return Ok(new ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>
        {
            Success = true,
            Message = $"Servicios de tipo '{tipo}' obtenidos exitosamente",
            Data = servicios,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Obtiene todos los servicios disponibles (activos y con disponibilidad)
    /// </summary>
    /// <returns>Lista de servicios disponibles</returns>
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>>> GetDisponibles()
    {
        _logger.LogInformation("✅ GET /api/servicios-adicionales/disponibles - Obteniendo servicios disponibles");

        var servicios = await _servicioService.GetDisponiblesAsync();

        return Ok(new ApiResponse<IEnumerable<ServicioAdicionalResponseDto>>
        {
            Success = true,
            Message = "Servicios disponibles obtenidos exitosamente",
            Data = servicios,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Crea un nuevo servicio adicional
    /// </summary>
    /// <param name="servicioDto">Datos del servicio a crear</param>
    /// <returns>Servicio creado con su ID asignado</returns>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/servicios-adicionales
    ///     {
    ///         "idProveedor": 1,
    ///         "nombre": "City Tour Cartagena",
    ///         "tipo": "tour",
    ///         "descripcion": "Recorrido por el centro histórico de Cartagena",
    ///         "precio": 85000,
    ///         "unidad": "persona",
    ///         "disponibilidad": true,
    ///         "tiempoEstimado": "3:00",
    ///         "ubicacion": "Plaza de Bolívar",
    ///         "capacidadMaxima": 15,
    ///         "edadMinima": 5,
    ///         "idiomasDisponibles": "[\"Español\", \"Inglés\"]",
    ///         "estado": true
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:servicios.crear")]
    [ProducesResponseType(typeof(ApiResponse<ServicioAdicionalResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ServicioAdicionalResponseDto>>> Create([FromBody] ServicioAdicionalCreateDto servicioDto)
    {
        _logger.LogInformation("📝 POST /api/servicios-adicionales - Creando nuevo servicio: {Nombre}", servicioDto.Nombre);

        var servicioCreado = await _servicioService.CreateAsync(servicioDto);

        var response = new ApiResponse<ServicioAdicionalResponseDto>
        {
            Success = true,
            Message = "Servicio adicional creado exitosamente",
            Data = servicioCreado,
            Timestamp = DateTime.UtcNow
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = servicioCreado.IdServicio },
            response);
    }

    /// <summary>
    /// Actualiza un servicio adicional existente
    /// </summary>
    /// <param name="id">ID del servicio a actualizar</param>
    /// <param name="servicioDto">Datos a actualizar (todos los campos son opcionales)</param>
    /// <returns>Servicio actualizado</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:servicios.actualizar")]
    [ProducesResponseType(typeof(ApiResponse<ServicioAdicionalResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicioAdicionalResponseDto>>> Update(int id, [FromBody] ServicioAdicionalUpdateDto servicioDto)
    {
        _logger.LogInformation("🔄 PUT /api/servicios-adicionales/{Id} - Actualizando servicio", id);

        var servicioActualizado = await _servicioService.UpdateAsync(id, servicioDto);

        return Ok(new ApiResponse<ServicioAdicionalResponseDto>
        {
            Success = true,
            Message = "Servicio adicional actualizado exitosamente",
            Data = servicioActualizado,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Elimina un servicio adicional (soft delete - cambia estado a inactivo)
    /// </summary>
    /// <param name="id">ID del servicio a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequirePermission:servicios.eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        _logger.LogInformation("🗑️ DELETE /api/servicios-adicionales/{Id} - Eliminando servicio", id);

        var resultado = await _servicioService.DeleteAsync(id);

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Servicio adicional eliminado exitosamente",
            Data = resultado,
            Timestamp = DateTime.UtcNow
        });
    }
}

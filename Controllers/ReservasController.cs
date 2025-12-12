using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using G2rismBeta.API.DTOs.Reserva;
using G2rismBeta.API.DTOs.ReservaHotel;
using G2rismBeta.API.DTOs.ReservaVuelo;
using G2rismBeta.API.DTOs.ReservaPaquete;
using G2rismBeta.API.DTOs.ReservaServicio;
using G2rismBeta.API.Interfaces;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Reservas
/// Endpoints para operaciones CRUD básicas de reservas
/// Requiere autenticación JWT. Autorización basada en permisos granulares.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;
    private readonly IReservaHotelService _reservaHotelService;
    private readonly IReservaVueloService _reservaVueloService;
    private readonly IReservaPaqueteService _reservaPaqueteService;
    private readonly IReservaServicioService _reservaServicioService;
    private readonly ILogger<ReservasController> _logger;

    /// <summary>
    /// Constructor: Recibe los servicios de reservas y logger por inyección de dependencias
    /// </summary>
    public ReservasController(
        IReservaService reservaService,
        IReservaHotelService reservaHotelService,
        IReservaVueloService reservaVueloService,
        IReservaPaqueteService reservaPaqueteService,
        IReservaServicioService reservaServicioService,
        ILogger<ReservasController> logger)
    {
        _reservaService = reservaService;
        _reservaHotelService = reservaHotelService;
        _reservaVueloService = reservaVueloService;
        _reservaPaqueteService = reservaPaqueteService;
        _reservaServicioService = reservaServicioService;
        _logger = logger;
    }

    // ========================================
    // ENDPOINTS DE CONSULTA (GET)
    // ========================================

    /// <summary>
    /// Obtener todas las reservas del sistema
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas
    ///
    /// </remarks>
    /// <response code="200">Lista de reservas obtenida exitosamente</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReservaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservaResponseDto>>> GetAllReservas()
    {
        try
        {
            _logger.LogInformation("📋 Obteniendo todas las reservas");
            var reservas = await _reservaService.GetAllReservasAsync();
            _logger.LogInformation($"✅ Se obtuvieron {reservas.Count()} reservas");
            return Ok(reservas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener las reservas");
            return StatusCode(500, new { message = "Error al obtener las reservas", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener una reserva específica por su ID
    /// </summary>
    /// <param name="id">ID de la reserva a buscar</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/1
    ///
    /// </remarks>
    /// <response code="200">Reserva encontrada</response>
    /// <response code="404">Reserva no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> GetReservaById(int id)
    {
        try
        {
            _logger.LogInformation($"🔍 Buscando reserva con ID: {id}");
            var reserva = await _reservaService.GetReservaByIdAsync(id);

            if (reserva == null)
            {
                _logger.LogWarning($"⚠️ No se encontró la reserva con ID {id}");
                return NotFound(new { message = $"No se encontró la reserva con ID {id}" });
            }

            _logger.LogInformation($"✅ Reserva encontrada: {reserva.IdReserva}");
            return Ok(reserva);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Argumento inválido");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener la reserva");
            return StatusCode(500, new { message = "Error al obtener la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener todas las reservas de un cliente específico
    /// </summary>
    /// <param name="idCliente">ID del cliente</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/cliente/5
    ///
    /// </remarks>
    /// <response code="200">Lista de reservas del cliente obtenida exitosamente</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("cliente/{idCliente}")]
    [ProducesResponseType(typeof(IEnumerable<ReservaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservaResponseDto>>> GetReservasByCliente(int idCliente)
    {
        try
        {
            _logger.LogInformation($"🔍 Obteniendo reservas del cliente ID: {idCliente}");
            var reservas = await _reservaService.GetReservasByClienteAsync(idCliente);
            _logger.LogInformation($"✅ Se obtuvieron {reservas.Count()} reservas del cliente {idCliente}");
            return Ok(reservas);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"⚠️ Cliente no encontrado: {idCliente}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener las reservas del cliente");
            return StatusCode(500, new { message = "Error al obtener las reservas del cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener reservas filtradas por estado
    /// </summary>
    /// <param name="estado">Estado de la reserva (pendiente, confirmada, cancelada, completada)</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/estado/pendiente
    ///
    /// </remarks>
    /// <response code="200">Lista de reservas con el estado especificado</response>
    /// <response code="400">Estado inválido</response>
    [HttpGet("estado/{estado}")]
    [ProducesResponseType(typeof(IEnumerable<ReservaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ReservaResponseDto>>> GetReservasByEstado(string estado)
    {
        try
        {
            _logger.LogInformation($"🔍 Obteniendo reservas con estado: {estado}");
            var reservas = await _reservaService.GetReservasByEstadoAsync(estado);
            _logger.LogInformation($"✅ Se obtuvieron {reservas.Count()} reservas con estado '{estado}'");
            return Ok(reservas);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, $"⚠️ Estado inválido: {estado}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener las reservas por estado");
            return StatusCode(500, new { message = "Error al obtener las reservas por estado", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE MODIFICACIÓN (POST, PUT, DELETE)
    // ========================================

    /// <summary>
    /// Crear una nueva reserva básica (sin servicios)
    /// </summary>
    /// <param name="reservaCreateDto">Datos de la reserva a crear</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas
    ///     {
    ///         "idCliente": 1,
    ///         "idEmpleado": 2,
    ///         "descripcion": "Viaje familiar a Cartagena",
    ///         "fechaInicioViaje": "2025-12-20",
    ///         "fechaFinViaje": "2025-12-27",
    ///         "numeroPasajeros": 4,
    ///         "estado": "pendiente",
    ///         "observaciones": "Requieren habitación con vista al mar"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Reserva creada exitosamente</response>
    /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
    /// <response code="404">Cliente o empleado no encontrado</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> CreateReserva([FromBody] ReservaCreateDto reservaCreateDto)
    {
        try
        {
            _logger.LogInformation("📝 Creando nueva reserva");
            var reservaCreada = await _reservaService.CreateReservaAsync(reservaCreateDto);
            _logger.LogInformation($"✅ Reserva creada exitosamente con ID: {reservaCreada.IdReserva}");

            return CreatedAtAction(
                nameof(GetReservaById),
                new { id = reservaCreada.IdReserva },
                reservaCreada
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Entidad relacionada no encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Argumento inválido");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear la reserva");
            return StatusCode(500, new { message = "Error al crear la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar una reserva existente
    /// </summary>
    /// <param name="id">ID de la reserva a actualizar</param>
    /// <param name="reservaUpdateDto">Datos a actualizar (solo campos proporcionados)</param>
    /// <remarks>
    /// Ejemplo de request (actualización parcial):
    ///
    ///     PUT /api/reservas/1
    ///     {
    ///         "estado": "confirmada",
    ///         "observaciones": "Cliente confirmó el pago inicial"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Reserva actualizada exitosamente</response>
    /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
    /// <response code="404">Reserva no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.actualizar</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequirePermission:reservas.actualizar")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> UpdateReserva(int id, [FromBody] ReservaUpdateDto reservaUpdateDto)
    {
        try
        {
            _logger.LogInformation($"📝 Actualizando reserva con ID: {id}");
            var reservaActualizada = await _reservaService.UpdateReservaAsync(id, reservaUpdateDto);
            _logger.LogInformation($"✅ Reserva {id} actualizada exitosamente");
            return Ok(reservaActualizada);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"⚠️ Reserva no encontrada: {id}");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Argumento inválido");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar la reserva");
            return StatusCode(500, new { message = "Error al actualizar la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS ADICIONALES DE OPERACIONES DE NEGOCIO
    // ========================================

    /// <summary>
    /// Confirmar una reserva (cambiar de pendiente a confirmada)
    /// </summary>
    /// <param name="id">ID de la reserva a confirmar</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas/1/confirmar
    ///
    /// </remarks>
    /// <response code="200">Reserva confirmada exitosamente</response>
    /// <response code="400">La reserva no puede ser confirmada (estado inválido)</response>
    /// <response code="404">Reserva no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.actualizar</response>
    [HttpPost("{id}/confirmar")]
    [Authorize(Policy = "RequirePermission:reservas.actualizar")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> ConfirmarReserva(int id)
    {
        try
        {
            _logger.LogInformation($"✅ Confirmando reserva ID: {id}");
            var reservaConfirmada = await _reservaService.ConfirmarReservaAsync(id);
            _logger.LogInformation($"✅ Reserva {id} confirmada exitosamente");
            return Ok(reservaConfirmada);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"⚠️ Reserva no encontrada: {id}");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ No se puede confirmar la reserva");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al confirmar la reserva");
            return StatusCode(500, new { message = "Error al confirmar la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar una reserva
    /// </summary>
    /// <param name="id">ID de la reserva a cancelar</param>
    /// <param name="motivoCancelacion">Motivo de la cancelación</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas/1/cancelar
    ///     {
    ///         "motivoCancelacion": "Cliente solicitó cambio de fechas"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Reserva cancelada exitosamente</response>
    /// <response code="400">La reserva no puede ser cancelada (ya está cancelada o completada)</response>
    /// <response code="404">Reserva no encontrada</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.eliminar</response>
    [HttpPost("{id}/cancelar")]
    [Authorize(Policy = "RequirePermission:reservas.eliminar")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> CancelarReserva(int id, [FromBody] CancelarReservaDto cancelarDto)
    {
        try
        {
            _logger.LogInformation($"❌ Cancelando reserva ID: {id}");
            var reservaCancelada = await _reservaService.CancelarReservaAsync(id, cancelarDto.MotivoCancelacion);
            _logger.LogInformation($"✅ Reserva {id} cancelada exitosamente");
            return Ok(reservaCancelada);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"⚠️ Reserva no encontrada: {id}");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ No se puede cancelar la reserva");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al cancelar la reserva");
            return StatusCode(500, new { message = "Error al cancelar la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE GESTIÓN DE HOTELES EN RESERVAS
    // ========================================

    /// <summary>
    /// Agregar un hotel a una reserva existente
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="dto">Datos del hotel a agregar</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas/1/hoteles
    ///     {
    ///         "idHotel": 5,
    ///         "fechaCheckin": "2025-12-20",
    ///         "fechaCheckout": "2025-12-23",
    ///         "numeroHabitaciones": 2,
    ///         "tipoHabitacion": "doble",
    ///         "numeroHuespedes": 4,
    ///         "observaciones": "Habitaciones contiguas preferiblemente"
    ///     }
    ///
    /// El sistema calcula automáticamente:
    /// - Número de noches
    /// - Precio por noche (del hotel actual)
    /// - Subtotal (noches * precio * habitaciones)
    /// - Actualiza el monto total de la reserva
    /// </remarks>
    /// <response code="201">Hotel agregado exitosamente a la reserva</response>
    /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
    /// <response code="404">Reserva o Hotel no encontrado</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost("{id}/hoteles")]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaHotelResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaHotelResponseDto>> AgregarHotelAReserva(int id, [FromBody] ReservaHotelCreateDto dto)
    {
        try
        {
            _logger.LogInformation("🏨 Agregando hotel {IdHotel} a la reserva {IdReserva}", dto.IdHotel, id);
            var reservaHotel = await _reservaHotelService.AgregarHotelAReservaAsync(id, dto);
            _logger.LogInformation("✅ Hotel agregado exitosamente. ID de relación: {Id}", reservaHotel.Id);

            return CreatedAtAction(
                nameof(ObtenerHotelDeReserva),
                new { id, idReservaHotel = reservaHotel.Id },
                reservaHotel);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Recurso no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Datos inválidos");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación no válida");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al agregar hotel a la reserva");
            return StatusCode(500, new { message = "Error al agregar hotel a la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener todos los hoteles de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/1/hoteles
    ///
    /// Devuelve la lista de todos los hoteles incluidos en la reserva con:
    /// - Información del hotel (nombre, ciudad)
    /// - Fechas de check-in y check-out
    /// - Número de habitaciones y huéspedes
    /// - Subtotal calculado
    /// - Propiedades computadas (número de noches, días hasta check-in, etc.)
    /// </remarks>
    /// <response code="200">Lista de hoteles obtenida exitosamente</response>
    [HttpGet("{id}/hoteles")]
    [ProducesResponseType(typeof(IEnumerable<ReservaHotelResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservaHotelResponseDto>>> ObtenerHotelesPorReserva(int id)
    {
        try
        {
            _logger.LogInformation("📋 Obteniendo hoteles de la reserva {IdReserva}", id);
            var hoteles = await _reservaHotelService.ObtenerHotelesPorReservaAsync(id);
            _logger.LogInformation("✅ {Count} hoteles encontrados", hoteles.Count());
            return Ok(hoteles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener hoteles de la reserva");
            return StatusCode(500, new { message = "Error al obtener hoteles de la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener información detallada de un hotel específico en una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaHotel">ID de la relación ReservaHotel</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/1/hoteles/5
    ///
    /// </remarks>
    /// <response code="200">Información del hotel obtenida exitosamente</response>
    /// <response code="404">Hotel no encontrado en esta reserva</response>
    [HttpGet("{id}/hoteles/{idReservaHotel}")]
    [ProducesResponseType(typeof(ReservaHotelResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaHotelResponseDto>> ObtenerHotelDeReserva(int id, int idReservaHotel)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo hotel {IdReservaHotel} de la reserva {IdReserva}", idReservaHotel, id);
            var reservaHotel = await _reservaHotelService.ObtenerPorIdAsync(idReservaHotel);

            // Verificar que el hotel pertenece a esta reserva
            if (reservaHotel.IdReserva != id)
            {
                _logger.LogWarning("⚠️ El hotel {IdReservaHotel} no pertenece a la reserva {IdReserva}", idReservaHotel, id);
                return NotFound(new { message = "El hotel especificado no pertenece a esta reserva" });
            }

            return Ok(reservaHotel);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Hotel no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener información del hotel");
            return StatusCode(500, new { message = "Error al obtener información del hotel", error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un hotel de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaHotel">ID de la relación ReservaHotel</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     DELETE /api/reservas/1/hoteles/5
    ///
    /// Al eliminar un hotel:
    /// - Se elimina la relación ReservaHotel
    /// - Se recalcula automáticamente el monto total de la reserva
    /// - Se actualiza el saldo pendiente
    /// </remarks>
    /// <response code="200">Hotel eliminado exitosamente de la reserva</response>
    /// <response code="404">Hotel no encontrado en esta reserva</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.eliminar</response>
    [HttpDelete("{id}/hoteles/{idReservaHotel}")]
    [Authorize(Policy = "RequirePermission:reservas.eliminar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EliminarHotelDeReserva(int id, int idReservaHotel)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando hotel {IdReservaHotel} de la reserva {IdReserva}", idReservaHotel, id);

            // Primero verificar que el hotel pertenece a esta reserva
            var reservaHotel = await _reservaHotelService.ObtenerPorIdAsync(idReservaHotel);
            if (reservaHotel.IdReserva != id)
            {
                _logger.LogWarning("⚠️ El hotel {IdReservaHotel} no pertenece a la reserva {IdReserva}", idReservaHotel, id);
                return NotFound(new { message = "El hotel especificado no pertenece a esta reserva" });
            }

            await _reservaHotelService.EliminarHotelDeReservaAsync(idReservaHotel);
            _logger.LogInformation("✅ Hotel eliminado exitosamente de la reserva");

            return Ok(new { message = "Hotel eliminado exitosamente de la reserva" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Hotel no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar hotel de la reserva");
            return StatusCode(500, new { message = "Error al eliminar hotel de la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE GESTIÓN DE VUELOS EN RESERVAS
    // ========================================

    /// <summary>
    /// Agregar un vuelo a una reserva existente
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="dto">Datos del vuelo a agregar</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas/1/vuelos
    ///     {
    ///         "idVuelo": 3,
    ///         "numeroPasajeros": 2,
    ///         "clase": "economica",
    ///         "asientosAsignados": "[\"12A\", \"12B\"]",
    ///         "equipajeIncluido": true,
    ///         "equipajeExtra": 10,
    ///         "costoEquipajeExtra": 50000
    ///     }
    ///
    /// El sistema realiza automáticamente:
    /// - Valida cupos disponibles en el vuelo
    /// - Descuenta cupos del vuelo
    /// - Calcula precio según clase (económica/ejecutiva)
    /// - Calcula subtotal (pasajeros * precio + equipaje extra)
    /// - Actualiza el monto total de la reserva
    /// </remarks>
    /// <response code="201">Vuelo agregado exitosamente a la reserva</response>
    /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
    /// <response code="404">Reserva o Vuelo no encontrado</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost("{id}/vuelos")]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaVueloResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaVueloResponseDto>> AgregarVueloAReserva(int id, [FromBody] ReservaVueloCreateDto dto)
    {
        try
        {
            _logger.LogInformation("🛫 Agregando vuelo {IdVuelo} a la reserva {IdReserva}", dto.IdVuelo, id);

            // Asignar el ID de la reserva desde la ruta al DTO
            dto.IdReserva = id;

            var reservaVuelo = await _reservaVueloService.AgregarVueloAReservaAsync(dto);
            _logger.LogInformation("✅ Vuelo agregado exitosamente. ID de relación: {Id}", reservaVuelo.Id);

            return CreatedAtAction(
                nameof(ObtenerVueloDeReserva),
                new { id, idReservaVuelo = reservaVuelo.Id },
                reservaVuelo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Recurso no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Datos inválidos");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación no válida");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al agregar vuelo a la reserva");
            return StatusCode(500, new { message = "Error al agregar vuelo a la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener todos los vuelos de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/1/vuelos
    ///
    /// Devuelve la lista de todos los vuelos incluidos en la reserva con:
    /// - Información del vuelo (número, origen, destino, fechas)
    /// - Nombre de la aerolínea
    /// - Número de pasajeros y clase
    /// - Subtotal calculado
    /// - Información de equipaje
    /// - Propiedades computadas
    /// </remarks>
    /// <response code="200">Lista de vuelos obtenida exitosamente</response>
    [HttpGet("{id}/vuelos")]
    [ProducesResponseType(typeof(IEnumerable<ReservaVueloResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservaVueloResponseDto>>> ObtenerVuelosPorReserva(int id)
    {
        try
        {
            _logger.LogInformation("📋 Obteniendo vuelos de la reserva {IdReserva}", id);
            var vuelos = await _reservaVueloService.GetVuelosPorReservaAsync(id);
            _logger.LogInformation("✅ {Count} vuelos encontrados", vuelos.Count());
            return Ok(vuelos);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva no encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener vuelos de la reserva");
            return StatusCode(500, new { message = "Error al obtener vuelos de la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener información detallada de un vuelo específico en una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaVuelo">ID de la relación ReservaVuelo</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     GET /api/reservas/1/vuelos/5
    ///
    /// </remarks>
    /// <response code="200">Información del vuelo obtenida exitosamente</response>
    /// <response code="404">Vuelo no encontrado en esta reserva</response>
    [HttpGet("{id}/vuelos/{idReservaVuelo}")]
    [ProducesResponseType(typeof(ReservaVueloResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaVueloResponseDto>> ObtenerVueloDeReserva(int id, int idReservaVuelo)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo vuelo {IdReservaVuelo} de la reserva {IdReserva}", idReservaVuelo, id);
            var reservaVuelo = await _reservaVueloService.GetReservaVueloPorIdAsync(idReservaVuelo);

            // Verificar que el vuelo pertenece a esta reserva
            if (reservaVuelo.IdReserva != id)
            {
                _logger.LogWarning("⚠️ El vuelo {IdReservaVuelo} no pertenece a la reserva {IdReserva}", idReservaVuelo, id);
                return NotFound(new { message = "El vuelo especificado no pertenece a esta reserva" });
            }

            return Ok(reservaVuelo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Vuelo no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener información del vuelo");
            return StatusCode(500, new { message = "Error al obtener información del vuelo", error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un vuelo de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaVuelo">ID de la relación ReservaVuelo</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     DELETE /api/reservas/1/vuelos/5
    ///
    /// Al eliminar un vuelo:
    /// - Se elimina la relación ReservaVuelo
    /// - Se devuelven los cupos al vuelo
    /// - Se recalcula automáticamente el monto total de la reserva
    /// - Se actualiza el saldo pendiente
    /// </remarks>
    /// <response code="200">Vuelo eliminado exitosamente de la reserva</response>
    /// <response code="404">Vuelo no encontrado en esta reserva</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.eliminar</response>
    [HttpDelete("{id}/vuelos/{idReservaVuelo}")]
    [Authorize(Policy = "RequirePermission:reservas.eliminar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EliminarVueloDeReserva(int id, int idReservaVuelo)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando vuelo {IdReservaVuelo} de la reserva {IdReserva}", idReservaVuelo, id);

            // Primero verificar que el vuelo pertenece a esta reserva
            var reservaVuelo = await _reservaVueloService.GetReservaVueloPorIdAsync(idReservaVuelo);
            if (reservaVuelo.IdReserva != id)
            {
                _logger.LogWarning("⚠️ El vuelo {IdReservaVuelo} no pertenece a la reserva {IdReserva}", idReservaVuelo, id);
                return NotFound(new { message = "El vuelo especificado no pertenece a esta reserva" });
            }

            await _reservaVueloService.EliminarVueloDeReservaAsync(idReservaVuelo);
            _logger.LogInformation("✅ Vuelo eliminado exitosamente de la reserva");

            return Ok(new { message = "Vuelo eliminado exitosamente de la reserva" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Vuelo no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar vuelo de la reserva");
            return StatusCode(500, new { message = "Error al eliminar vuelo de la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE PAQUETES
    // ========================================

    /// <summary>
    /// Agregar un paquete turístico a una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="dto">Datos del paquete a agregar</param>
    /// <returns>Paquete agregado con todos sus detalles</returns>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost("{id}/paquetes")]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaPaqueteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaPaqueteResponseDto>> AgregarPaqueteAReserva(int id, [FromBody] ReservaPaqueteCreateDto dto)
    {
        try
        {
            _logger.LogInformation("📦 Agregando paquete {IdPaquete} a reserva {IdReserva}", dto.IdPaquete, id);

            var paqueteAgregado = await _reservaPaqueteService.AgregarPaqueteAReservaAsync(id, dto);

            _logger.LogInformation("✅ Paquete agregado exitosamente a la reserva");

            return CreatedAtAction(
                nameof(ObtenerPaquetePorId),
                new { id, idReservaPaquete = paqueteAgregado.Id },
                paqueteAgregado);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva o paquete no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida al agregar paquete");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al agregar paquete a la reserva");
            return StatusCode(500, new { message = "Error al agregar paquete a la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener todos los paquetes de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <returns>Lista de paquetes de la reserva</returns>
    [HttpGet("{id}/paquetes")]
    [ProducesResponseType(typeof(IEnumerable<ReservaPaqueteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservaPaqueteResponseDto>>> ObtenerPaquetesDeReserva(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo paquetes de la reserva {IdReserva}", id);

            var paquetes = await _reservaPaqueteService.ObtenerPaquetesPorReservaAsync(id);

            _logger.LogInformation("✅ Se encontraron {Count} paquetes", paquetes.Count());

            return Ok(paquetes);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva no encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener paquetes de la reserva");
            return StatusCode(500, new { message = "Error al obtener paquetes de la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un paquete específico de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaPaquete">ID de la relación reserva-paquete</param>
    /// <returns>Detalles del paquete</returns>
    [HttpGet("{id}/paquetes/{idReservaPaquete}")]
    [ProducesResponseType(typeof(ReservaPaqueteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaPaqueteResponseDto>> ObtenerPaquetePorId(int id, int idReservaPaquete)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo paquete {IdReservaPaquete} de reserva {IdReserva}", idReservaPaquete, id);

            var paquete = await _reservaPaqueteService.ObtenerPaquetePorIdAsync(idReservaPaquete);

            // Validar que el paquete pertenezca a la reserva especificada
            if (paquete.IdReserva != id)
            {
                _logger.LogWarning("⚠️ El paquete no pertenece a la reserva especificada");
                return NotFound(new { message = "El paquete no pertenece a la reserva especificada" });
            }

            _logger.LogInformation("✅ Paquete encontrado: {Nombre}", paquete.NombrePaquete);

            return Ok(paquete);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Paquete no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener paquete de la reserva");
            return StatusCode(500, new { message = "Error al obtener paquete de la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un paquete de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaPaquete">ID de la relación reserva-paquete</param>
    /// <returns>Mensaje de confirmación</returns>
    /// <response code="403">Si el usuario no tiene el permiso reservas.eliminar</response>
    [HttpDelete("{id}/paquetes/{idReservaPaquete}")]
    [Authorize(Policy = "RequirePermission:reservas.eliminar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EliminarPaqueteDeReserva(int id, int idReservaPaquete)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando paquete {IdReservaPaquete} de reserva {IdReserva}", idReservaPaquete, id);

            var resultado = await _reservaPaqueteService.EliminarPaqueteDeReservaAsync(id, idReservaPaquete);

            if (!resultado)
            {
                return BadRequest(new { message = "No se pudo eliminar el paquete" });
            }

            _logger.LogInformation("✅ Paquete eliminado exitosamente");

            return Ok(new { message = "Paquete eliminado exitosamente de la reserva" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Paquete no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida al eliminar paquete");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar paquete de la reserva");
            return StatusCode(500, new { message = "Error al eliminar paquete de la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS PARA SERVICIOS ADICIONALES
    // ========================================

    /// <summary>
    /// Agregar un servicio adicional a una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="dto">Datos del servicio a agregar</param>
    /// <returns>Servicio agregado</returns>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost("{id}/servicios")]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaServicioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaServicioResponseDto>> AgregarServicioAReserva(int id, [FromBody] ReservaServicioCreateDto dto)
    {
        try
        {
            _logger.LogInformation("🎯 Agregando servicio a reserva {IdReserva}", id);

            // Asegurar que el ID de la reserva coincida
            dto.IdReserva = id;

            var servicio = await _reservaServicioService.AgregarServicioAReservaAsync(dto);

            _logger.LogInformation("✅ Servicio agregado exitosamente");

            return CreatedAtAction(
                nameof(GetServiciosDeReserva),
                new { id = servicio.IdReserva },
                servicio);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva o servicio no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Error de validación al agregar servicio");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al agregar servicio a la reserva");
            return StatusCode(500, new { message = "Error al agregar servicio a la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener todos los servicios adicionales de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <returns>Lista de servicios de la reserva</returns>
    [HttpGet("{id}/servicios")]
    [ProducesResponseType(typeof(IEnumerable<ReservaServicioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservaServicioResponseDto>>> GetServiciosDeReserva(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo servicios de reserva {IdReserva}", id);

            var servicios = await _reservaServicioService.GetServiciosPorReservaAsync(id);

            _logger.LogInformation("✅ Se encontraron {Count} servicios", servicios.Count());

            return Ok(servicios);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva no encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener servicios de la reserva");
            return StatusCode(500, new { message = "Error al obtener servicios de la reserva", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener servicios de una reserva filtrados por estado
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="estado">Estado a filtrar (pendiente, confirmado, completado, cancelado)</param>
    /// <returns>Lista de servicios filtrados</returns>
    [HttpGet("{id}/servicios/estado/{estado}")]
    [ProducesResponseType(typeof(IEnumerable<ReservaServicioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservaServicioResponseDto>>> GetServiciosPorEstado(int id, string estado)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo servicios de reserva {IdReserva} con estado {Estado}", id, estado);

            var servicios = await _reservaServicioService.GetServiciosPorReservaYEstadoAsync(id, estado);

            _logger.LogInformation("✅ Se encontraron {Count} servicios con estado '{Estado}'", servicios.Count(), estado);

            return Ok(servicios);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Reserva no encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener servicios por estado");
            return StatusCode(500, new { message = "Error al obtener servicios por estado", error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar un servicio adicional de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <param name="idReservaServicio">ID de la relación reserva-servicio</param>
    /// <returns>Mensaje de confirmación</returns>
    /// <response code="403">Si el usuario no tiene el permiso reservas.eliminar</response>
    [HttpDelete("{id}/servicios/{idReservaServicio}")]
    [Authorize(Policy = "RequirePermission:reservas.eliminar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EliminarServicioDeReserva(int id, int idReservaServicio)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando servicio {IdReservaServicio} de reserva {IdReserva}", idReservaServicio, id);

            var resultado = await _reservaServicioService.EliminarServicioDeReservaAsync(idReservaServicio);

            if (!resultado)
            {
                return BadRequest(new { message = "No se pudo eliminar el servicio" });
            }

            _logger.LogInformation("✅ Servicio eliminado exitosamente");

            return Ok(new { message = "Servicio eliminado exitosamente de la reserva" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Servicio no encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida al eliminar servicio");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar servicio de la reserva");
            return StatusCode(500, new { message = "Error al eliminar servicio de la reserva", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINT PARA CREAR RESERVA COMPLETA
    // ========================================

    /// <summary>
    /// Crear una reserva completa con todos los servicios en una sola petición
    /// </summary>
    /// <param name="reservaCompletaDto">Datos completos de la reserva (incluye hoteles, vuelos, paquetes y servicios)</param>
    /// <remarks>
    /// Ejemplo de request:
    ///
    ///     POST /api/reservas/completa
    ///     {
    ///         "idCliente": 1,
    ///         "idEmpleado": 2,
    ///         "descripcion": "Viaje familiar a Cartagena - Vacaciones de fin de año",
    ///         "fechaInicioViaje": "2025-12-20",
    ///         "fechaFinViaje": "2025-12-27",
    ///         "numeroPasajeros": 4,
    ///         "estado": "pendiente",
    ///         "observaciones": "Cliente requiere habitación con vista al mar",
    ///         "hoteles": [
    ///             {
    ///                 "idHotel": 1,
    ///                 "fechaCheckin": "2025-12-20",
    ///                 "fechaCheckout": "2025-12-27",
    ///                 "numeroHabitaciones": 2,
    ///                 "tipoHabitacion": "doble",
    ///                 "numeroHuespedes": 4,
    ///                 "observaciones": "Habitaciones contiguas"
    ///             }
    ///         ],
    ///         "vuelos": [
    ///             {
    ///                 "idVuelo": 1,
    ///                 "numeroPasajeros": 4,
    ///                 "clase": "economica",
    ///                 "equipajeIncluido": true
    ///             }
    ///         ],
    ///         "paquetes": [],
    ///         "servicios": [
    ///             {
    ///                 "idServicio": 1,
    ///                 "cantidad": 1,
    ///                 "observaciones": "Tour guiado por la ciudad"
    ///             }
    ///         ]
    ///     }
    ///
    /// El sistema realiza automáticamente:
    /// - Validación de disponibilidad de todos los servicios
    /// - Cálculo de subtotales de cada servicio
    /// - Cálculo del monto total de la reserva
    /// - Descuento de cupos de vuelos
    /// - Actualización de totales en una transacción
    ///
    /// Nota: Debe incluir al menos un servicio (hotel, vuelo, paquete o servicio adicional)
    /// </remarks>
    /// <response code="201">Reserva completa creada exitosamente con todos los servicios</response>
    /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
    /// <response code="404">Cliente, empleado o algún servicio no encontrado</response>
    /// <response code="403">Si el usuario no tiene el permiso reservas.crear</response>
    [HttpPost("completa")]
    [Authorize(Policy = "RequirePermission:reservas.crear")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaResponseDto>> CrearReservaCompleta([FromBody] ReservaCompletaCreateDto reservaCompletaDto)
    {
        try
        {
            _logger.LogInformation("🚀 Recibida petición para crear reserva completa");
            _logger.LogInformation("   Cliente: {IdCliente}, Empleado: {IdEmpleado}",
                reservaCompletaDto.IdCliente, reservaCompletaDto.IdEmpleado);
            _logger.LogInformation("   Servicios: {Hoteles} hoteles, {Vuelos} vuelos, {Paquetes} paquetes, {Servicios} servicios adicionales",
                reservaCompletaDto.Hoteles.Count,
                reservaCompletaDto.Vuelos.Count,
                reservaCompletaDto.Paquetes.Count,
                reservaCompletaDto.Servicios.Count);

            var reservaCreada = await _reservaService.CreateReservaCompletaAsync(reservaCompletaDto);

            _logger.LogInformation("✅ Reserva completa creada exitosamente con ID: {IdReserva}", reservaCreada.IdReserva);
            _logger.LogInformation("💰 Monto total: {MontoTotal:C}", reservaCreada.MontoTotal);

            return CreatedAtAction(
                nameof(GetReservaById),
                new { id = reservaCreada.IdReserva },
                reservaCreada
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Recurso no encontrado al crear reserva completa");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "⚠️ Argumentos inválidos al crear reserva completa");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Operación inválida al crear reserva completa");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inesperado al crear reserva completa");
            return StatusCode(500, new {
                message = "Error al crear la reserva completa",
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }
}

/// <summary>
/// DTO auxiliar para la cancelación de reservas
/// </summary>
public class CancelarReservaDto
{
    /// <summary>
    /// Motivo de la cancelación
    /// </summary>
    public string MotivoCancelacion { get; set; } = string.Empty;
}
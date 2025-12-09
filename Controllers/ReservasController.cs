using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using G2rismBeta.API.DTOs.Reserva;
using G2rismBeta.API.DTOs.ReservaHotel;
using G2rismBeta.API.DTOs.ReservaVuelo;
using G2rismBeta.API.DTOs.ReservaPaquete;
using G2rismBeta.API.Interfaces;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Reservas
/// Endpoints para operaciones CRUD básicas de reservas
/// Requiere autenticación. Accesible para empleados (Super Admin, Admin, Empleado).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Super Administrador,Administrador,Empleado")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;
    private readonly IReservaHotelService _reservaHotelService;
    private readonly IReservaVueloService _reservaVueloService;
    private readonly IReservaPaqueteService _reservaPaqueteService;
    private readonly ILogger<ReservasController> _logger;

    /// <summary>
    /// Constructor: Recibe los servicios de reservas y logger por inyección de dependencias
    /// </summary>
    public ReservasController(
        IReservaService reservaService,
        IReservaHotelService reservaHotelService,
        IReservaVueloService reservaVueloService,
        IReservaPaqueteService reservaPaqueteService,
        ILogger<ReservasController> logger)
    {
        _reservaService = reservaService;
        _reservaHotelService = reservaHotelService;
        _reservaVueloService = reservaVueloService;
        _reservaPaqueteService = reservaPaqueteService;
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
    [HttpPost]
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
    [HttpPut("{id}")]
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
    [HttpPost("{id}/confirmar")]
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
    [HttpPost("{id}/cancelar")]
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
    [HttpPost("{id}/hoteles")]
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
    [HttpDelete("{id}/hoteles/{idReservaHotel}")]
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
    [HttpPost("{id}/vuelos")]
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
    [HttpDelete("{id}/vuelos/{idReservaVuelo}")]
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
    [HttpPost("{id}/paquetes")]
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
    [HttpDelete("{id}/paquetes/{idReservaPaquete}")]
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
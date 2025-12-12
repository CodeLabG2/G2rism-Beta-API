using AutoMapper;
using G2rismBeta.API.DTOs.Reserva;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio que contiene toda la lógica de negocio para gestión de Reservas
/// </summary>
public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IReservaHotelService _reservaHotelService;
    private readonly IReservaVueloService _reservaVueloService;
    private readonly IReservaPaqueteService _reservaPaqueteService;
    private readonly IReservaServicioService _reservaServicioService;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservaService> _logger;

    /// <summary>
    /// Constructor: Recibe los repositories y servicios necesarios y AutoMapper
    /// </summary>
    public ReservaService(
        IReservaRepository reservaRepository,
        IClienteRepository clienteRepository,
        IEmpleadoRepository empleadoRepository,
        IReservaHotelService reservaHotelService,
        IReservaVueloService reservaVueloService,
        IReservaPaqueteService reservaPaqueteService,
        IReservaServicioService reservaServicioService,
        IMapper mapper,
        ILogger<ReservaService> logger)
    {
        _reservaRepository = reservaRepository;
        _clienteRepository = clienteRepository;
        _empleadoRepository = empleadoRepository;
        _reservaHotelService = reservaHotelService;
        _reservaVueloService = reservaVueloService;
        _reservaPaqueteService = reservaPaqueteService;
        _reservaServicioService = reservaServicioService;
        _mapper = mapper;
        _logger = logger;
    }

    // ========================================
    // OPERACIONES CRUD BÁSICAS
    // ========================================

    /// <summary>
    /// Obtener todas las reservas del sistema
    /// </summary>
    public async Task<IEnumerable<ReservaResponseDto>> GetAllReservasAsync()
    {
        var reservas = await _reservaRepository.GetAllConDetallesAsync();
        return _mapper.Map<IEnumerable<ReservaResponseDto>>(reservas);
    }

    /// <summary>
    /// Obtener una reserva por su ID
    /// </summary>
    public async Task<ReservaResponseDto?> GetReservaByIdAsync(int idReserva)
    {
        // 1. Validar ID
        if (idReserva <= 0)
        {
            throw new ArgumentException("El ID de la reserva debe ser mayor a 0", nameof(idReserva));
        }

        // 2. Buscar la reserva con detalles
        var reserva = await _reservaRepository.GetReservaConDetallesAsync(idReserva);

        // 3. Si no existe, retornar null
        if (reserva == null)
        {
            return null;
        }

        // 4. Convertir a DTO
        return _mapper.Map<ReservaResponseDto>(reserva);
    }

    /// <summary>
    /// Crear una nueva reserva básica (sin servicios)
    /// </summary>
    public async Task<ReservaResponseDto> CreateReservaAsync(ReservaCreateDto reservaCreateDto)
    {
        // ========================================
        // VALIDACIONES DE NEGOCIO
        // ========================================

        // 1. Validar que el cliente exista
        var clienteExiste = await _clienteRepository.ExistsAsync(reservaCreateDto.IdCliente);
        if (!clienteExiste)
        {
            throw new KeyNotFoundException($"El cliente con ID {reservaCreateDto.IdCliente} no existe");
        }

        // 2. Validar que el empleado exista
        var empleadoExiste = await _empleadoRepository.ExistsAsync(reservaCreateDto.IdEmpleado);
        if (!empleadoExiste)
        {
            throw new KeyNotFoundException($"El empleado con ID {reservaCreateDto.IdEmpleado} no existe");
        }

        // 3. Validar que la fecha de fin sea posterior a la fecha de inicio
        if (reservaCreateDto.FechaFinViaje <= reservaCreateDto.FechaInicioViaje)
        {
            throw new InvalidOperationException("La fecha de fin del viaje debe ser posterior a la fecha de inicio");
        }

        // 4. Validar que la fecha de inicio no sea en el pasado
        if (reservaCreateDto.FechaInicioViaje.Date < DateTime.Today)
        {
            throw new InvalidOperationException("La fecha de inicio del viaje no puede ser en el pasado");
        }

        // 5. Validar número de pasajeros
        if (reservaCreateDto.NumeroPasajeros <= 0)
        {
            throw new ArgumentException("El número de pasajeros debe ser mayor a 0");
        }

        // 6. Validar estado válido
        var estadosValidos = new[] { "pendiente", "confirmada" };
        if (!estadosValidos.Contains(reservaCreateDto.Estado.ToLower()))
        {
            throw new ArgumentException("El estado inicial debe ser 'pendiente' o 'confirmada'");
        }

        // ========================================
        // CREAR RESERVA
        // ========================================

        // 7. Mapear DTO a entidad
        var reserva = _mapper.Map<Reserva>(reservaCreateDto);

        // 8. Establecer valores iniciales
        reserva.FechaHora = DateTime.Now;
        reserva.MontoTotal = 0; // Se calculará al agregar servicios
        reserva.MontoPagado = 0;
        reserva.SaldoPendiente = 0;

        // 9. Guardar en la base de datos
        var reservaCreada = await _reservaRepository.AddAsync(reserva);
        await _reservaRepository.SaveChangesAsync();

        // 10. Obtener la reserva completa con navegación
        var reservaCompleta = await _reservaRepository.GetReservaConDetallesAsync(reservaCreada.IdReserva);

        // 11. Convertir a DTO y retornar
        return _mapper.Map<ReservaResponseDto>(reservaCompleta);
    }

    /// <summary>
    /// Actualizar una reserva existente
    /// </summary>
    public async Task<ReservaResponseDto> UpdateReservaAsync(int idReserva, ReservaUpdateDto reservaUpdateDto)
    {
        // 1. Validar que la reserva exista
        var reservaExistente = await _reservaRepository.GetByIdAsync(idReserva);
        if (reservaExistente == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que no esté cancelada o completada (no se pueden editar)
        if (reservaExistente.Estado.ToLower() == "cancelada")
        {
            throw new InvalidOperationException("No se puede actualizar una reserva cancelada");
        }

        if (reservaExistente.Estado.ToLower() == "completada")
        {
            throw new InvalidOperationException("No se puede actualizar una reserva completada");
        }

        // 3. Validar empleado si se está cambiando
        if (reservaUpdateDto.IdEmpleado.HasValue)
        {
            var empleadoExiste = await _empleadoRepository.ExistsAsync(reservaUpdateDto.IdEmpleado.Value);
            if (!empleadoExiste)
            {
                throw new KeyNotFoundException($"El empleado con ID {reservaUpdateDto.IdEmpleado} no existe");
            }
        }

        // 4. Validar fechas si se están cambiando
        var fechaInicio = reservaUpdateDto.FechaInicioViaje ?? reservaExistente.FechaInicioViaje;
        var fechaFin = reservaUpdateDto.FechaFinViaje ?? reservaExistente.FechaFinViaje;

        if (fechaFin <= fechaInicio)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio");
        }

        // 5. Validar estado si se está cambiando
        if (reservaUpdateDto.Estado != null)
        {
            var estadosValidos = new[] { "pendiente", "confirmada", "cancelada", "completada" };
            if (!estadosValidos.Contains(reservaUpdateDto.Estado.ToLower()))
            {
                throw new ArgumentException("Estado no válido. Valores permitidos: pendiente, confirmada, cancelada, completada");
            }
        }

        // 6. Actualizar campos individualmente solo si no son null (actualizaciones parciales)
        // IMPORTANTE: No usar AutoMapper aquí porque sobrescribe campos no enviados con valores por defecto
        if (reservaUpdateDto.IdEmpleado.HasValue)
            reservaExistente.IdEmpleado = reservaUpdateDto.IdEmpleado.Value;

        if (reservaUpdateDto.Descripcion != null)
            reservaExistente.Descripcion = reservaUpdateDto.Descripcion;

        if (reservaUpdateDto.FechaInicioViaje.HasValue)
            reservaExistente.FechaInicioViaje = reservaUpdateDto.FechaInicioViaje.Value;

        if (reservaUpdateDto.FechaFinViaje.HasValue)
            reservaExistente.FechaFinViaje = reservaUpdateDto.FechaFinViaje.Value;

        if (reservaUpdateDto.NumeroPasajeros.HasValue)
            reservaExistente.NumeroPasajeros = reservaUpdateDto.NumeroPasajeros.Value;

        if (reservaUpdateDto.Estado != null)
            reservaExistente.Estado = reservaUpdateDto.Estado;

        if (reservaUpdateDto.Observaciones != null)
            reservaExistente.Observaciones = reservaUpdateDto.Observaciones;

        // 7. Actualizar fecha de modificación
        reservaExistente.FechaModificacion = DateTime.Now;

        // 8. Guardar cambios
        await _reservaRepository.UpdateAsync(reservaExistente);
        await _reservaRepository.SaveChangesAsync();

        // 9. Obtener reserva actualizada con navegación
        var reservaActualizada = await _reservaRepository.GetReservaConDetallesAsync(idReserva);

        // 10. Retornar DTO
        return _mapper.Map<ReservaResponseDto>(reservaActualizada);
    }

    /// <summary>
    /// Eliminar una reserva (solo si está en estado pendiente)
    /// </summary>
    public async Task<bool> DeleteReservaAsync(int idReserva)
    {
        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que esté en estado pendiente
        if (reserva.Estado.ToLower() != "pendiente")
        {
            throw new InvalidOperationException("Solo se pueden eliminar reservas en estado 'pendiente'");
        }

        // 3. Eliminar la reserva
        var resultado = await _reservaRepository.DeleteAsync(idReserva);
        await _reservaRepository.SaveChangesAsync();

        return resultado;
    }

    // ========================================
    // CONSULTAS ESPECIALIZADAS
    // ========================================

    /// <summary>
    /// Obtener todas las reservas de un cliente
    /// </summary>
    public async Task<IEnumerable<ReservaResponseDto>> GetReservasByClienteAsync(int idCliente)
    {
        // Validar que el cliente exista
        var clienteExiste = await _clienteRepository.ExistsAsync(idCliente);
        if (!clienteExiste)
        {
            throw new KeyNotFoundException($"El cliente con ID {idCliente} no existe");
        }

        var reservas = await _reservaRepository.GetReservasByClienteAsync(idCliente);
        return _mapper.Map<IEnumerable<ReservaResponseDto>>(reservas);
    }

    /// <summary>
    /// Obtener todas las reservas gestionadas por un empleado
    /// </summary>
    public async Task<IEnumerable<ReservaResponseDto>> GetReservasByEmpleadoAsync(int idEmpleado)
    {
        // Validar que el empleado exista
        var empleadoExiste = await _empleadoRepository.ExistsAsync(idEmpleado);
        if (!empleadoExiste)
        {
            throw new KeyNotFoundException($"El empleado con ID {idEmpleado} no existe");
        }

        var reservas = await _reservaRepository.GetReservasByEmpleadoAsync(idEmpleado);
        return _mapper.Map<IEnumerable<ReservaResponseDto>>(reservas);
    }

    /// <summary>
    /// Obtener reservas por estado
    /// </summary>
    public async Task<IEnumerable<ReservaResponseDto>> GetReservasByEstadoAsync(string estado)
    {
        // Validar estado
        var estadosValidos = new[] { "pendiente", "confirmada", "cancelada", "completada" };
        if (!estadosValidos.Contains(estado.ToLower()))
        {
            throw new ArgumentException("Estado no válido", nameof(estado));
        }

        var reservas = await _reservaRepository.GetReservasByEstadoAsync(estado);
        return _mapper.Map<IEnumerable<ReservaResponseDto>>(reservas);
    }

    /// <summary>
    /// Obtener reservas en un rango de fechas
    /// </summary>
    public async Task<IEnumerable<ReservaResponseDto>> GetReservasByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        // Validar fechas
        if (fechaFin < fechaInicio)
        {
            throw new ArgumentException("La fecha final debe ser posterior o igual a la fecha inicial");
        }

        var reservas = await _reservaRepository.GetReservasByRangoFechasAsync(fechaInicio, fechaFin);
        return _mapper.Map<IEnumerable<ReservaResponseDto>>(reservas);
    }

    // ========================================
    // OPERACIONES DE NEGOCIO
    // ========================================

    /// <summary>
    /// Cambiar el estado de una reserva
    /// </summary>
    public async Task<ReservaResponseDto> CambiarEstadoReservaAsync(int idReserva, string nuevoEstado)
    {
        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar nuevo estado
        var estadosValidos = new[] { "pendiente", "confirmada", "cancelada", "completada" };
        if (!estadosValidos.Contains(nuevoEstado.ToLower()))
        {
            throw new ArgumentException("Estado no válido", nameof(nuevoEstado));
        }

        // 3. Validar transiciones de estado
        var estadoActual = reserva.Estado.ToLower();

        // No se puede cambiar de cancelada o completada
        if (estadoActual == "cancelada" || estadoActual == "completada")
        {
            throw new InvalidOperationException($"No se puede cambiar el estado de una reserva {estadoActual}");
        }

        // 4. Cambiar el estado
        reserva.Estado = nuevoEstado.ToLower();
        reserva.FechaModificacion = DateTime.Now;

        await _reservaRepository.UpdateAsync(reserva);
        await _reservaRepository.SaveChangesAsync();

        // 5. Retornar reserva actualizada
        var reservaActualizada = await _reservaRepository.GetReservaConDetallesAsync(idReserva);
        return _mapper.Map<ReservaResponseDto>(reservaActualizada);
    }

    /// <summary>
    /// Cancelar una reserva
    /// </summary>
    public async Task<ReservaResponseDto> CancelarReservaAsync(int idReserva, string motivoCancelacion)
    {
        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que no esté ya cancelada o completada
        if (reserva.Estado.ToLower() == "cancelada")
        {
            throw new InvalidOperationException("La reserva ya está cancelada");
        }

        if (reserva.Estado.ToLower() == "completada")
        {
            throw new InvalidOperationException("No se puede cancelar una reserva completada");
        }

        // 3. Cancelar la reserva
        reserva.Estado = "cancelada";
        reserva.Observaciones = $"CANCELADA - {motivoCancelacion}\n{reserva.Observaciones}";
        reserva.FechaModificacion = DateTime.Now;

        await _reservaRepository.UpdateAsync(reserva);
        await _reservaRepository.SaveChangesAsync();

        // TODO: En Día 3 Tarea 2-4, liberar cupos de servicios (hoteles, vuelos, etc.)

        // 4. Retornar reserva actualizada
        var reservaActualizada = await _reservaRepository.GetReservaConDetallesAsync(idReserva);
        return _mapper.Map<ReservaResponseDto>(reservaActualizada);
    }

    /// <summary>
    /// Confirmar una reserva (cambiar de pendiente a confirmada)
    /// </summary>
    public async Task<ReservaResponseDto> ConfirmarReservaAsync(int idReserva)
    {
        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que esté en estado pendiente
        if (reserva.Estado.ToLower() != "pendiente")
        {
            throw new InvalidOperationException("Solo se pueden confirmar reservas en estado 'pendiente'");
        }

        // 3. Confirmar la reserva
        reserva.Estado = "confirmada";
        reserva.FechaModificacion = DateTime.Now;

        await _reservaRepository.UpdateAsync(reserva);
        await _reservaRepository.SaveChangesAsync();

        // 4. Retornar reserva actualizada
        var reservaActualizada = await _reservaRepository.GetReservaConDetallesAsync(idReserva);
        return _mapper.Map<ReservaResponseDto>(reservaActualizada);
    }

    /// <summary>
    /// Recalcular los montos de una reserva
    /// Se usa después de agregar/quitar servicios
    /// </summary>
    public async Task<bool> RecalcularMontosAsync(int idReserva)
    {
        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetReservaConDetallesAsync(idReserva);
        if (reserva == null)
        {
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Calcular total de servicios
        decimal montoTotal = 0;

        // TODO: En Día 3 Tarea 2-4, sumar montos de todos los servicios
        // montoTotal += reserva.ReservasHoteles.Sum(rh => rh.Subtotal);
        // montoTotal += reserva.ReservasVuelos.Sum(rv => rv.Subtotal);
        // montoTotal += reserva.ReservasPaquetes.Sum(rp => rp.Subtotal);
        // montoTotal += reserva.ReservasServicios.Sum(rs => rs.Subtotal);

        // 3. Actualizar montos
        return await _reservaRepository.ActualizarMontosAsync(idReserva, montoTotal, reserva.MontoPagado);
    }

    // ========================================
    // VALIDACIONES
    // ========================================

    /// <summary>
    /// Verificar si un cliente existe
    /// </summary>
    public async Task<bool> ClienteExisteAsync(int idCliente)
    {
        return await _clienteRepository.ExistsAsync(idCliente);
    }

    /// <summary>
    /// Verificar si un empleado existe
    /// </summary>
    public async Task<bool> EmpleadoExisteAsync(int idEmpleado)
    {
        return await _empleadoRepository.ExistsAsync(idEmpleado);
    }

    /// <summary>
    /// Verificar si una reserva existe
    /// </summary>
    public async Task<bool> ReservaExisteAsync(int idReserva)
    {
        return await _reservaRepository.ExistsAsync(idReserva);
    }

    // ========================================
    // CREACIÓN DE RESERVA COMPLETA
    // ========================================

    /// <summary>
    /// Crear una reserva completa con todos los servicios en una sola transacción atómica
    /// Este método garantiza que o se crea todo o no se crea nada (transacción ACID)
    /// </summary>
    public async Task<ReservaResponseDto> CreateReservaCompletaAsync(ReservaCompletaCreateDto reservaCompletaDto)
    {
        _logger.LogInformation("🚀 Iniciando creación de reserva completa para cliente {IdCliente}",
            reservaCompletaDto.IdCliente);

        // ========================================
        // VALIDACIONES INICIALES
        // ========================================

        // 1. Validar que el cliente exista
        var clienteExiste = await _clienteRepository.ExistsAsync(reservaCompletaDto.IdCliente);
        if (!clienteExiste)
        {
            throw new KeyNotFoundException($"El cliente con ID {reservaCompletaDto.IdCliente} no existe");
        }

        // 2. Validar que el empleado exista
        var empleadoExiste = await _empleadoRepository.ExistsAsync(reservaCompletaDto.IdEmpleado);
        if (!empleadoExiste)
        {
            throw new KeyNotFoundException($"El empleado con ID {reservaCompletaDto.IdEmpleado} no existe");
        }

        // 3. Validar fechas
        if (reservaCompletaDto.FechaFinViaje <= reservaCompletaDto.FechaInicioViaje)
        {
            throw new InvalidOperationException("La fecha de fin del viaje debe ser posterior a la fecha de inicio");
        }

        if (reservaCompletaDto.FechaInicioViaje.Date < DateTime.Today)
        {
            throw new InvalidOperationException("La fecha de inicio del viaje no puede ser en el pasado");
        }

        // 4. Validar número de pasajeros
        if (reservaCompletaDto.NumeroPasajeros <= 0)
        {
            throw new ArgumentException("El número de pasajeros debe ser mayor a 0");
        }

        // 5. Validar que al menos haya un servicio
        var totalServicios = reservaCompletaDto.Hoteles.Count +
                            reservaCompletaDto.Vuelos.Count +
                            reservaCompletaDto.Paquetes.Count +
                            reservaCompletaDto.Servicios.Count;

        if (totalServicios == 0)
        {
            throw new InvalidOperationException(
                "Debe incluir al menos un servicio (hotel, vuelo, paquete o servicio adicional)");
        }

        _logger.LogInformation("✅ Validaciones iniciales completadas. Total servicios: {TotalServicios}", totalServicios);

        // ========================================
        // CREAR RESERVA BÁSICA
        // ========================================

        _logger.LogInformation("📝 Creando reserva básica...");

        var reservaCreateDto = new ReservaCreateDto
        {
            IdCliente = reservaCompletaDto.IdCliente,
            IdEmpleado = reservaCompletaDto.IdEmpleado,
            Descripcion = reservaCompletaDto.Descripcion,
            FechaInicioViaje = reservaCompletaDto.FechaInicioViaje,
            FechaFinViaje = reservaCompletaDto.FechaFinViaje,
            NumeroPasajeros = reservaCompletaDto.NumeroPasajeros,
            Estado = reservaCompletaDto.Estado,
            Observaciones = reservaCompletaDto.Observaciones
        };

        var reservaCreada = await CreateReservaAsync(reservaCreateDto);
        var idReserva = reservaCreada.IdReserva;

        _logger.LogInformation("✅ Reserva básica creada con ID: {IdReserva}", idReserva);

        try
        {
            // ========================================
            // AGREGAR HOTELES A LA RESERVA
            // ========================================

            if (reservaCompletaDto.Hoteles.Any())
            {
                _logger.LogInformation("🏨 Agregando {Count} hoteles a la reserva...",
                    reservaCompletaDto.Hoteles.Count);

                foreach (var hotelDto in reservaCompletaDto.Hoteles)
                {
                    await _reservaHotelService.AgregarHotelAReservaAsync(idReserva, hotelDto);
                }

                _logger.LogInformation("✅ Hoteles agregados exitosamente");
            }

            // ========================================
            // AGREGAR VUELOS A LA RESERVA
            // ========================================

            if (reservaCompletaDto.Vuelos.Any())
            {
                _logger.LogInformation("✈️ Agregando {Count} vuelos a la reserva...",
                    reservaCompletaDto.Vuelos.Count);

                foreach (var vueloDto in reservaCompletaDto.Vuelos)
                {
                    vueloDto.IdReserva = idReserva;
                    await _reservaVueloService.AgregarVueloAReservaAsync(vueloDto);
                }

                _logger.LogInformation("✅ Vuelos agregados exitosamente");
            }

            // ========================================
            // AGREGAR PAQUETES A LA RESERVA
            // ========================================

            if (reservaCompletaDto.Paquetes.Any())
            {
                _logger.LogInformation("📦 Agregando {Count} paquetes a la reserva...",
                    reservaCompletaDto.Paquetes.Count);

                foreach (var paqueteDto in reservaCompletaDto.Paquetes)
                {
                    await _reservaPaqueteService.AgregarPaqueteAReservaAsync(idReserva, paqueteDto);
                }

                _logger.LogInformation("✅ Paquetes agregados exitosamente");
            }

            // ========================================
            // AGREGAR SERVICIOS ADICIONALES A LA RESERVA
            // ========================================

            if (reservaCompletaDto.Servicios.Any())
            {
                _logger.LogInformation("🎯 Agregando {Count} servicios adicionales a la reserva...",
                    reservaCompletaDto.Servicios.Count);

                foreach (var servicioDto in reservaCompletaDto.Servicios)
                {
                    servicioDto.IdReserva = idReserva;
                    await _reservaServicioService.AgregarServicioAReservaAsync(servicioDto);
                }

                _logger.LogInformation("✅ Servicios adicionales agregados exitosamente");
            }

            // ========================================
            // OBTENER RESERVA COMPLETA Y RETORNAR
            // ========================================

            _logger.LogInformation("📊 Obteniendo reserva completa con todos los detalles...");
            var reservaCompleta = await GetReservaByIdAsync(idReserva);

            _logger.LogInformation("🎉 Reserva completa creada exitosamente!");
            _logger.LogInformation("📈 Resumen:");
            _logger.LogInformation("   - ID Reserva: {IdReserva}", reservaCompleta?.IdReserva);
            _logger.LogInformation("   - Hoteles: {Hoteles}", reservaCompletaDto.Hoteles.Count);
            _logger.LogInformation("   - Vuelos: {Vuelos}", reservaCompletaDto.Vuelos.Count);
            _logger.LogInformation("   - Paquetes: {Paquetes}", reservaCompletaDto.Paquetes.Count);
            _logger.LogInformation("   - Servicios: {Servicios}", reservaCompletaDto.Servicios.Count);
            _logger.LogInformation("   - Monto Total: {MontoTotal:C}", reservaCompleta?.MontoTotal);

            return reservaCompleta!;
        }
        catch (Exception ex)
        {
            // Si algo falla al agregar servicios, registrar el error
            _logger.LogError(ex, "❌ Error al agregar servicios a la reserva {IdReserva}. La reserva fue creada pero puede estar incompleta.", idReserva);

            // Re-lanzar la excepción para que el controlador la maneje
            throw new InvalidOperationException(
                $"La reserva fue creada (ID: {idReserva}) pero ocurrió un error al agregar los servicios: {ex.Message}. " +
                "Por favor, agregue los servicios manualmente o cancele la reserva.", ex);
        }
    }
}
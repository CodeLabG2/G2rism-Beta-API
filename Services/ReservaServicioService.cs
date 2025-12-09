using AutoMapper;
using G2rismBeta.API.DTOs.ReservaServicio;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio para la gestión de servicios adicionales en reservas
/// Maneja la lógica de negocio para agregar, consultar y eliminar servicios de reservas
/// </summary>
public class ReservaServicioService : IReservaServicioService
{
    private readonly IReservaServicioRepository _reservaServicioRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IServicioAdicionalRepository _servicioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservaServicioService> _logger;

    public ReservaServicioService(
        IReservaServicioRepository reservaServicioRepository,
        IReservaRepository reservaRepository,
        IServicioAdicionalRepository servicioRepository,
        IMapper mapper,
        ILogger<ReservaServicioService> logger)
    {
        _reservaServicioRepository = reservaServicioRepository;
        _reservaRepository = reservaRepository;
        _servicioRepository = servicioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Agrega un servicio adicional a una reserva existente
    /// </summary>
    public async Task<ReservaServicioResponseDto> AgregarServicioAReservaAsync(ReservaServicioCreateDto dto)
    {
        _logger.LogInformation("🎯 Agregando servicio {IdServicio} a reserva {IdReserva}", dto.IdServicio, dto.IdReserva);

        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(dto.IdReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", dto.IdReserva);
            throw new KeyNotFoundException($"La reserva con ID {dto.IdReserva} no existe");
        }

        // 2. Validar que el servicio exista y tenga información completa
        var servicio = await _servicioRepository.GetByIdConProveedorAsync(dto.IdServicio);
        if (servicio == null)
        {
            _logger.LogWarning("⚠️ Servicio {IdServicio} no encontrado", dto.IdServicio);
            throw new KeyNotFoundException($"El servicio con ID {dto.IdServicio} no existe");
        }

        // 3. Validar que el servicio esté activo y disponible
        if (!servicio.Estado)
        {
            _logger.LogWarning("⚠️ Servicio {IdServicio} no está activo", dto.IdServicio);
            throw new InvalidOperationException($"El servicio '{servicio.Nombre}' no está activo");
        }

        if (!servicio.Disponibilidad)
        {
            _logger.LogWarning("⚠️ Servicio {IdServicio} no está disponible", dto.IdServicio);
            throw new InvalidOperationException($"El servicio '{servicio.Nombre}' no está disponible actualmente");
        }

        // 4. Validar capacidad máxima si el servicio la tiene definida
        if (servicio.CapacidadMaxima.HasValue && dto.Cantidad > servicio.CapacidadMaxima.Value)
        {
            _logger.LogWarning("⚠️ Cantidad solicitada ({Cantidad}) excede capacidad máxima ({CapacidadMaxima})",
                dto.Cantidad, servicio.CapacidadMaxima.Value);
            throw new InvalidOperationException(
                $"La cantidad solicitada ({dto.Cantidad}) excede la capacidad máxima del servicio ({servicio.CapacidadMaxima.Value})");
        }

        // 5. Validar fecha del servicio si está especificada
        if (dto.FechaServicio.HasValue)
        {
            // La fecha del servicio debe estar dentro del rango de la reserva
            if (dto.FechaServicio.Value < reserva.FechaInicioViaje || dto.FechaServicio.Value > reserva.FechaFinViaje)
            {
                _logger.LogWarning("⚠️ Fecha del servicio ({FechaServicio}) fuera del rango de viaje ({FechaInicio} - {FechaFin})",
                    dto.FechaServicio.Value, reserva.FechaInicioViaje, reserva.FechaFinViaje);
                throw new InvalidOperationException(
                    $"La fecha del servicio debe estar entre {reserva.FechaInicioViaje:d} y {reserva.FechaFinViaje:d}");
            }
        }

        // 6. Calcular el subtotal
        decimal precioUnitario = servicio.Precio;
        decimal subtotal = precioUnitario * dto.Cantidad;

        _logger.LogInformation("💵 Precio unitario: {PrecioUnitario:C}, Cantidad: {Cantidad}, Subtotal: {Subtotal:C}",
            precioUnitario, dto.Cantidad, subtotal);

        // 7. Crear la entidad ReservaServicio
        var reservaServicio = new ReservaServicio
        {
            IdReserva = dto.IdReserva,
            IdServicio = dto.IdServicio,
            Cantidad = dto.Cantidad,
            FechaServicio = dto.FechaServicio,
            HoraServicio = dto.HoraServicio,
            PrecioUnitario = precioUnitario,
            Subtotal = subtotal,
            Observaciones = dto.Observaciones,
            Estado = dto.Estado.ToLower(),
            FechaAgregado = DateTime.Now
        };

        // 8. Guardar la reserva de servicio
        await _reservaServicioRepository.AddAsync(reservaServicio);
        await _reservaServicioRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Servicio agregado a la reserva. Subtotal: {Subtotal:C}", subtotal);

        // 9. Actualizar el MontoTotal de la reserva
        reserva.MontoTotal += subtotal;
        reserva.SaldoPendiente = reserva.MontoTotal - reserva.MontoPagado;
        reserva.FechaModificacion = DateTime.Now;
        await _reservaRepository.SaveChangesAsync();

        _logger.LogInformation("💰 MontoTotal actualizado: {MontoTotal:C}", reserva.MontoTotal);

        // 10. Obtener la reserva de servicio completa para la respuesta
        var reservaServicioCompleta = await _reservaServicioRepository.GetReservaServicioConDetallesAsync(reservaServicio.Id);

        // 11. Mapear a DTO de respuesta
        var response = _mapper.Map<ReservaServicioResponseDto>(reservaServicioCompleta);

        return response;
    }

    /// <summary>
    /// Obtiene todos los servicios de una reserva
    /// </summary>
    public async Task<IEnumerable<ReservaServicioResponseDto>> GetServiciosPorReservaAsync(int idReserva)
    {
        _logger.LogInformation("🔍 Obteniendo servicios de reserva {IdReserva}", idReserva);

        // Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        var servicios = await _reservaServicioRepository.GetServiciosByReservaIdAsync(idReserva);
        var serviciosDto = _mapper.Map<IEnumerable<ReservaServicioResponseDto>>(servicios);

        _logger.LogInformation("✅ Se encontraron {Count} servicios para la reserva", serviciosDto.Count());

        return serviciosDto;
    }

    /// <summary>
    /// Obtiene un servicio específico de una reserva
    /// </summary>
    public async Task<ReservaServicioResponseDto> GetReservaServicioPorIdAsync(int id)
    {
        _logger.LogInformation("🔍 Obteniendo reserva de servicio {Id}", id);

        var reservaServicio = await _reservaServicioRepository.GetReservaServicioConDetallesAsync(id);
        if (reservaServicio == null)
        {
            _logger.LogWarning("⚠️ Reserva de servicio {Id} no encontrada", id);
            throw new KeyNotFoundException($"La reserva de servicio con ID {id} no existe");
        }

        var response = _mapper.Map<ReservaServicioResponseDto>(reservaServicio);

        return response;
    }

    /// <summary>
    /// Elimina un servicio de una reserva y actualiza el total
    /// </summary>
    public async Task<bool> EliminarServicioDeReservaAsync(int id)
    {
        _logger.LogInformation("🗑️ Eliminando servicio {Id} de reserva", id);

        // Obtener la reserva de servicio con detalles
        var reservaServicio = await _reservaServicioRepository.GetReservaServicioConDetallesAsync(id);
        if (reservaServicio == null)
        {
            _logger.LogWarning("⚠️ Reserva de servicio {Id} no encontrada", id);
            throw new KeyNotFoundException($"La reserva de servicio con ID {id} no existe");
        }

        // Guardar datos para actualizar la reserva
        var idReserva = reservaServicio.IdReserva;
        var subtotal = reservaServicio.Subtotal;

        // Eliminar la reserva de servicio
        await _reservaServicioRepository.DeleteAsync(id);
        await _reservaServicioRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Servicio eliminado de la reserva");

        // Actualizar el MontoTotal de la reserva
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva != null)
        {
            reserva.MontoTotal -= subtotal;
            reserva.SaldoPendiente = reserva.MontoTotal - reserva.MontoPagado;
            reserva.FechaModificacion = DateTime.Now;
            await _reservaRepository.SaveChangesAsync();

            _logger.LogInformation("💰 MontoTotal actualizado: {MontoTotal:C}", reserva.MontoTotal);
        }

        return true;
    }

    /// <summary>
    /// Obtiene servicios de una reserva filtrados por estado
    /// </summary>
    public async Task<IEnumerable<ReservaServicioResponseDto>> GetServiciosPorReservaYEstadoAsync(int idReserva, string estado)
    {
        _logger.LogInformation("🔍 Obteniendo servicios de reserva {IdReserva} con estado {Estado}", idReserva, estado);

        // Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        var servicios = await _reservaServicioRepository.GetServiciosByReservaYEstadoAsync(idReserva, estado);
        var serviciosDto = _mapper.Map<IEnumerable<ReservaServicioResponseDto>>(servicios);

        _logger.LogInformation("✅ Se encontraron {Count} servicios con estado '{Estado}'", serviciosDto.Count(), estado);

        return serviciosDto;
    }
}
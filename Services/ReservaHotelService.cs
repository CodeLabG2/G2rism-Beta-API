using AutoMapper;
using G2rismBeta.API.DTOs.ReservaHotel;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio de gestión de relaciones Reserva-Hotel con lógica de negocio
/// </summary>
public class ReservaHotelService : IReservaHotelService
{
    private readonly IReservaHotelRepository _reservaHotelRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservaHotelService> _logger;

    public ReservaHotelService(
        IReservaHotelRepository reservaHotelRepository,
        IReservaRepository reservaRepository,
        IHotelRepository hotelRepository,
        IMapper mapper,
        ILogger<ReservaHotelService> logger)
    {
        _reservaHotelRepository = reservaHotelRepository;
        _reservaRepository = reservaRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ReservaHotelResponseDto> AgregarHotelAReservaAsync(int idReserva, ReservaHotelCreateDto dto)
    {
        _logger.LogInformation("📝 Agregando hotel {IdHotel} a la reserva {IdReserva}", dto.IdHotel, idReserva);

        // 1. Validar que la reserva existe
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que el hotel existe y está activo
        var hotel = await _hotelRepository.GetByIdAsync(dto.IdHotel);
        if (hotel == null)
        {
            _logger.LogWarning("⚠️ Hotel {IdHotel} no encontrado", dto.IdHotel);
            throw new KeyNotFoundException($"El hotel con ID {dto.IdHotel} no existe");
        }

        if (!hotel.Estado)
        {
            throw new InvalidOperationException($"El hotel '{hotel.Nombre}' no está activo");
        }

        // 3. Validar que las fechas estén dentro del rango de la reserva
        if (dto.FechaCheckin < reserva.FechaInicioViaje || dto.FechaCheckout > reserva.FechaFinViaje)
        {
            throw new ArgumentException(
                $"Las fechas del hotel deben estar dentro del rango del viaje " +
                $"({reserva.FechaInicioViaje:yyyy-MM-dd} - {reserva.FechaFinViaje:yyyy-MM-dd})");
        }

        // 4. Verificar que el hotel no esté ya agregado a esta reserva
        var yaExiste = await _reservaHotelRepository.ExisteHotelEnReservaAsync(idReserva, dto.IdHotel);
        if (yaExiste)
        {
            throw new InvalidOperationException(
                $"El hotel '{hotel.Nombre}' ya está agregado a esta reserva. " +
                "Si desea modificar las fechas o habitaciones, elimine la entrada actual y agregue una nueva.");
        }

        // 5. Crear la entidad ReservaHotel
        var reservaHotel = new ReservaHotel
        {
            IdReserva = idReserva,
            IdHotel = dto.IdHotel,
            FechaCheckin = dto.FechaCheckin,
            FechaCheckout = dto.FechaCheckout,
            NumeroHabitaciones = dto.NumeroHabitaciones,
            TipoHabitacion = dto.TipoHabitacion,
            NumeroHuespedes = dto.NumeroHuespedes,
            PrecioPorNoche = hotel.PrecioPorNoche, // Usar precio actual del hotel
            Observaciones = dto.Observaciones
        };

        // 6. Calcular subtotal automáticamente
        var numeroNoches = (dto.FechaCheckout - dto.FechaCheckin).Days;
        reservaHotel.Subtotal = numeroNoches * hotel.PrecioPorNoche * dto.NumeroHabitaciones;

        _logger.LogInformation("💰 Subtotal calculado: {Subtotal} ({Noches} noches x ${PrecioPorNoche} x {Habitaciones} habitaciones)",
            reservaHotel.Subtotal, numeroNoches, hotel.PrecioPorNoche, dto.NumeroHabitaciones);

        // 7. Guardar en la base de datos
        await _reservaHotelRepository.AddAsync(reservaHotel);
        await _reservaHotelRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Hotel agregado exitosamente a la reserva. ID de relación: {Id}", reservaHotel.Id);

        // 8. Actualizar el monto total de la reserva
        await ActualizarMontoTotalReservaAsync(idReserva);

        // 9. Recargar la entidad con datos completos para la respuesta
        var reservaHotelCompleta = await _reservaHotelRepository.GetByIdConHotelAsync(reservaHotel.Id);

        return MapearAResponseDto(reservaHotelCompleta!);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReservaHotelResponseDto>> ObtenerHotelesPorReservaAsync(int idReserva)
    {
        _logger.LogInformation("🔍 Obteniendo hoteles de la reserva {IdReserva}", idReserva);

        var reservasHoteles = await _reservaHotelRepository.GetHotelesByReservaAsync(idReserva);

        var resultado = reservasHoteles.Select(MapearAResponseDto).ToList();

        _logger.LogInformation("✅ Se encontraron {Count} hoteles en la reserva", resultado.Count);

        return resultado;
    }

    /// <inheritdoc/>
    public async Task<ReservaHotelResponseDto> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("🔍 Obteniendo información de ReservaHotel {Id}", id);

        var reservaHotel = await _reservaHotelRepository.GetByIdConHotelAsync(id);

        if (reservaHotel == null)
        {
            _logger.LogWarning("⚠️ ReservaHotel {Id} no encontrada", id);
            throw new KeyNotFoundException($"No se encontró la relación ReservaHotel con ID {id}");
        }

        return MapearAResponseDto(reservaHotel);
    }

    /// <inheritdoc/>
    public async Task<bool> EliminarHotelDeReservaAsync(int id)
    {
        _logger.LogInformation("🗑️ Eliminando hotel de reserva. ID de relación: {Id}", id);

        var reservaHotel = await _reservaHotelRepository.GetByIdAsync(id);

        if (reservaHotel == null)
        {
            _logger.LogWarning("⚠️ ReservaHotel {Id} no encontrada", id);
            throw new KeyNotFoundException($"No se encontró la relación ReservaHotel con ID {id}");
        }

        var idReserva = reservaHotel.IdReserva;

        // Eliminar la relación
        await _reservaHotelRepository.DeleteAsync(id);
        await _reservaHotelRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Hotel eliminado de la reserva exitosamente");

        // Actualizar el monto total de la reserva
        await ActualizarMontoTotalReservaAsync(idReserva);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> VerificarDisponibilidadAsync(int idHotel, DateTime fechaCheckin, DateTime fechaCheckout)
    {
        _logger.LogInformation("🔍 Verificando disponibilidad del hotel {IdHotel} para {FechaCheckin} - {FechaCheckout}",
            idHotel, fechaCheckin.ToShortDateString(), fechaCheckout.ToShortDateString());

        // Obtener el hotel
        var hotel = await _hotelRepository.GetByIdAsync(idHotel);

        if (hotel == null)
        {
            throw new KeyNotFoundException($"El hotel con ID {idHotel} no existe");
        }

        if (!hotel.Estado)
        {
            return false; // Hotel inactivo = no disponible
        }

        // Obtener reservas existentes en ese rango de fechas
        var reservasExistentes = await _reservaHotelRepository
            .GetReservasPorHotelYFechasAsync(idHotel, fechaCheckin, fechaCheckout);

        var totalHabitacionesReservadas = reservasExistentes.Sum(r => r.NumeroHabitaciones);

        // Si el hotel no tiene número de habitaciones configurado, asumimos disponibilidad
        if (!hotel.NumeroHabitaciones.HasValue || hotel.NumeroHabitaciones.Value == 0)
        {
            _logger.LogInformation("✅ Hotel sin límite de habitaciones configurado. Disponible.");
            return true;
        }

        var disponible = totalHabitacionesReservadas < hotel.NumeroHabitaciones.Value;

        _logger.LogInformation(disponible
            ? "✅ Hotel disponible. Habitaciones reservadas: {Reservadas}/{Total}"
            : "❌ Hotel NO disponible. Habitaciones reservadas: {Reservadas}/{Total}",
            totalHabitacionesReservadas, hotel.NumeroHabitaciones.Value);

        return disponible;
    }

    #region Métodos Privados

    /// <summary>
    /// Recalcula y actualiza el monto total de una reserva sumando todos sus servicios
    /// </summary>
    private async Task ActualizarMontoTotalReservaAsync(int idReserva)
    {
        _logger.LogInformation("💰 Actualizando monto total de la reserva {IdReserva}", idReserva);

        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ No se pudo actualizar el monto. Reserva {IdReserva} no encontrada", idReserva);
            return;
        }

        // Calcular total de hoteles
        var totalHoteles = await _reservaHotelRepository.CalcularTotalHotelesPorReservaAsync(idReserva);

        // TODO: Cuando se implementen vuelos, paquetes y servicios, sumar aquí
        var nuevoMontoTotal = totalHoteles;

        reserva.MontoTotal = nuevoMontoTotal;
        reserva.SaldoPendiente = nuevoMontoTotal - reserva.MontoPagado;
        reserva.FechaModificacion = DateTime.Now;

        await _reservaRepository.UpdateAsync(reserva);
        await _reservaRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Monto total actualizado: ${MontoTotal}. Saldo pendiente: ${SaldoPendiente}",
            reserva.MontoTotal, reserva.SaldoPendiente);
    }

    /// <summary>
    /// Mapea una entidad ReservaHotel a un DTO de respuesta
    /// </summary>
    private ReservaHotelResponseDto MapearAResponseDto(ReservaHotel reservaHotel)
    {
        return new ReservaHotelResponseDto
        {
            Id = reservaHotel.Id,
            IdReserva = reservaHotel.IdReserva,
            IdHotel = reservaHotel.IdHotel,
            NombreHotel = reservaHotel.Hotel?.Nombre ?? "N/A",
            CiudadHotel = reservaHotel.Hotel?.Ciudad ?? "N/A",
            FechaCheckin = reservaHotel.FechaCheckin,
            FechaCheckout = reservaHotel.FechaCheckout,
            NumeroHabitaciones = reservaHotel.NumeroHabitaciones,
            TipoHabitacion = reservaHotel.TipoHabitacion,
            NumeroHuespedes = reservaHotel.NumeroHuespedes,
            PrecioPorNoche = reservaHotel.PrecioPorNoche,
            Subtotal = reservaHotel.Subtotal,
            Observaciones = reservaHotel.Observaciones,
            // Propiedades computadas
            NumeroNoches = reservaHotel.NumeroNoches,
            CostoPorHabitacion = reservaHotel.CostoPorHabitacion,
            EstadiaActiva = reservaHotel.EstadiaActiva,
            DiasHastaCheckin = reservaHotel.DiasHastaCheckin
        };
    }

    #endregion
}
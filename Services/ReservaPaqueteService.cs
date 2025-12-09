using AutoMapper;
using G2rismBeta.API.DTOs.ReservaPaquete;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio para la gestión de paquetes turísticos en reservas
/// Maneja la lógica de negocio para agregar, consultar y eliminar paquetes de reservas
/// </summary>
public class ReservaPaqueteService : IReservaPaqueteService
{
    private readonly IReservaPaqueteRepository _reservaPaqueteRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IPaqueteTuristicoRepository _paqueteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservaPaqueteService> _logger;

    public ReservaPaqueteService(
        IReservaPaqueteRepository reservaPaqueteRepository,
        IReservaRepository reservaRepository,
        IPaqueteTuristicoRepository paqueteRepository,
        IMapper mapper,
        ILogger<ReservaPaqueteService> logger)
    {
        _reservaPaqueteRepository = reservaPaqueteRepository;
        _reservaRepository = reservaRepository;
        _paqueteRepository = paqueteRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Agrega un paquete turístico a una reserva existente
    /// </summary>
    public async Task<ReservaPaqueteResponseDto> AgregarPaqueteAReservaAsync(int idReserva, ReservaPaqueteCreateDto createDto)
    {
        _logger.LogInformation("📦 Agregando paquete {IdPaquete} a reserva {IdReserva}", createDto.IdPaquete, idReserva);

        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Validar que el paquete exista
        var paquete = await _paqueteRepository.GetByIdAsync(createDto.IdPaquete);
        if (paquete == null)
        {
            _logger.LogWarning("⚠️ Paquete {IdPaquete} no encontrado", createDto.IdPaquete);
            throw new KeyNotFoundException($"El paquete con ID {createDto.IdPaquete} no existe");
        }

        // 3. Validar que el paquete esté activo
        if (!paquete.Estado)
        {
            _logger.LogWarning("⚠️ Paquete {IdPaquete} no está activo", createDto.IdPaquete);
            throw new InvalidOperationException($"El paquete '{paquete.Nombre}' no está activo");
        }

        // 4. Validar cupos disponibles
        if (paquete.CuposDisponibles < createDto.NumeroPersonas)
        {
            _logger.LogWarning("⚠️ Cupos insuficientes. Disponibles: {Disponibles}, Solicitados: {Solicitados}",
                paquete.CuposDisponibles, createDto.NumeroPersonas);
            throw new InvalidOperationException(
                $"Cupos insuficientes. Disponibles: {paquete.CuposDisponibles}, Solicitados: {createDto.NumeroPersonas}");
        }

        // 5. Validar que el número de personas no exceda el máximo del paquete (si está definido)
        if (paquete.NumeroMaximoPersonas.HasValue && createDto.NumeroPersonas > paquete.NumeroMaximoPersonas.Value)
        {
            _logger.LogWarning("⚠️ Número de personas ({Solicitado}) excede el máximo permitido ({Maximo})",
                createDto.NumeroPersonas, paquete.NumeroMaximoPersonas.Value);
            throw new InvalidOperationException(
                $"El número de personas ({createDto.NumeroPersonas}) excede el máximo permitido ({paquete.NumeroMaximoPersonas.Value})");
        }

        // 6. Validar que el número de personas cumpla el mínimo del paquete (si está definido)
        if (paquete.NumeroMinimoPersonas.HasValue && createDto.NumeroPersonas < paquete.NumeroMinimoPersonas.Value)
        {
            _logger.LogWarning("⚠️ Número de personas ({Solicitado}) es menor al mínimo requerido ({Minimo})",
                createDto.NumeroPersonas, paquete.NumeroMinimoPersonas.Value);
            throw new InvalidOperationException(
                $"El número de personas ({createDto.NumeroPersonas}) es menor al mínimo requerido ({paquete.NumeroMinimoPersonas.Value})");
        }

        // 7. Validar que el paquete no esté vencido (si tiene fechas definidas)
        if (paquete.FechaFin.HasValue && paquete.FechaFin.Value.Date < DateTime.Today)
        {
            _logger.LogWarning("⚠️ Paquete {IdPaquete} ya venció", createDto.IdPaquete);
            throw new InvalidOperationException($"El paquete '{paquete.Nombre}' ya ha vencido");
        }

        // 8. Calcular fecha de fin del paquete (si no se proporciona)
        DateTime fechaFinPaquete;
        if (createDto.FechaFinPaquete.HasValue)
        {
            fechaFinPaquete = createDto.FechaFinPaquete.Value;
        }
        else
        {
            // Calcular automáticamente según la duración del paquete
            fechaFinPaquete = createDto.FechaInicioPaquete.AddDays(paquete.Duracion);
        }

        // 9. Validar que las fechas del paquete estén dentro del rango de la reserva
        if (createDto.FechaInicioPaquete.Date < reserva.FechaInicioViaje.Date)
        {
            _logger.LogWarning("⚠️ Fecha de inicio del paquete ({FechaPaquete}) es anterior al inicio de la reserva ({FechaReserva})",
                createDto.FechaInicioPaquete, reserva.FechaInicioViaje);
            throw new InvalidOperationException(
                "La fecha de inicio del paquete no puede ser anterior al inicio de la reserva");
        }

        if (fechaFinPaquete.Date > reserva.FechaFinViaje.Date)
        {
            _logger.LogWarning("⚠️ Fecha de fin del paquete ({FechaPaquete}) es posterior al fin de la reserva ({FechaReserva})",
                fechaFinPaquete, reserva.FechaFinViaje);
            throw new InvalidOperationException(
                "La fecha de fin del paquete no puede ser posterior al fin de la reserva");
        }

        // 10. Validar que no se haya agregado ya este paquete a la reserva
        var yaExiste = await _reservaPaqueteRepository.ExistsReservaPaqueteAsync(idReserva, createDto.IdPaquete);
        if (yaExiste)
        {
            _logger.LogWarning("⚠️ El paquete {IdPaquete} ya está agregado a la reserva {IdReserva}",
                createDto.IdPaquete, idReserva);
            throw new InvalidOperationException(
                $"El paquete '{paquete.Nombre}' ya está agregado a esta reserva");
        }

        // 11. Calcular el subtotal
        decimal precioPorPersona = paquete.Precio;
        decimal subtotal = precioPorPersona * createDto.NumeroPersonas;

        // 12. Crear la entidad ReservaPaquete
        var reservaPaquete = new ReservaPaquete
        {
            IdReserva = idReserva,
            IdPaquete = createDto.IdPaquete,
            NumeroPersonas = createDto.NumeroPersonas,
            FechaInicioPaquete = createDto.FechaInicioPaquete,
            FechaFinPaquete = fechaFinPaquete,
            PrecioPorPersona = precioPorPersona,
            Subtotal = subtotal,
            Personalizaciones = createDto.Personalizaciones,
            Observaciones = createDto.Observaciones,
            FechaAgregado = DateTime.Now
        };

        // 13. Guardar la reserva de paquete
        await _reservaPaqueteRepository.AddAsync(reservaPaquete);
        await _reservaPaqueteRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Paquete agregado a la reserva. Subtotal: {Subtotal:C}", subtotal);

        // 14. Descontar cupos del paquete
        paquete.CuposDisponibles -= createDto.NumeroPersonas;
        paquete.FechaModificacion = DateTime.Now;
        await _paqueteRepository.SaveChangesAsync();

        _logger.LogInformation("📊 Cupos actualizados. Restantes: {CuposDisponibles}", paquete.CuposDisponibles);

        // 15. Actualizar el MontoTotal de la reserva
        reserva.MontoTotal += subtotal;
        reserva.SaldoPendiente = reserva.MontoTotal - reserva.MontoPagado;
        reserva.FechaModificacion = DateTime.Now;
        await _reservaRepository.SaveChangesAsync();

        _logger.LogInformation("💰 MontoTotal actualizado: {MontoTotal:C}", reserva.MontoTotal);

        // 16. Obtener la reserva de paquete completa para la respuesta
        var reservaPaqueteCompleta = await _reservaPaqueteRepository.GetByIdWithDetailsAsync(reservaPaquete.Id);

        // 17. Mapear a DTO de respuesta
        var response = _mapper.Map<ReservaPaqueteResponseDto>(reservaPaqueteCompleta);

        return response;
    }

    /// <summary>
    /// Obtiene todos los paquetes de una reserva
    /// </summary>
    public async Task<IEnumerable<ReservaPaqueteResponseDto>> ObtenerPaquetesPorReservaAsync(int idReserva)
    {
        _logger.LogInformation("🔍 Obteniendo paquetes de la reserva {IdReserva}", idReserva);

        // Validar que la reserva exista
        var reservaExiste = await _reservaRepository.ExisteReservaAsync(idReserva);
        if (!reservaExiste)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        var reservasPaquetes = await _reservaPaqueteRepository.GetByReservaIdWithDetailsAsync(idReserva);
        var response = _mapper.Map<IEnumerable<ReservaPaqueteResponseDto>>(reservasPaquetes);

        _logger.LogInformation("✅ Se encontraron {Count} paquetes en la reserva", response.Count());

        return response;
    }

    /// <summary>
    /// Obtiene un paquete específico de una reserva
    /// </summary>
    public async Task<ReservaPaqueteResponseDto> ObtenerPaquetePorIdAsync(int id)
    {
        _logger.LogInformation("🔍 Obteniendo detalles del paquete en reserva ID: {Id}", id);

        var reservaPaquete = await _reservaPaqueteRepository.GetByIdWithDetailsAsync(id);
        if (reservaPaquete == null)
        {
            _logger.LogWarning("⚠️ Paquete en reserva {Id} no encontrado", id);
            throw new KeyNotFoundException($"El paquete en reserva con ID {id} no existe");
        }

        var response = _mapper.Map<ReservaPaqueteResponseDto>(reservaPaquete);

        _logger.LogInformation("✅ Paquete encontrado: {Nombre}", response.NombrePaquete);

        return response;
    }

    /// <summary>
    /// Elimina un paquete de una reserva
    /// </summary>
    public async Task<bool> EliminarPaqueteDeReservaAsync(int idReserva, int idReservaPaquete)
    {
        _logger.LogInformation("🗑️ Eliminando paquete {IdReservaPaquete} de la reserva {IdReserva}",
            idReservaPaquete, idReserva);

        // 1. Validar que la reserva exista
        var reserva = await _reservaRepository.GetByIdAsync(idReserva);
        if (reserva == null)
        {
            _logger.LogWarning("⚠️ Reserva {IdReserva} no encontrada", idReserva);
            throw new KeyNotFoundException($"La reserva con ID {idReserva} no existe");
        }

        // 2. Obtener el paquete en la reserva
        var reservaPaquete = await _reservaPaqueteRepository.GetByIdWithDetailsAsync(idReservaPaquete);
        if (reservaPaquete == null)
        {
            _logger.LogWarning("⚠️ Paquete en reserva {Id} no encontrado", idReservaPaquete);
            throw new KeyNotFoundException($"El paquete en reserva con ID {idReservaPaquete} no existe");
        }

        // 3. Validar que el paquete pertenezca a la reserva especificada
        if (reservaPaquete.IdReserva != idReserva)
        {
            _logger.LogWarning("⚠️ El paquete {IdReservaPaquete} no pertenece a la reserva {IdReserva}",
                idReservaPaquete, idReserva);
            throw new InvalidOperationException(
                $"El paquete especificado no pertenece a la reserva {idReserva}");
        }

        // 4. Validar que el paquete no haya iniciado
        if (reservaPaquete.PaqueteIniciado)
        {
            _logger.LogWarning("⚠️ No se puede eliminar el paquete porque ya ha iniciado");
            throw new InvalidOperationException(
                "No se puede eliminar un paquete que ya ha iniciado");
        }

        // 5. Obtener el paquete para restaurar cupos
        var paquete = reservaPaquete.Paquete;
        if (paquete == null)
        {
            paquete = await _paqueteRepository.GetByIdAsync(reservaPaquete.IdPaquete);
        }

        // 6. Guardar el subtotal antes de eliminar
        decimal subtotalEliminado = reservaPaquete.Subtotal;

        // 7. Eliminar el paquete de la reserva
        await _reservaPaqueteRepository.DeleteAsync(idReservaPaquete);
        await _reservaPaqueteRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Paquete eliminado de la reserva");

        // 8. Restaurar cupos del paquete (si el paquete aún existe)
        if (paquete != null)
        {
            paquete.CuposDisponibles += reservaPaquete.NumeroPersonas;
            paquete.FechaModificacion = DateTime.Now;
            await _paqueteRepository.SaveChangesAsync();

            _logger.LogInformation("📊 Cupos restaurados. Disponibles: {CuposDisponibles}", paquete.CuposDisponibles);
        }

        // 9. Actualizar el MontoTotal de la reserva
        reserva.MontoTotal -= subtotalEliminado;
        reserva.SaldoPendiente = reserva.MontoTotal - reserva.MontoPagado;
        reserva.FechaModificacion = DateTime.Now;
        await _reservaRepository.SaveChangesAsync();

        _logger.LogInformation("💰 MontoTotal actualizado: {MontoTotal:C}", reserva.MontoTotal);

        return true;
    }
}
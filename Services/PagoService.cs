using AutoMapper;
using G2rismBeta.API.DTOs.Pago;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio para la gestión de Pagos.
/// Implementa la lógica de negocio para registro y control de pagos.
/// </summary>
public class PagoService : IPagoService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly IFacturaRepository _facturaRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IFormaDePagoRepository _formaDePagoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PagoService> _logger;

    public PagoService(
        IPagoRepository pagoRepository,
        IFacturaRepository facturaRepository,
        IReservaRepository reservaRepository,
        IFormaDePagoRepository formaDePagoRepository,
        IMapper mapper,
        ILogger<PagoService> logger)
    {
        _pagoRepository = pagoRepository;
        _facturaRepository = facturaRepository;
        _reservaRepository = reservaRepository;
        _formaDePagoRepository = formaDePagoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PagoResponseDto>> GetAllPagosAsync()
    {
        _logger.LogInformation("Obteniendo todos los pagos con detalles");
        var pagos = await _pagoRepository.GetAllPagosConDetallesAsync();
        return _mapper.Map<IEnumerable<PagoResponseDto>>(pagos);
    }

    /// <inheritdoc/>
    public async Task<PagoResponseDto> GetPagoByIdAsync(int id)
    {
        _logger.LogInformation($"Obteniendo pago con ID: {id}");

        var pago = await _pagoRepository.GetPagoConDetallesAsync(id);
        if (pago == null)
        {
            throw new KeyNotFoundException($"No se encontró el pago con ID {id}");
        }

        return _mapper.Map<PagoResponseDto>(pago);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PagoResponseDto>> GetPagosPorFacturaAsync(int idFactura)
    {
        _logger.LogInformation($"Obteniendo pagos para la factura ID: {idFactura}");

        // Verificar que la factura existe
        var factura = await _facturaRepository.GetByIdAsync(idFactura);
        if (factura == null)
        {
            throw new KeyNotFoundException($"No se encontró la factura con ID {idFactura}");
        }

        var pagos = await _pagoRepository.GetPagosPorFacturaAsync(idFactura);
        return _mapper.Map<IEnumerable<PagoResponseDto>>(pagos);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PagoResponseDto>> GetPagosPorEstadoAsync(string estado)
    {
        _logger.LogInformation($"Obteniendo pagos con estado: {estado}");

        // Validar estado
        var estadosValidos = new[] { "pendiente", "aprobado", "rechazado" };
        if (!estadosValidos.Contains(estado.ToLower()))
        {
            throw new ArgumentException($"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}");
        }

        var pagos = await _pagoRepository.GetPagosPorEstadoAsync(estado.ToLower());
        return _mapper.Map<IEnumerable<PagoResponseDto>>(pagos);
    }

    /// <inheritdoc/>
    public async Task<PagoResponseDto> CreatePagoAsync(PagoCreateDto createDto)
    {
        _logger.LogInformation($"Creando nuevo pago para factura ID: {createDto.IdFactura}");

        // Validar que la factura existe
        var factura = await _facturaRepository.GetByIdAsync(createDto.IdFactura);
        if (factura == null)
        {
            throw new KeyNotFoundException($"No se encontró la factura con ID {createDto.IdFactura}");
        }

        // Validar que la forma de pago existe y está activa
        var formaDePago = await _formaDePagoRepository.GetByIdAsync(createDto.IdFormaPago);
        if (formaDePago == null)
        {
            throw new KeyNotFoundException($"No se encontró la forma de pago con ID {createDto.IdFormaPago}");
        }

        if (!formaDePago.Activo)
        {
            throw new InvalidOperationException($"La forma de pago '{formaDePago.Metodo}' no está activa");
        }

        // Calcular el saldo pendiente de la factura
        var totalPagadoActual = await _pagoRepository.GetTotalPagadoPorFacturaAsync(createDto.IdFactura);
        var saldoPendiente = factura.Total - totalPagadoActual;

        // Validar que el monto del pago no exceda el saldo pendiente
        if (createDto.Monto > saldoPendiente)
        {
            throw new InvalidOperationException(
                $"El monto del pago (${createDto.Monto:N2}) excede el saldo pendiente de la factura (${saldoPendiente:N2})");
        }

        // Verificar que la referencia de transacción no esté duplicada
        if (!string.IsNullOrWhiteSpace(createDto.ReferenciaTransaccion))
        {
            var existeReferencia = await _pagoRepository.ExistePorReferenciaAsync(createDto.ReferenciaTransaccion);
            if (existeReferencia)
            {
                throw new InvalidOperationException(
                    $"Ya existe un pago con la referencia de transacción '{createDto.ReferenciaTransaccion}'");
            }
        }

        // Crear el pago
        var pago = _mapper.Map<Pago>(createDto);
        pago.FechaPago = createDto.FechaPago ?? DateTime.Now;
        pago.Estado = string.IsNullOrWhiteSpace(createDto.Estado) ? "pendiente" : createDto.Estado.ToLower();
        pago.FechaCreacion = DateTime.Now;

        await _pagoRepository.AddAsync(pago);
        await _pagoRepository.SaveChangesAsync();

        _logger.LogInformation($"Pago creado exitosamente con ID: {pago.IdPago}");

        // Si el pago está aprobado, actualizar los montos de factura y reserva
        if (pago.Estado == "aprobado")
        {
            await ActualizarMontosFacturaYReservaAsync(factura, pago.Monto, sumar: true);
        }

        // Obtener el pago con detalles para retornar
        var pagoCreado = await _pagoRepository.GetPagoConDetallesAsync(pago.IdPago);
        return _mapper.Map<PagoResponseDto>(pagoCreado);
    }

    /// <inheritdoc/>
    public async Task<PagoResponseDto> UpdatePagoAsync(int id, PagoUpdateDto updateDto)
    {
        _logger.LogInformation($"Actualizando pago ID: {id}");

        var pago = await _pagoRepository.GetByIdAsync(id);
        if (pago == null)
        {
            throw new KeyNotFoundException($"No se encontró el pago con ID {id}");
        }

        // Guardar el monto y estado anterior para recalcular si cambian
        var montoAnterior = pago.Monto;
        var estadoAnterior = pago.Estado;

        // Actualizar campos individualmente solo si no son null (actualizaciones parciales)
        // IMPORTANTE: No usar AutoMapper aquí porque sobrescribe campos no enviados con valores por defecto

        if (updateDto.Monto.HasValue)
            pago.Monto = updateDto.Monto.Value;

        if (!string.IsNullOrWhiteSpace(updateDto.ReferenciaTransaccion))
        {
            // Verificar que la referencia no exista
            var existeReferencia = await _pagoRepository.ExistePorReferenciaAsync(updateDto.ReferenciaTransaccion);
            if (existeReferencia)
            {
                throw new InvalidOperationException(
                    $"Ya existe un pago con la referencia de transacción '{updateDto.ReferenciaTransaccion}'");
            }
            pago.ReferenciaTransaccion = updateDto.ReferenciaTransaccion;
        }

        if (updateDto.ComprobantePago != null)
            pago.ComprobantePago = updateDto.ComprobantePago;

        if (!string.IsNullOrWhiteSpace(updateDto.Estado))
            pago.Estado = updateDto.Estado.ToLower();

        if (updateDto.Observaciones != null)
            pago.Observaciones = updateDto.Observaciones;

        pago.FechaModificacion = DateTime.Now;

        await _pagoRepository.UpdateAsync(pago);
        await _pagoRepository.SaveChangesAsync();

        _logger.LogInformation($"Pago ID {id} actualizado exitosamente");

        // Recalcular montos si el monto o estado cambió
        if ((updateDto.Monto.HasValue && updateDto.Monto.Value != montoAnterior) ||
            (!string.IsNullOrWhiteSpace(updateDto.Estado) && updateDto.Estado.ToLower() != estadoAnterior))
        {
            var factura = await _facturaRepository.GetByIdAsync(pago.IdFactura);
            if (factura != null)
            {
                await RecalcularMontosFacturaYReservaAsync(factura);
            }
        }

        var pagoActualizado = await _pagoRepository.GetPagoConDetallesAsync(id);
        return _mapper.Map<PagoResponseDto>(pagoActualizado);
    }

    /// <inheritdoc/>
    public async Task<PagoResponseDto> CambiarEstadoPagoAsync(int id, string nuevoEstado)
    {
        _logger.LogInformation($"Cambiando estado del pago ID {id} a: {nuevoEstado}");

        var pago = await _pagoRepository.GetByIdAsync(id);
        if (pago == null)
        {
            throw new KeyNotFoundException($"No se encontró el pago con ID {id}");
        }

        // Validar el nuevo estado
        var estadosValidos = new[] { "pendiente", "aprobado", "rechazado" };
        nuevoEstado = nuevoEstado.ToLower();
        if (!estadosValidos.Contains(nuevoEstado))
        {
            throw new ArgumentException($"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}");
        }

        var estadoAnterior = pago.Estado;
        pago.Estado = nuevoEstado;
        pago.FechaModificacion = DateTime.Now;

        await _pagoRepository.UpdateAsync(pago);
        await _pagoRepository.SaveChangesAsync();

        _logger.LogInformation($"Estado del pago ID {id} cambiado de '{estadoAnterior}' a '{nuevoEstado}'");

        // Recalcular montos de factura y reserva
        var factura = await _facturaRepository.GetByIdAsync(pago.IdFactura);
        if (factura != null)
        {
            await RecalcularMontosFacturaYReservaAsync(factura);
        }

        var pagoActualizado = await _pagoRepository.GetPagoConDetallesAsync(id);
        return _mapper.Map<PagoResponseDto>(pagoActualizado);
    }

    /// <inheritdoc/>
    public async Task DeletePagoAsync(int id)
    {
        _logger.LogInformation($"Eliminando pago ID: {id}");

        var pago = await _pagoRepository.GetByIdAsync(id);
        if (pago == null)
        {
            throw new KeyNotFoundException($"No se encontró el pago con ID {id}");
        }

        // Solo permitir eliminar pagos pendientes
        if (pago.Estado != "pendiente")
        {
            throw new InvalidOperationException(
                $"No se puede eliminar un pago con estado '{pago.Estado}'. Solo se pueden eliminar pagos pendientes.");
        }

        await _pagoRepository.DeleteAsync(id);
        await _pagoRepository.SaveChangesAsync();

        _logger.LogInformation($"Pago ID {id} eliminado exitosamente");
    }

    #region Private Helper Methods

    /// <summary>
    /// Actualiza los montos de una factura y su reserva asociada
    /// </summary>
    private async Task ActualizarMontosFacturaYReservaAsync(Factura factura, decimal monto, bool sumar)
    {
        _logger.LogInformation($"Actualizando montos de factura ID {factura.IdFactura} y su reserva");

        // Obtener la reserva asociada
        var reserva = await _reservaRepository.GetByIdAsync(factura.IdReserva);
        if (reserva == null)
        {
            _logger.LogWarning($"No se encontró la reserva ID {factura.IdReserva} asociada a la factura");
            return;
        }

        // Actualizar monto pagado de la reserva
        if (sumar)
        {
            reserva.MontoPagado += monto;
        }
        else
        {
            reserva.MontoPagado -= monto;
        }

        // Calcular saldo pendiente
        reserva.SaldoPendiente = reserva.MontoTotal - reserva.MontoPagado;
        reserva.FechaModificacion = DateTime.Now;

        // Actualizar estado de la factura según el monto pagado
        var totalPagado = await _pagoRepository.GetTotalPagadoPorFacturaAsync(factura.IdFactura);

        if (totalPagado >= factura.Total)
        {
            factura.Estado = "pagada";
        }
        else if (totalPagado > 0)
        {
            factura.Estado = "pendiente"; // Pago parcial
        }

        await _reservaRepository.UpdateAsync(reserva);
        await _facturaRepository.UpdateAsync(factura);
        await _reservaRepository.SaveChangesAsync();
        await _facturaRepository.SaveChangesAsync();

        _logger.LogInformation(
            $"Montos actualizados - Reserva ID {reserva.IdReserva}: " +
            $"Total: ${reserva.MontoTotal:N2}, Pagado: ${reserva.MontoPagado:N2}, " +
            $"Saldo: ${reserva.SaldoPendiente:N2}");
    }

    /// <summary>
    /// Recalcula los montos totales de una factura y su reserva basándose en todos los pagos aprobados
    /// </summary>
    private async Task RecalcularMontosFacturaYReservaAsync(Factura factura)
    {
        _logger.LogInformation($"Recalculando montos de factura ID {factura.IdFactura}");

        // Calcular el total pagado (solo pagos aprobados)
        var totalPagado = await _pagoRepository.GetTotalPagadoPorFacturaAsync(factura.IdFactura);

        // Obtener la reserva
        var reserva = await _reservaRepository.GetByIdAsync(factura.IdReserva);
        if (reserva != null)
        {
            reserva.MontoPagado = totalPagado;
            reserva.SaldoPendiente = reserva.MontoTotal - totalPagado;
            reserva.FechaModificacion = DateTime.Now;

            await _reservaRepository.UpdateAsync(reserva);
            await _reservaRepository.SaveChangesAsync();
        }

        // Actualizar estado de la factura
        if (totalPagado >= factura.Total)
        {
            factura.Estado = "pagada";
        }
        else if (totalPagado > 0)
        {
            factura.Estado = "pendiente";
        }
        else
        {
            factura.Estado = "pendiente";
        }

        await _facturaRepository.UpdateAsync(factura);
        await _facturaRepository.SaveChangesAsync();

        _logger.LogInformation($"Recálculo completado - Total pagado: ${totalPagado:N2}, Estado factura: {factura.Estado}");
    }

    #endregion
}
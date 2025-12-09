namespace G2rismBeta.API.DTOs.ReservaServicio;

/// <summary>
/// DTO de respuesta para mostrar información de una relación Reserva-Servicio
/// </summary>
public class ReservaServicioResponseDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID de la reserva asociada
    /// </summary>
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del servicio adicional asociado
    /// </summary>
    public int IdServicio { get; set; }

    /// <summary>
    /// Nombre del servicio (para facilitar visualización)
    /// </summary>
    public string? NombreServicio { get; set; }

    /// <summary>
    /// Tipo de servicio (tour, guia, actividad, transporte_interno)
    /// </summary>
    public string? TipoServicio { get; set; }

    /// <summary>
    /// Unidad de medida del servicio (persona, grupo, hora, dia)
    /// </summary>
    public string? UnidadServicio { get; set; }

    /// <summary>
    /// Cantidad de unidades contratadas
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Fecha programada para el servicio
    /// </summary>
    public DateTime? FechaServicio { get; set; }

    /// <summary>
    /// Hora programada para el servicio
    /// </summary>
    public TimeSpan? HoraServicio { get; set; }

    /// <summary>
    /// Precio unitario del servicio
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Subtotal (Cantidad * PrecioUnitario)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Observaciones específicas
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Estado del servicio
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en que se agregó el servicio a la reserva
    /// </summary>
    public DateTime FechaAgregado { get; set; }

    /// <summary>
    /// Indica si el servicio está confirmado
    /// </summary>
    public bool EstaConfirmado { get; set; }

    /// <summary>
    /// Indica si el servicio está completado
    /// </summary>
    public bool EstaCompletado { get; set; }

    /// <summary>
    /// Indica si el servicio está cancelado
    /// </summary>
    public bool EstaCancelado { get; set; }

    /// <summary>
    /// Indica si el servicio ya fue ejecutado
    /// </summary>
    public bool ServicioEjecutado { get; set; }

    /// <summary>
    /// Días hasta la ejecución del servicio
    /// </summary>
    public int? DiasHastaServicio { get; set; }
}
namespace G2rismBeta.API.DTOs.ReservaPaquete;

/// <summary>
/// DTO para la respuesta de un paquete turístico en una reserva
/// </summary>
public class ReservaPaqueteResponseDto
{
    /// <summary>
    /// ID único de la relación reserva-paquete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID de la reserva
    /// </summary>
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del paquete turístico
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// Nombre del paquete turístico
    /// </summary>
    public string? NombrePaquete { get; set; }

    /// <summary>
    /// Destino principal del paquete
    /// </summary>
    public string? DestinoPrincipal { get; set; }

    /// <summary>
    /// Número de personas
    /// </summary>
    public int NumeroPersonas { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete
    /// </summary>
    public DateTime FechaInicioPaquete { get; set; }

    /// <summary>
    /// Fecha de fin del paquete
    /// </summary>
    public DateTime FechaFinPaquete { get; set; }

    /// <summary>
    /// Duración en días
    /// </summary>
    public int DuracionDias { get; set; }

    /// <summary>
    /// Precio por persona
    /// </summary>
    public decimal PrecioPorPersona { get; set; }

    /// <summary>
    /// Subtotal (NumeroPersonas * PrecioPorPersona)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Subtotal formateado
    /// </summary>
    public string? SubtotalFormateado { get; set; }

    /// <summary>
    /// Personalizaciones solicitadas
    /// </summary>
    public string? Personalizaciones { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha en que se agregó el paquete a la reserva
    /// </summary>
    public DateTime FechaAgregado { get; set; }

    /// <summary>
    /// Indica si el paquete ya inició
    /// </summary>
    public bool PaqueteIniciado { get; set; }

    /// <summary>
    /// Indica si el paquete ya finalizó
    /// </summary>
    public bool PaqueteCompletado { get; set; }

    /// <summary>
    /// Días restantes hasta el inicio
    /// </summary>
    public int DiasHastaInicio { get; set; }
}
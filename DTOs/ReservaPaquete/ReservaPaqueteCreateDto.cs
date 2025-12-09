namespace G2rismBeta.API.DTOs.ReservaPaquete;

/// <summary>
/// DTO para agregar un paquete turístico a una reserva
/// </summary>
public class ReservaPaqueteCreateDto
{
    /// <summary>
    /// ID del paquete turístico a agregar
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// Número de personas que tomarán el paquete
    /// </summary>
    public int NumeroPersonas { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete para esta reserva
    /// </summary>
    public DateTime FechaInicioPaquete { get; set; }

    /// <summary>
    /// Fecha de fin del paquete (opcional, se calcula automáticamente si no se proporciona)
    /// </summary>
    public DateTime? FechaFinPaquete { get; set; }

    /// <summary>
    /// Personalizaciones solicitadas (JSON string)
    /// Ejemplo: "{\"alimentacion\": \"vegetariana\", \"habitacion\": \"vista_mar\"}"
    /// </summary>
    public string? Personalizaciones { get; set; }

    /// <summary>
    /// Observaciones o solicitudes especiales
    /// </summary>
    public string? Observaciones { get; set; }
}
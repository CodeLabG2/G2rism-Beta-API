namespace G2rismBeta.API.DTOs.ReservaHotel;

/// <summary>
/// DTO para crear una relación entre una Reserva y un Hotel
/// </summary>
public class ReservaHotelCreateDto
{
    /// <summary>
    /// ID del hotel a agregar a la reserva
    /// </summary>
    public int IdHotel { get; set; }

    /// <summary>
    /// Fecha de check-in en el hotel
    /// </summary>
    public DateTime FechaCheckin { get; set; }

    /// <summary>
    /// Fecha de check-out del hotel
    /// </summary>
    public DateTime FechaCheckout { get; set; }

    /// <summary>
    /// Número de habitaciones a reservar
    /// </summary>
    public int NumeroHabitaciones { get; set; } = 1;

    /// <summary>
    /// Tipo de habitación (simple, doble, triple, suite, presidencial)
    /// </summary>
    public string? TipoHabitacion { get; set; }

    /// <summary>
    /// Número de huéspedes
    /// </summary>
    public int NumeroHuespedes { get; set; }

    /// <summary>
    /// Observaciones o requisitos especiales
    /// </summary>
    public string? Observaciones { get; set; }
}
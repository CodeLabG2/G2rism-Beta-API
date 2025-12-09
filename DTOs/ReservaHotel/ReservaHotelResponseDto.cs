namespace G2rismBeta.API.DTOs.ReservaHotel;

/// <summary>
/// DTO de respuesta para la relación Reserva-Hotel con información completa
/// </summary>
public class ReservaHotelResponseDto
{
    /// <summary>
    /// Identificador único de la relación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID de la reserva
    /// </summary>
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del hotel
    /// </summary>
    public int IdHotel { get; set; }

    /// <summary>
    /// Nombre del hotel
    /// </summary>
    public string NombreHotel { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad del hotel
    /// </summary>
    public string CiudadHotel { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de check-in
    /// </summary>
    public DateTime FechaCheckin { get; set; }

    /// <summary>
    /// Fecha de check-out
    /// </summary>
    public DateTime FechaCheckout { get; set; }

    /// <summary>
    /// Número de habitaciones reservadas
    /// </summary>
    public int NumeroHabitaciones { get; set; }

    /// <summary>
    /// Tipo de habitación
    /// </summary>
    public string? TipoHabitacion { get; set; }

    /// <summary>
    /// Número de huéspedes
    /// </summary>
    public int NumeroHuespedes { get; set; }

    /// <summary>
    /// Precio por noche al momento de la reserva
    /// </summary>
    public decimal PrecioPorNoche { get; set; }

    /// <summary>
    /// Subtotal del hotel en esta reserva
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    public string? Observaciones { get; set; }

    // Propiedades computadas
    /// <summary>
    /// Número de noches calculado
    /// </summary>
    public int NumeroNoches { get; set; }

    /// <summary>
    /// Costo por habitación
    /// </summary>
    public decimal CostoPorHabitacion { get; set; }

    /// <summary>
    /// Indica si la estadía está activa
    /// </summary>
    public bool EstadiaActiva { get; set; }

    /// <summary>
    /// Días restantes hasta el check-in
    /// </summary>
    public int DiasHastaCheckin { get; set; }
}

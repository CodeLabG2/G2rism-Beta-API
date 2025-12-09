using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad que representa la relación entre una Reserva y un Hotel.
/// Tabla intermedia para la relación N:M entre reservas y hoteles.
/// Una reserva puede incluir múltiples hoteles y un hotel puede estar en múltiples reservas.
/// </summary>
[Table("reservas_hoteles")]
public class ReservaHotel
{
    #region Primary Key

    /// <summary>
    /// Identificador único de la relación Reserva-Hotel
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    #endregion

    #region Foreign Keys

    /// <summary>
    /// ID de la reserva asociada
    /// </summary>
    [Required(ErrorMessage = "El ID de la reserva es obligatorio")]
    [Column("id_reserva")]
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del hotel asociado
    /// </summary>
    [Required(ErrorMessage = "El ID del hotel es obligatorio")]
    [Column("id_hotel")]
    public int IdHotel { get; set; }

    #endregion

    #region Reservation Details

    /// <summary>
    /// Fecha de check-in en el hotel
    /// </summary>
    [Required(ErrorMessage = "La fecha de check-in es obligatoria")]
    [Column("fecha_checkin", TypeName = "DATE")]
    public DateTime FechaCheckin { get; set; }

    /// <summary>
    /// Fecha de check-out del hotel
    /// </summary>
    [Required(ErrorMessage = "La fecha de check-out es obligatoria")]
    [Column("fecha_checkout", TypeName = "DATE")]
    public DateTime FechaCheckout { get; set; }

    /// <summary>
    /// Número de habitaciones reservadas
    /// </summary>
    [Required(ErrorMessage = "El número de habitaciones es obligatorio")]
    [Range(1, 50, ErrorMessage = "El número de habitaciones debe estar entre 1 y 50")]
    [Column("numero_habitaciones")]
    public int NumeroHabitaciones { get; set; }

    /// <summary>
    /// Tipo de habitación reservada
    /// Valores: simple, doble, triple, suite, presidencial
    /// </summary>
    [StringLength(50, ErrorMessage = "El tipo de habitación no puede exceder 50 caracteres")]
    [Column("tipo_habitacion")]
    public string? TipoHabitacion { get; set; }

    /// <summary>
    /// Número de huéspedes que se hospedarán
    /// </summary>
    [Required(ErrorMessage = "El número de huéspedes es obligatorio")]
    [Range(1, 100, ErrorMessage = "El número de huéspedes debe estar entre 1 y 100")]
    [Column("numero_huespedes")]
    public int NumeroHuespedes { get; set; }

    #endregion

    #region Financial Information

    /// <summary>
    /// Precio por noche de la habitación al momento de la reserva
    /// Se guarda el precio histórico para mantener consistencia
    /// </summary>
    [Required(ErrorMessage = "El precio por noche es obligatorio")]
    [Column("precio_por_noche", TypeName = "DECIMAL(10,2)")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio por noche debe ser mayor a 0")]
    public decimal PrecioPorNoche { get; set; }

    /// <summary>
    /// Subtotal del hotel en esta reserva
    /// Se calcula automáticamente: NumeroNoches * PrecioPorNoche * NumeroHabitaciones
    /// </summary>
    [Required]
    [Column("subtotal", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El subtotal debe ser positivo")]
    public decimal Subtotal { get; set; }

    #endregion

    #region Additional Information

    /// <summary>
    /// Observaciones o requisitos especiales para el hotel
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Reserva asociada (relación N:1)
    /// </summary>
    [ForeignKey("IdReserva")]
    public virtual Reserva? Reserva { get; set; }

    /// <summary>
    /// Hotel asociado (relación N:1)
    /// </summary>
    [ForeignKey("IdHotel")]
    public virtual Hotel? Hotel { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Número de noches de estadía en el hotel (calculado)
    /// </summary>
    [NotMapped]
    public int NumeroNoches
    {
        get
        {
            var noches = (FechaCheckout - FechaCheckin).Days;
            return noches < 0 ? 0 : noches;
        }
    }

    /// <summary>
    /// Costo total por habitación (precio * noches)
    /// </summary>
    [NotMapped]
    public decimal CostoPorHabitacion => NumeroNoches * PrecioPorNoche;

    /// <summary>
    /// Indica si el check-in ya pasó
    /// </summary>
    [NotMapped]
    public bool CheckinPasado => DateTime.Today >= FechaCheckin;

    /// <summary>
    /// Indica si el check-out ya pasó
    /// </summary>
    [NotMapped]
    public bool CheckoutPasado => DateTime.Today > FechaCheckout;

    /// <summary>
    /// Indica si la estadía está activa (entre check-in y check-out)
    /// </summary>
    [NotMapped]
    public bool EstadiaActiva => DateTime.Today >= FechaCheckin && DateTime.Today <= FechaCheckout;

    /// <summary>
    /// Días restantes hasta el check-in
    /// </summary>
    [NotMapped]
    public int DiasHastaCheckin
    {
        get
        {
            var dias = (FechaCheckin - DateTime.Today).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    #endregion
}

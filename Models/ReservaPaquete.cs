using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad que representa la relación entre una Reserva y un Paquete Turístico.
/// Permite agregar múltiples paquetes a una reserva con sus propias condiciones.
/// </summary>
[Table("reservas_paquetes")]
public class ReservaPaquete
{
    #region Primary Key

    /// <summary>
    /// Identificador único de la relación reserva-paquete
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    #endregion

    #region Foreign Keys

    /// <summary>
    /// ID de la reserva asociada
    /// </summary>
    [Required(ErrorMessage = "La reserva es obligatoria")]
    [Column("id_reserva")]
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del paquete turístico
    /// </summary>
    [Required(ErrorMessage = "El paquete es obligatorio")]
    [Column("id_paquete")]
    public int IdPaquete { get; set; }

    #endregion

    #region Package Details

    /// <summary>
    /// Número de personas que tomarán el paquete
    /// </summary>
    [Required(ErrorMessage = "El número de personas es obligatorio")]
    [Range(1, 100, ErrorMessage = "El número de personas debe estar entre 1 y 100")]
    [Column("numero_personas")]
    public int NumeroPersonas { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete para esta reserva
    /// </summary>
    [Required(ErrorMessage = "La fecha de inicio del paquete es obligatoria")]
    [Column("fecha_inicio_paquete", TypeName = "DATE")]
    public DateTime FechaInicioPaquete { get; set; }

    /// <summary>
    /// Fecha de fin del paquete para esta reserva
    /// Se calcula automáticamente según la duración del paquete
    /// </summary>
    [Required(ErrorMessage = "La fecha de fin del paquete es obligatoria")]
    [Column("fecha_fin_paquete", TypeName = "DATE")]
    public DateTime FechaFinPaquete { get; set; }

    #endregion

    #region Financial Information

    /// <summary>
    /// Precio por persona del paquete (tomado del paquete al momento de la reserva)
    /// </summary>
    [Required(ErrorMessage = "El precio por persona es obligatorio")]
    [Column("precio_por_persona", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El precio por persona debe ser positivo")]
    public decimal PrecioPorPersona { get; set; }

    /// <summary>
    /// Subtotal del paquete (NumeroPersonas * PrecioPorPersona)
    /// Se calcula automáticamente
    /// </summary>
    [Required]
    [Column("subtotal", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El subtotal debe ser positivo")]
    public decimal Subtotal { get; set; }

    #endregion

    #region Additional Information

    /// <summary>
    /// Personalizaciones solicitadas para el paquete (JSON)
    /// Ejemplo: {"alimentacion": "vegetariana", "habitacion": "vista_mar", "excursiones_extra": ["tour_ciudad"]}
    /// </summary>
    [Column("personalizaciones", TypeName = "json")]
    public string? Personalizaciones { get; set; }

    /// <summary>
    /// Observaciones o solicitudes especiales para el paquete
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    #endregion

    #region Audit Fields

    /// <summary>
    /// Fecha y hora en que se agregó el paquete a la reserva
    /// </summary>
    [Required]
    [Column("fecha_agregado")]
    public DateTime FechaAgregado { get; set; } = DateTime.Now;

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Reserva asociada (relación N:1)
    /// </summary>
    [ForeignKey("IdReserva")]
    public virtual Reserva? Reserva { get; set; }

    /// <summary>
    /// Paquete turístico asociado (relación N:1)
    /// </summary>
    [ForeignKey("IdPaquete")]
    public virtual PaqueteTuristico? Paquete { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Duración del paquete en días (calculado)
    /// </summary>
    [NotMapped]
    public int DuracionDias
    {
        get
        {
            var dias = (FechaFinPaquete - FechaInicioPaquete).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    /// <summary>
    /// Precio total formateado
    /// </summary>
    [NotMapped]
    public string SubtotalFormateado => $"${Subtotal:N0} COP";

    /// <summary>
    /// Indica si el paquete ya inició
    /// </summary>
    [NotMapped]
    public bool PaqueteIniciado => DateTime.Today >= FechaInicioPaquete;

    /// <summary>
    /// Indica si el paquete ya finalizó
    /// </summary>
    [NotMapped]
    public bool PaqueteCompletado => DateTime.Today > FechaFinPaquete;

    /// <summary>
    /// Días restantes hasta el inicio del paquete
    /// </summary>
    [NotMapped]
    public int DiasHastaInicio
    {
        get
        {
            var dias = (FechaInicioPaquete - DateTime.Today).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    #endregion
}

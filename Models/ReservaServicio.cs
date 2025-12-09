using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad que representa la relación entre Reservas y Servicios Adicionales.
/// Permite asociar múltiples servicios adicionales a una reserva (relación N:M).
/// </summary>
[Table("reservas_servicios")]
[Index(nameof(IdReserva), Name = "idx_reservaservicio_reserva")]
[Index(nameof(IdServicio), Name = "idx_reservaservicio_servicio")]
public class ReservaServicio
{
    #region Primary Key

    /// <summary>
    /// Identificador único de la relación Reserva-Servicio
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
    /// ID del servicio adicional asociado
    /// </summary>
    [Required(ErrorMessage = "El servicio es obligatorio")]
    [Column("id_servicio")]
    public int IdServicio { get; set; }

    #endregion

    #region Service Details

    /// <summary>
    /// Cantidad de unidades del servicio contratado
    /// (personas, grupos, horas, días, según la unidad del servicio)
    /// </summary>
    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000")]
    [Column("cantidad")]
    public int Cantidad { get; set; }

    /// <summary>
    /// Fecha en que se solicitó el servicio (para planificación)
    /// </summary>
    [Column("fecha_servicio", TypeName = "DATE")]
    public DateTime? FechaServicio { get; set; }

    /// <summary>
    /// Hora programada para el servicio (si aplica)
    /// </summary>
    [Column("hora_servicio")]
    public TimeSpan? HoraServicio { get; set; }

    #endregion

    #region Financial Information

    /// <summary>
    /// Precio unitario del servicio al momento de la reserva
    /// Se toma del ServicioAdicional pero se guarda para mantener historial
    /// </summary>
    [Required]
    [Column("precio_unitario", TypeName = "DECIMAL(10,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "El precio unitario debe ser mayor a 0")]
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Subtotal calculado: Cantidad * PrecioUnitario
    /// Se calcula automáticamente en el servicio
    /// </summary>
    [Required]
    [Column("subtotal", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El subtotal debe ser positivo")]
    public decimal Subtotal { get; set; }

    #endregion

    #region Additional Information

    /// <summary>
    /// Observaciones específicas del servicio en esta reserva
    /// </summary>
    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Estado del servicio: pendiente, confirmado, completado, cancelado
    /// </summary>
    [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
    [Column("estado")]
    public string Estado { get; set; } = "pendiente";

    #endregion

    #region Audit Fields

    /// <summary>
    /// Fecha y hora en que se agregó el servicio a la reserva
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
    /// Servicio adicional asociado (relación N:1)
    /// </summary>
    [ForeignKey("IdServicio")]
    public virtual ServicioAdicional? Servicio { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Indica si el servicio está confirmado
    /// </summary>
    [NotMapped]
    public bool EstaConfirmado => Estado?.ToLower() == "confirmado";

    /// <summary>
    /// Indica si el servicio está completado
    /// </summary>
    [NotMapped]
    public bool EstaCompletado => Estado?.ToLower() == "completado";

    /// <summary>
    /// Indica si el servicio está cancelado
    /// </summary>
    [NotMapped]
    public bool EstaCancelado => Estado?.ToLower() == "cancelado";

    /// <summary>
    /// Indica si el servicio ya fue ejecutado (basado en la fecha)
    /// </summary>
    [NotMapped]
    public bool ServicioEjecutado
    {
        get
        {
            if (!FechaServicio.HasValue)
                return false;
            return DateTime.Today > FechaServicio.Value;
        }
    }

    /// <summary>
    /// Días hasta la ejecución del servicio
    /// </summary>
    [NotMapped]
    public int? DiasHastaServicio
    {
        get
        {
            if (!FechaServicio.HasValue)
                return null;
            var dias = (FechaServicio.Value - DateTime.Today).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    #endregion
}
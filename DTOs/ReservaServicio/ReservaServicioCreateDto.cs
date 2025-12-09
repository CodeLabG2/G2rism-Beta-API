using System.ComponentModel.DataAnnotations;

namespace G2rismBeta.API.DTOs.ReservaServicio;

/// <summary>
/// DTO para crear una nueva relación entre Reserva y Servicio Adicional
/// </summary>
public class ReservaServicioCreateDto
{
    /// <summary>
    /// ID de la reserva (se pasa por URL)
    /// </summary>
    [Required(ErrorMessage = "La reserva es obligatoria")]
    public int IdReserva { get; set; }

    /// <summary>
    /// ID del servicio adicional a agregar
    /// </summary>
    [Required(ErrorMessage = "El servicio es obligatorio")]
    public int IdServicio { get; set; }

    /// <summary>
    /// Cantidad de unidades del servicio
    /// (personas, grupos, horas, días, según la unidad del servicio)
    /// </summary>
    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000")]
    public int Cantidad { get; set; }

    /// <summary>
    /// Fecha en que se solicitó el servicio (opcional, para planificación)
    /// </summary>
    public DateTime? FechaServicio { get; set; }

    /// <summary>
    /// Hora programada para el servicio (opcional)
    /// </summary>
    public TimeSpan? HoraServicio { get; set; }

    /// <summary>
    /// Observaciones específicas del servicio en esta reserva
    /// </summary>
    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Estado del servicio: pendiente, confirmado, completado, cancelado
    /// </summary>
    [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
    [RegularExpression("^(pendiente|confirmado|completado|cancelado)$",
        ErrorMessage = "El estado debe ser 'pendiente', 'confirmado', 'completado' o 'cancelado'")]
    public string Estado { get; set; } = "pendiente";
}
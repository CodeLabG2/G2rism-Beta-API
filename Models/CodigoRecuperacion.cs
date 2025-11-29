using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad para gestionar códigos de recuperación de contraseña (6 dígitos)
/// Migrado desde TokenRecuperacion para usar códigos numéricos cortos
/// </summary>
[Table("codigos_recuperacion")]
public class CodigoRecuperacion
{
    /// <summary>
    /// Identificador único del código
    /// </summary>
    [Key]
    [Column("id_codigo")]
    public int IdCodigo { get; set; }

    /// <summary>
    /// ID del usuario al que pertenece el código
    /// </summary>
    [Required(ErrorMessage = "El ID del usuario es obligatorio")]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    /// <summary>
    /// Código numérico de 6 dígitos (ej: "123456")
    /// Reemplaza el anterior token GUID largo
    /// </summary>
    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener exactamente 6 dígitos")]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de código
    /// Posibles valores: 'recuperacion_password', 'verificacion_email', 'activacion_cuenta'
    /// </summary>
    [Required(ErrorMessage = "El tipo de código es obligatorio")]
    [Column("tipo_codigo")]
    public string TipoCodigo { get; set; } = "recuperacion_password";

    /// <summary>
    /// Fecha y hora de generación del código
    /// </summary>
    [Column("fecha_generacion")]
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha y hora de expiración del código
    /// Típicamente 1 hora después de la generación
    /// </summary>
    [Required(ErrorMessage = "La fecha de expiración es obligatoria")]
    [Column("fecha_expiracion")]
    public DateTime FechaExpiracion { get; set; }

    /// <summary>
    /// Indica si el código ya fue utilizado
    /// Un código solo puede usarse una vez
    /// </summary>
    [Column("usado")]
    public bool Usado { get; set; } = false;

    /// <summary>
    /// Fecha y hora en que se usó el código
    /// NULL si aún no se ha usado
    /// </summary>
    [Column("fecha_uso")]
    public DateTime? FechaUso { get; set; }

    /// <summary>
    /// IP desde la que se solicitó el código
    /// Para auditoría y seguridad
    /// </summary>
    [StringLength(45, ErrorMessage = "La IP no puede exceder 45 caracteres")]
    [Column("ip_solicitud")]
    public string? IpSolicitud { get; set; }

    /// <summary>
    /// Contador de intentos de validación fallidos
    /// Para prevenir ataques de fuerza bruta
    /// Máximo permitido: 5 intentos
    /// </summary>
    [Column("intentos_validacion")]
    public int IntentosValidacion { get; set; } = 0;

    /// <summary>
    /// Indica si el código fue bloqueado por exceso de intentos
    /// </summary>
    [Column("bloqueado")]
    public bool Bloqueado { get; set; } = false;

    // ========================================
    // PROPIEDADES CALCULADAS
    // ========================================

    /// <summary>
    /// Indica si el código está activo (no usado, no expirado, no bloqueado)
    /// </summary>
    [NotMapped]
    public bool EstaActivo => !Usado && !Bloqueado && DateTime.Now < FechaExpiracion;

    /// <summary>
    /// Indica si ha expirado
    /// </summary>
    [NotMapped]
    public bool HaExpirado => DateTime.Now >= FechaExpiracion;

    /// <summary>
    /// Intentos restantes antes del bloqueo
    /// </summary>
    [NotMapped]
    public int IntentosRestantes => Math.Max(0, 5 - IntentosValidacion);

    // ========================================
    // RELACIONES DE NAVEGACIÓN
    // ========================================

    /// <summary>
    /// Usuario al que pertenece el código
    /// </summary>
    [ForeignKey("IdUsuario")]
    public virtual Usuario Usuario { get; set; } = null!;
}

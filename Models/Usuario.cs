using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad que representa un Usuario en el sistema
/// Un usuario puede ser un Cliente o un Empleado
/// </summary>
[Table("usuarios")]
public class Usuario
{
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nombre de usuario para login (único en el sistema)
    /// Ejemplo: "juan.perez", "admin"
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario (único, usado para login y recuperación)
    /// </summary>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña encriptada con BCrypt
    /// NUNCA almacenar contraseñas en texto plano
    /// </summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(255, ErrorMessage = "El hash de contraseña no puede exceder 255 caracteres")]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario: 'cliente' o 'empleado'
    /// </summary>
    [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
    [Column("tipo_usuario")]
    public string TipoUsuario { get; set; } = "cliente"; // 'cliente' o 'empleado'

    /// <summary>
    /// Fecha y hora del último acceso al sistema
    /// </summary>
    [Column("ultimo_acceso")]
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// Contador de intentos de login fallidos
    /// Se reinicia a 0 cuando el login es exitoso
    /// </summary>
    [Range(0, 10, ErrorMessage = "Los intentos fallidos deben estar entre 0 y 10")]
    [Column("intentos_fallidos")]
    public int IntentosFallidos { get; set; } = 0;

    /// <summary>
    /// Indica si la cuenta está bloqueada
    /// Se bloquea automáticamente después de X intentos fallidos
    /// </summary>
    [Column("bloqueado")]
    public bool Bloqueado { get; set; } = false;

    /// <summary>
    /// Estado del usuario (true = activo, false = inactivo)
    /// </summary>
    [Column("estado")]
    public bool Estado { get; set; } = true;

    /// <summary>
    /// Fecha y hora de creación del usuario
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha y hora de la última modificación
    /// </summary>
    [Column("fecha_modificacion")]
    public DateTime? FechaModificacion { get; set; }

    // ========================================
    // RELACIONES DE NAVEGACIÓN
    // ========================================

    /// <summary>
    /// Roles asignados al usuario
    /// Relación muchos a muchos a través de UsuarioRol
    /// </summary>
    public virtual ICollection<UsuarioRol> UsuariosRoles { get; set; } = new List<UsuarioRol>();

    /// <summary>
    /// Tokens de recuperación generados por este usuario (LEGACY)
    /// </summary>
    public virtual ICollection<TokenRecuperacion> TokensRecuperacion { get; set; } = new List<TokenRecuperacion>();

    /// <summary>
    /// Códigos de recuperación generados por este usuario (6 dígitos)
    /// </summary>
    public virtual ICollection<CodigoRecuperacion> CodigosRecuperacion { get; set; } = new List<CodigoRecuperacion>();
}
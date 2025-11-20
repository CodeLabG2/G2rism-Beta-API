namespace G2rismBeta.API.DTOs.Usuario;

/// <summary>
/// DTO para respuesta de un usuario (sin datos sensibles)
/// </summary>
public class UsuarioResponseDto
{
    /// <summary>
    /// ID único del usuario
    /// </summary>
    /// <example>1</example>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nombre de usuario
    /// </summary>
    /// <example>juan.perez</example>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico
    /// </summary>
    /// <example>juan.perez@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario
    /// </summary>
    /// <example>cliente</example>
    public string TipoUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora del último acceso
    /// </summary>
    /// <example>2025-10-31T10:30:00</example>
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// Cantidad de intentos fallidos de login
    /// </summary>
    /// <example>0</example>
    public int IntentosFallidos { get; set; }

    /// <summary>
    /// Indica si la cuenta está bloqueada
    /// </summary>
    /// <example>false</example>
    public bool Bloqueado { get; set; }

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    /// <example>true</example>
    public bool Estado { get; set; }

    /// <summary>
    /// Fecha de creación del usuario
    /// </summary>
    /// <example>2025-10-01T08:00:00</example>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    /// <example>2025-10-31T15:45:00</example>
    public DateTime? FechaModificacion { get; set; }
}
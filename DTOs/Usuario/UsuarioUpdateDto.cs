using System.ComponentModel.DataAnnotations;

namespace G2rismBeta.API.DTOs.Usuario;

/// <summary>
/// DTO para actualizar un usuario existente
/// Todos los campos son opcionales excepto el ID
/// </summary>
public class UsuarioUpdateDto
{
    /// <summary>
    /// Nuevo username (opcional)
    /// </summary>
    /// <example>juan.perez.updated</example>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El username solo puede contener letras, números, puntos, guiones y guiones bajos")]
    public string? Username { get; set; }

    /// <summary>
    /// Nuevo email (opcional)
    /// </summary>
    /// <example>juan.nuevo@example.com</example>
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string? Email { get; set; }

    /// <summary>
    /// Nuevo tipo de usuario (opcional)
    /// </summary>
    /// <example>empleado</example>
    [RegularExpression(@"^(cliente|empleado)$", ErrorMessage = "El tipo de usuario debe ser 'cliente' o 'empleado'")]
    public string? TipoUsuario { get; set; }
}
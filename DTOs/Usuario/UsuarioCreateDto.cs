using System.ComponentModel.DataAnnotations;

namespace G2rismBeta.API.DTOs.Usuario;

/// <summary>
/// DTO para crear un nuevo usuario
/// </summary>
public class UsuarioCreateDto
{
    /// <summary>
    /// Nombre de usuario único para login
    /// </summary>
    /// <example>juan.perez</example>
    [Required(ErrorMessage = "El username es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El username solo puede contener letras, números, puntos, guiones y guiones bajos")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico único
    /// </summary>
    /// <example>juan.perez@example.com</example>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario (será hasheada)
    /// Debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial
    /// </summary>
    /// <example>Password123!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmación de contraseña
    /// </summary>
    /// <example>Password123!</example>
    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario: 'cliente' o 'empleado'
    /// </summary>
    /// <example>cliente</example>
    [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
    [RegularExpression(@"^(cliente|empleado)$", ErrorMessage = "El tipo de usuario debe ser 'cliente' o 'empleado'")]
    public string TipoUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Lista de IDs de roles a asignar al usuario (opcional)
    /// Si no se proporciona, no se asignarán roles
    /// </summary>
    /// <example>[1, 2]</example>
    public List<int>? RolesIds { get; set; }
}
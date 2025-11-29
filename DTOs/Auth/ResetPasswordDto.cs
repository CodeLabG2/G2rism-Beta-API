using System.ComponentModel.DataAnnotations;

namespace G2rismBeta.API.DTOs.Auth;

/// <summary>
/// DTO para resetear la contraseña usando el código de recuperación de 6 dígitos
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// Código de recuperación de 6 dígitos recibido por email
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener exactamente 6 dígitos")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe contener exactamente 6 dígitos numéricos")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nueva contraseña
    /// Debe cumplir con los requisitos de seguridad
    /// </summary>
    /// <example>NewPassword123!</example>
    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmación de la nueva contraseña
    /// </summary>
    /// <example>NewPassword123!</example>
    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
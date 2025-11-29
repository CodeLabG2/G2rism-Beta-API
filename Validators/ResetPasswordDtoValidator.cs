using FluentValidation;
using G2rismBeta.API.DTOs.Auth;
using G2rismBeta.API.Helpers;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para ResetPasswordDto
/// </summary>
public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        // ========================================
        // VALIDACIÓN DE CÓDIGO DE 6 DÍGITOS
        // ========================================

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio")
            .Length(6).WithMessage("El código debe tener exactamente 6 dígitos")
            .Must(codigo => !string.IsNullOrWhiteSpace(codigo) && codigo.Trim() == codigo)
                .WithMessage("El código no debe contener espacios")
            .Must(codigo => codigo.All(char.IsDigit))
                .WithMessage("El código debe contener solo dígitos numéricos");

        // ========================================
        // VALIDACIÓN DE NEW PASSWORD
        // ========================================

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria")
            .Length(8, 100).WithMessage("La nueva contraseña debe tener entre 8 y 100 caracteres")
            .Must(password =>
            {
                var (isValid, _) = PasswordHasher.ValidatePasswordStrength(password);
                return isValid;
            })
            .WithMessage("La nueva contraseña debe contener al menos: una mayúscula, una minúscula, un número y un carácter especial");

        // ========================================
        // VALIDACIÓN DE CONFIRM PASSWORD
        // ========================================

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("La confirmación de contraseña es obligatoria")
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden");
    }
}
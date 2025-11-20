using FluentValidation;
using G2rismBeta.API.DTOs.Usuario;
using G2rismBeta.API.Helpers;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para UsuarioCreateDto
/// Implementa reglas de validación adicionales a las Data Annotations
/// </summary>
public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
{
    public UsuarioCreateDtoValidator()
    {
        // ========================================
        // VALIDACIÓN DE USERNAME
        // ========================================

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El username es obligatorio")
            .Length(3, 50).WithMessage("El username debe tener entre 3 y 50 caracteres")
            .Matches(@"^[a-zA-Z0-9._-]+$")
                .WithMessage("El username solo puede contener letras, números, puntos, guiones y guiones bajos")
            .Must(username => !username.StartsWith(".") && !username.EndsWith("."))
                .WithMessage("El username no puede comenzar ni terminar con punto")
            .Must(username => !username.Contains(".."))
                .WithMessage("El username no puede contener puntos consecutivos");

        // ========================================
        // VALIDACIÓN DE EMAIL
        // ========================================

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .Must(email => email.ToLower() == email || email.ToUpper() != email)
                .WithMessage("El email debe estar en minúsculas");

        // ========================================
        // VALIDACIÓN DE PASSWORD
        // ========================================

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria")
            .Length(8, 100).WithMessage("La contraseña debe tener entre 8 y 100 caracteres")
            .Must(password =>
            {
                var (isValid, _) = PasswordHasher.ValidatePasswordStrength(password);
                return isValid;
            })
            .WithMessage("La contraseña debe contener al menos: una mayúscula, una minúscula, un número y un carácter especial");

        // ========================================
        // VALIDACIÓN DE CONFIRM PASSWORD
        // ========================================

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("La confirmación de contraseña es obligatoria")
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

        // ========================================
        // VALIDACIÓN DE TIPO USUARIO
        // ========================================

        RuleFor(x => x.TipoUsuario)
            .NotEmpty().WithMessage("El tipo de usuario es obligatorio")
            .Must(tipo => tipo == "cliente" || tipo == "empleado")
                .WithMessage("El tipo de usuario debe ser 'cliente' o 'empleado'");

        // ========================================
        // VALIDACIÓN DE ROLES IDS
        // ========================================

        RuleFor(x => x.RolesIds)
            .Must(roles => roles == null || roles.Count > 0)
                .WithMessage("Si se proporcionan roles, debe haber al menos uno")
            .Must(roles => roles == null || roles.All(id => id > 0))
                .WithMessage("Todos los IDs de roles deben ser mayores a 0")
            .When(x => x.RolesIds != null);
    }
}
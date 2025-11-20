using FluentValidation;
using G2rismBeta.API.DTOs.Usuario;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para UsuarioUpdateDto
/// Todos los campos son opcionales pero si se proporcionan deben ser válidos
/// </summary>
public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator()
    {
        // ========================================
        // VALIDACIÓN DE USERNAME (OPCIONAL)
        // ========================================

        RuleFor(x => x.Username)
            .Length(3, 50).WithMessage("El username debe tener entre 3 y 50 caracteres")
            .Matches(@"^[a-zA-Z0-9._-]+$")
                .WithMessage("El username solo puede contener letras, números, puntos, guiones y guiones bajos")
            .Must(username => !username!.StartsWith(".") && !username.EndsWith("."))
                .WithMessage("El username no puede comenzar ni terminar con punto")
            .Must(username => !username!.Contains(".."))
                .WithMessage("El username no puede contener puntos consecutivos")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        // ========================================
        // VALIDACIÓN DE EMAIL (OPCIONAL)
        // ========================================

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .Must(email => email!.ToLower() == email || email.ToUpper() != email)
                .WithMessage("El email debe estar en minúsculas")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // ========================================
        // VALIDACIÓN DE TIPO USUARIO (OPCIONAL)
        // ========================================

        RuleFor(x => x.TipoUsuario)
            .Must(tipo => tipo == "cliente" || tipo == "empleado")
                .WithMessage("El tipo de usuario debe ser 'cliente' o 'empleado'")
            .When(x => !string.IsNullOrWhiteSpace(x.TipoUsuario));

        // ========================================
        // VALIDACIÓN GENERAL
        // ========================================

        // Al menos un campo debe ser proporcionado para actualizar
        RuleFor(x => x)
            .Must(dto =>
                !string.IsNullOrWhiteSpace(dto.Username) ||
                !string.IsNullOrWhiteSpace(dto.Email) ||
                !string.IsNullOrWhiteSpace(dto.TipoUsuario)
            )
            .WithMessage("Debe proporcionar al menos un campo para actualizar");
    }
}
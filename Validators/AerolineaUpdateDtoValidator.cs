using FluentValidation;
using G2rismBeta.API.DTOs.Aerolinea;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para la actualización de aerolíneas
/// Todos los campos son opcionales, pero si se proporcionan deben ser válidos
/// </summary>
public class AerolineaUpdateDtoValidator : AbstractValidator<AerolineaUpdateDto>
{
    public AerolineaUpdateDtoValidator()
    {
        // Nombre (opcional)
        RuleFor(x => x.Nombre)
            .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Nombre));

        // País (opcional)
        RuleFor(x => x.Pais)
            .MaximumLength(100).WithMessage("El país no puede exceder 100 caracteres")
            .MinimumLength(2).WithMessage("El país debe tener al menos 2 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Pais));

        // Sitio Web (opcional)
        RuleFor(x => x.SitioWeb)
            .MaximumLength(200).WithMessage("El sitio web no puede exceder 200 caracteres")
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("El sitio web debe ser una URL válida")
            .When(x => !string.IsNullOrWhiteSpace(x.SitioWeb));

        // Teléfono (opcional)
        RuleFor(x => x.TelefonoContacto)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("El teléfono contiene caracteres inválidos")
            .When(x => !string.IsNullOrWhiteSpace(x.TelefonoContacto));

        // Email (opcional)
        RuleFor(x => x.EmailContacto)
            .EmailAddress().WithMessage("El email no tiene un formato válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.EmailContacto));

        // Políticas de equipaje (opcional)
        RuleFor(x => x.PoliticasEquipaje)
            .MaximumLength(5000).WithMessage("Las políticas de equipaje no pueden exceder 5000 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.PoliticasEquipaje));

        // Estado (opcional)
        RuleFor(x => x.Estado)
            .Must(estado => new[] { "Activa", "Inactiva" }.Contains(estado))
            .WithMessage("El estado debe ser 'Activa' o 'Inactiva'")
            .When(x => !string.IsNullOrWhiteSpace(x.Estado));
    }
}

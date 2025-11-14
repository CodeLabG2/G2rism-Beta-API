using FluentValidation;
using G2rismBeta.API.DTOs.Aerolinea;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para la creación de aerolíneas
/// </summary>
public class AerolineaCreateDtoValidator : AbstractValidator<AerolineaCreateDto>
{
    public AerolineaCreateDtoValidator()
    {
        // Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

        // Código IATA
        RuleFor(x => x.CodigoIata)
            .NotEmpty().WithMessage("El código IATA es obligatorio")
            .Length(2).WithMessage("El código IATA debe tener exactamente 2 caracteres")
            .Matches("^[A-Z0-9]{2}$").WithMessage("El código IATA debe contener solo letras mayúsculas o números");

        // Código ICAO
        RuleFor(x => x.CodigoIcao)
            .NotEmpty().WithMessage("El código ICAO es obligatorio")
            .Length(3).WithMessage("El código ICAO debe tener exactamente 3 caracteres")
            .Matches("^[A-Z0-9]{3}$").WithMessage("El código ICAO debe contener solo letras mayúsculas o números");

        // País
        RuleFor(x => x.Pais)
            .NotEmpty().WithMessage("El país es obligatorio")
            .MaximumLength(100).WithMessage("El país no puede exceder 100 caracteres")
            .MinimumLength(2).WithMessage("El país debe tener al menos 2 caracteres");

        // Sitio Web (opcional pero validado si se proporciona)
        RuleFor(x => x.SitioWeb)
            .MaximumLength(200).WithMessage("El sitio web no puede exceder 200 caracteres")
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("El sitio web debe ser una URL válida")
            .When(x => !string.IsNullOrWhiteSpace(x.SitioWeb));

        // Teléfono
        RuleFor(x => x.TelefonoContacto)
            .NotEmpty().WithMessage("El teléfono de contacto es obligatorio")
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("El teléfono contiene caracteres inválidos");

        // Email
        RuleFor(x => x.EmailContacto)
            .NotEmpty().WithMessage("El email de contacto es obligatorio")
            .EmailAddress().WithMessage("El email no tiene un formato válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        // Políticas de equipaje (opcional)
        RuleFor(x => x.PoliticasEquipaje)
            .MaximumLength(5000).WithMessage("Las políticas de equipaje no pueden exceder 5000 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.PoliticasEquipaje));

        // Estado
        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio")
            .Must(estado => new[] { "Activa", "Inactiva" }.Contains(estado))
            .WithMessage("El estado debe ser 'Activa' o 'Inactiva'");
    }
}

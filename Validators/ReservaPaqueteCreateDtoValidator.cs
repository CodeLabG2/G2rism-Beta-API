using FluentValidation;
using G2rismBeta.API.DTOs.ReservaPaquete;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para ReservaPaqueteCreateDto
/// Valida la estructura y reglas de negocio básicas para agregar un paquete a una reserva
/// </summary>
public class ReservaPaqueteCreateDtoValidator : AbstractValidator<ReservaPaqueteCreateDto>
{
    public ReservaPaqueteCreateDtoValidator()
    {
        // Validación del IdPaquete
        RuleFor(x => x.IdPaquete)
            .GreaterThan(0)
            .WithMessage("El ID del paquete debe ser mayor a 0");

        // Validación del número de personas
        RuleFor(x => x.NumeroPersonas)
            .GreaterThan(0)
            .WithMessage("El número de personas debe ser mayor a 0")
            .LessThanOrEqualTo(100)
            .WithMessage("El número de personas no puede exceder 100");

        // Validación de fecha de inicio
        RuleFor(x => x.FechaInicioPaquete)
            .NotEmpty()
            .WithMessage("La fecha de inicio del paquete es obligatoria")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("La fecha de inicio del paquete no puede ser anterior a hoy");

        // Validación de fecha de fin (si se proporciona)
        RuleFor(x => x.FechaFinPaquete)
            .GreaterThan(x => x.FechaInicioPaquete)
            .When(x => x.FechaFinPaquete.HasValue)
            .WithMessage("La fecha de fin del paquete debe ser posterior a la fecha de inicio");

        // Validación de personalizaciones (si se proporciona, debe ser JSON válido)
        RuleFor(x => x.Personalizaciones)
            .Must(BeValidJsonOrNull)
            .When(x => !string.IsNullOrWhiteSpace(x.Personalizaciones))
            .WithMessage("Las personalizaciones deben ser un JSON válido");

        // Validación de observaciones
        RuleFor(x => x.Observaciones)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
            .WithMessage("Las observaciones no pueden exceder 500 caracteres");
    }

    /// <summary>
    /// Valida si el string es un JSON válido o null
    /// </summary>
    private bool BeValidJsonOrNull(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return true;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

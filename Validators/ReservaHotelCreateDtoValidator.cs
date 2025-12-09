using FluentValidation;
using G2rismBeta.API.DTOs.ReservaHotel;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para ReservaHotelCreateDto
/// Valida las reglas de negocio para agregar un hotel a una reserva
/// </summary>
public class ReservaHotelCreateDtoValidator : AbstractValidator<ReservaHotelCreateDto>
{
    public ReservaHotelCreateDtoValidator()
    {
        // ID del Hotel
        RuleFor(x => x.IdHotel)
            .GreaterThan(0)
            .WithMessage("El ID del hotel debe ser mayor a 0");

        // Fecha Check-in
        RuleFor(x => x.FechaCheckin)
            .NotEmpty()
            .WithMessage("La fecha de check-in es obligatoria")
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-1))
            .WithMessage("La fecha de check-in no puede ser anterior a hoy");

        // Fecha Check-out
        RuleFor(x => x.FechaCheckout)
            .NotEmpty()
            .WithMessage("La fecha de check-out es obligatoria")
            .GreaterThan(x => x.FechaCheckin)
            .WithMessage("La fecha de check-out debe ser posterior a la fecha de check-in");

        // Validación de duración mínima (al menos 1 noche)
        RuleFor(x => x)
            .Must(x => (x.FechaCheckout - x.FechaCheckin).Days >= 1)
            .WithMessage("La estadía debe ser de al menos 1 noche")
            .WithName("Fechas");

        // Número de habitaciones
        RuleFor(x => x.NumeroHabitaciones)
            .GreaterThan(0)
            .WithMessage("El número de habitaciones debe ser mayor a 0")
            .LessThanOrEqualTo(50)
            .WithMessage("El número de habitaciones no puede exceder 50");

        // Tipo de habitación (opcional pero con validación si se proporciona)
        RuleFor(x => x.TipoHabitacion)
            .MaximumLength(50)
            .WithMessage("El tipo de habitación no puede exceder 50 caracteres")
            .Must(tipo => string.IsNullOrWhiteSpace(tipo) ||
                         new[] { "simple", "doble", "triple", "suite", "presidencial" }
                         .Contains(tipo.ToLower()))
            .When(x => !string.IsNullOrWhiteSpace(x.TipoHabitacion))
            .WithMessage("El tipo de habitación debe ser: simple, doble, triple, suite o presidencial");

        // Número de huéspedes
        RuleFor(x => x.NumeroHuespedes)
            .GreaterThan(0)
            .WithMessage("El número de huéspedes debe ser mayor a 0")
            .LessThanOrEqualTo(100)
            .WithMessage("El número de huéspedes no puede exceder 100");

        // Validación lógica: huéspedes vs habitaciones
        RuleFor(x => x)
            .Must(x => x.NumeroHuespedes <= (x.NumeroHabitaciones * 4))
            .WithMessage("El número de huéspedes es muy alto para el número de habitaciones reservadas (máximo 4 por habitación)")
            .WithName("Capacidad");

        // Observaciones (opcional)
        RuleFor(x => x.Observaciones)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Observaciones));
    }
}

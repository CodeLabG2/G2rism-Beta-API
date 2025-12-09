using FluentValidation;
using G2rismBeta.API.DTOs.ReservaServicio;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para ReservaServicioCreateDto
/// Valida las reglas de negocio al agregar un servicio a una reserva
/// </summary>
public class ReservaServicioCreateDtoValidator : AbstractValidator<ReservaServicioCreateDto>
{
    public ReservaServicioCreateDtoValidator()
    {
        // ============================================
        // VALIDACIÓN DE IDs
        // ============================================

        RuleFor(x => x.IdReserva)
            .GreaterThan(0)
            .WithMessage("El ID de la reserva debe ser mayor a 0");

        RuleFor(x => x.IdServicio)
            .GreaterThan(0)
            .WithMessage("El ID del servicio debe ser mayor a 0");

        // ============================================
        // VALIDACIÓN DE CANTIDAD
        // ============================================

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("La cantidad no puede exceder 1000 unidades");

        // ============================================
        // VALIDACIÓN DE FECHA Y HORA DEL SERVICIO
        // ============================================

        RuleFor(x => x.FechaServicio)
            .Must(fecha => !fecha.HasValue || fecha.Value >= DateTime.Today)
            .WithMessage("La fecha del servicio no puede ser anterior a hoy")
            .When(x => x.FechaServicio.HasValue);

        RuleFor(x => x.HoraServicio)
            .Must(hora => !hora.HasValue || (hora.Value >= TimeSpan.Zero && hora.Value < TimeSpan.FromHours(24)))
            .WithMessage("La hora del servicio debe estar entre 00:00 y 23:59")
            .When(x => x.HoraServicio.HasValue);

        // ============================================
        // VALIDACIÓN DE OBSERVACIONES
        // ============================================

        RuleFor(x => x.Observaciones)
            .MaximumLength(1000)
            .WithMessage("Las observaciones no pueden exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observaciones));

        // ============================================
        // VALIDACIÓN DE ESTADO
        // ============================================

        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("El estado es obligatorio")
            .Must(estado => new[] { "pendiente", "confirmado", "completado", "cancelado" }
                .Contains(estado?.ToLower()))
            .WithMessage("El estado debe ser 'pendiente', 'confirmado', 'completado' o 'cancelado'");

        // ============================================
        // VALIDACIÓN DE LÓGICA DE NEGOCIO
        // ============================================

        // Si se especifica hora, debe haber fecha
        RuleFor(x => x.FechaServicio)
            .NotNull()
            .WithMessage("Debe especificar la fecha del servicio si indica una hora")
            .When(x => x.HoraServicio.HasValue);
    }
}
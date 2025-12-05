using FluentValidation;
using G2rismBeta.API.DTOs.Vuelo;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para la actualización de vuelos
/// </summary>
public class VueloUpdateDtoValidator : AbstractValidator<VueloUpdateDto>
{
    public VueloUpdateDtoValidator()
    {
        // Número de Vuelo
        RuleFor(x => x.NumeroVuelo)
            .NotEmpty().WithMessage("El número de vuelo no puede estar vacío")
            .MaximumLength(10).WithMessage("El número de vuelo no puede exceder 10 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("El número de vuelo debe contener solo letras mayúsculas y números")
            .When(x => x.NumeroVuelo != null);

        // Origen
        RuleFor(x => x.Origen)
            .NotEmpty().WithMessage("El origen no puede estar vacío")
            .MaximumLength(100).WithMessage("El origen no puede exceder 100 caracteres")
            .MinimumLength(3).WithMessage("El origen debe tener al menos 3 caracteres")
            .When(x => x.Origen != null);

        // Destino
        RuleFor(x => x.Destino)
            .NotEmpty().WithMessage("El destino no puede estar vacío")
            .MaximumLength(100).WithMessage("El destino no puede exceder 100 caracteres")
            .MinimumLength(3).WithMessage("El destino debe tener al menos 3 caracteres")
            .When(x => x.Destino != null);

        // Fecha Salida
        RuleFor(x => x.FechaSalida)
            .GreaterThan(DateTime.Now).WithMessage("La fecha de salida debe ser mayor a la fecha actual")
            .When(x => x.FechaSalida.HasValue);

        // Fecha Llegada
        RuleFor(x => x.FechaLlegada)
            .GreaterThan(x => x.FechaSalida ?? DateTime.MinValue).WithMessage("La fecha de llegada debe ser posterior a la fecha de salida")
            .When(x => x.FechaLlegada.HasValue);

        // Hora Salida
        RuleFor(x => x.HoraSalida)
            .GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("La hora de salida no es válida")
            .LessThan(TimeSpan.FromHours(24)).WithMessage("La hora de salida no es válida")
            .When(x => x.HoraSalida.HasValue);

        // Hora Llegada
        RuleFor(x => x.HoraLlegada)
            .GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("La hora de llegada no es válida")
            .LessThan(TimeSpan.FromHours(24)).WithMessage("La hora de llegada no es válida")
            .When(x => x.HoraLlegada.HasValue);

        // Cupos Disponibles
        RuleFor(x => x.CuposDisponibles)
            .GreaterThanOrEqualTo(0).WithMessage("Los cupos disponibles no pueden ser negativos")
            .LessThanOrEqualTo(x => x.CuposTotales ?? int.MaxValue).WithMessage("Los cupos disponibles no pueden exceder los cupos totales")
            .When(x => x.CuposDisponibles.HasValue);

        // Cupos Totales
        RuleFor(x => x.CuposTotales)
            .GreaterThan(0).WithMessage("Los cupos totales deben ser mayor a 0")
            .LessThanOrEqualTo(1000).WithMessage("Los cupos totales no pueden exceder 1000")
            .When(x => x.CuposTotales.HasValue);

        // Precio Económica
        RuleFor(x => x.PrecioEconomica)
            .GreaterThan(0).WithMessage("El precio de clase económica debe ser mayor a 0")
            .LessThan(100000000).WithMessage("El precio de clase económica es demasiado alto")
            .When(x => x.PrecioEconomica.HasValue);

        // Precio Ejecutiva
        RuleFor(x => x.PrecioEjecutiva)
            .GreaterThan(0).WithMessage("El precio de clase ejecutiva debe ser mayor a 0")
            .GreaterThanOrEqualTo(x => x.PrecioEconomica ?? 0).WithMessage("El precio de clase ejecutiva debe ser mayor o igual al de clase económica")
            .When(x => x.PrecioEjecutiva.HasValue);

        // Duración Minutos
        RuleFor(x => x.DuracionMinutos)
            .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos")
            .LessThanOrEqualTo(1440).WithMessage("La duración no puede exceder 24 horas (1440 minutos)")
            .When(x => x.DuracionMinutos.HasValue);

        // Escalas
        RuleFor(x => x.Escalas)
            .GreaterThanOrEqualTo(0).WithMessage("El número de escalas no puede ser negativo")
            .LessThanOrEqualTo(5).WithMessage("El número de escalas no puede exceder 5")
            .When(x => x.Escalas.HasValue);

        // Observaciones
        RuleFor(x => x.Observaciones)
            .MaximumLength(500).WithMessage("Las observaciones no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Observaciones));
    }
}

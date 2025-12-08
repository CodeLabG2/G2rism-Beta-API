using FluentValidation;
using G2rismBeta.API.DTOs.Vuelo;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para la creación de vuelos
/// </summary>
public class VueloCreateDtoValidator : AbstractValidator<VueloCreateDto>
{
    public VueloCreateDtoValidator()
    {
        // ID Aerolínea
        RuleFor(x => x.IdAerolinea)
            .GreaterThan(0).WithMessage("El ID de la aerolínea debe ser mayor a 0");

        // ID Proveedor
        RuleFor(x => x.IdProveedor)
            .GreaterThan(0).WithMessage("El ID del proveedor debe ser mayor a 0");

        // Número de Vuelo
        RuleFor(x => x.NumeroVuelo)
            .NotEmpty().WithMessage("El número de vuelo es obligatorio")
            .MaximumLength(10).WithMessage("El número de vuelo no puede exceder 10 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("El número de vuelo debe contener solo letras mayúsculas y números");

        // Origen
        RuleFor(x => x.Origen)
            .NotEmpty().WithMessage("El origen es obligatorio")
            .MaximumLength(100).WithMessage("El origen no puede exceder 100 caracteres")
            .MinimumLength(3).WithMessage("El origen debe tener al menos 3 caracteres");

        // Destino
        RuleFor(x => x.Destino)
            .NotEmpty().WithMessage("El destino es obligatorio")
            .MaximumLength(100).WithMessage("El destino no puede exceder 100 caracteres")
            .MinimumLength(3).WithMessage("El destino debe tener al menos 3 caracteres")
            .NotEqual(x => x.Origen).WithMessage("El destino no puede ser igual al origen");

        // Fecha Salida
        RuleFor(x => x.FechaSalida)
            .NotEmpty().WithMessage("La fecha de salida es obligatoria")
            .GreaterThan(DateTime.Now).WithMessage("La fecha de salida debe ser mayor a la fecha actual");

        // Fecha Llegada
        RuleFor(x => x.FechaLlegada)
            .NotEmpty().WithMessage("La fecha de llegada es obligatoria")
            .GreaterThanOrEqualTo(x => x.FechaSalida).WithMessage("La fecha de llegada debe ser posterior o igual a la fecha de salida");

        // Hora Salida
        RuleFor(x => x.HoraSalida)
            .NotEmpty().WithMessage("La hora de salida es obligatoria")
            .GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("La hora de salida no es válida")
            .LessThan(TimeSpan.FromHours(24)).WithMessage("La hora de salida no es válida");

        // Hora Llegada
        RuleFor(x => x.HoraLlegada)
            .NotEmpty().WithMessage("La hora de llegada es obligatoria")
            .GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("La hora de llegada no es válida")
            .LessThan(TimeSpan.FromHours(24)).WithMessage("La hora de llegada no es válida");

        // Cupos Disponibles
        RuleFor(x => x.CuposDisponibles)
            .GreaterThanOrEqualTo(0).WithMessage("Los cupos disponibles no pueden ser negativos")
            .LessThanOrEqualTo(x => x.CuposTotales).WithMessage("Los cupos disponibles no pueden exceder los cupos totales");

        // Cupos Totales
        RuleFor(x => x.CuposTotales)
            .GreaterThan(0).WithMessage("Los cupos totales deben ser mayor a 0")
            .LessThanOrEqualTo(1000).WithMessage("Los cupos totales no pueden exceder 1000");

        // Precio Económica
        RuleFor(x => x.PrecioEconomica)
            .GreaterThan(0).WithMessage("El precio de clase económica debe ser mayor a 0")
            .LessThan(100000000).WithMessage("El precio de clase económica es demasiado alto");

        // Precio Ejecutiva (opcional)
        RuleFor(x => x.PrecioEjecutiva)
            .GreaterThan(0).WithMessage("El precio de clase ejecutiva debe ser mayor a 0")
            .GreaterThanOrEqualTo(x => x.PrecioEconomica).WithMessage("El precio de clase ejecutiva debe ser mayor o igual al de clase económica")
            .When(x => x.PrecioEjecutiva.HasValue);

        // Duración Minutos
        RuleFor(x => x.DuracionMinutos)
            .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos")
            .LessThanOrEqualTo(1440).WithMessage("La duración no puede exceder 24 horas (1440 minutos)");

        // Escalas
        RuleFor(x => x.Escalas)
            .GreaterThanOrEqualTo(0).WithMessage("El número de escalas no puede ser negativo")
            .LessThanOrEqualTo(5).WithMessage("El número de escalas no puede exceder 5");

        // Observaciones (opcional)
        RuleFor(x => x.Observaciones)
            .MaximumLength(500).WithMessage("Las observaciones no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Observaciones));
    }
}

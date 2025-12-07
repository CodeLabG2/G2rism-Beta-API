using FluentValidation;
using G2rismBeta.API.DTOs.PaqueteTuristico;
using System.Text.Json;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para PaqueteTuristicoUpdateDto
/// Todos los campos son opcionales, pero si se proporcionan deben cumplir las reglas
/// </summary>
public class PaqueteTuristicoUpdateDtoValidator : AbstractValidator<PaqueteTuristicoUpdateDto>
{
    private readonly string[] _tiposPaquetePermitidos =
    {
        "aventura", "familiar", "empresarial", "lujo",
        "cultural", "ecologico", "romantico"
    };

    private readonly string[] _nivelesDificultadPermitidos =
    {
        "bajo", "medio", "alto"
    };

    public PaqueteTuristicoUpdateDtoValidator()
    {
        // Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre no puede estar vacío si se proporciona")
            .MaximumLength(200)
            .WithMessage("El nombre no puede exceder 200 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-&'.0-9]+$")
            .WithMessage("El nombre solo puede contener letras, números, espacios y caracteres: - & ' .")
            .When(x => x.Nombre != null);

        // Detalle
        RuleFor(x => x.Detalle)
            .MaximumLength(5000)
            .WithMessage("El detalle no puede exceder 5000 caracteres")
            .When(x => x.Detalle != null);

        // Destino Principal
        RuleFor(x => x.DestinoPrincipal)
            .NotEmpty()
            .WithMessage("El destino principal no puede estar vacío si se proporciona")
            .MaximumLength(100)
            .WithMessage("El destino principal no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-',]+$")
            .WithMessage("El destino solo puede contener letras, espacios y caracteres: - ' ,")
            .When(x => x.DestinoPrincipal != null);

        // Destinos Adicionales (JSON)
        RuleFor(x => x.DestinosAdicionales)
            .Must(BeValidJson)
            .WithMessage("Los destinos adicionales deben ser un array JSON válido")
            .When(x => x.DestinosAdicionales != null);

        // Duración
        RuleFor(x => x.Duracion)
            .GreaterThan(0)
            .WithMessage("La duración debe ser mayor a 0 días")
            .LessThanOrEqualTo(365)
            .WithMessage("La duración no puede exceder 365 días")
            .When(x => x.Duracion.HasValue);

        // Precio
        RuleFor(x => x.Precio)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor a 0")
            .LessThanOrEqualTo(9999999.99m)
            .WithMessage("El precio no puede exceder 9,999,999.99")
            .When(x => x.Precio.HasValue);

        // Cupos Disponibles
        RuleFor(x => x.CuposDisponibles)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Los cupos disponibles no pueden ser negativos")
            .LessThanOrEqualTo(1000)
            .WithMessage("Los cupos disponibles no pueden exceder 1000")
            .When(x => x.CuposDisponibles.HasValue);

        // Incluye (JSON)
        RuleFor(x => x.Incluye)
            .Must(BeValidJson)
            .WithMessage("Los servicios incluidos deben ser un array JSON válido")
            .When(x => x.Incluye != null);

        // No Incluye (JSON)
        RuleFor(x => x.NoIncluye)
            .Must(BeValidJson)
            .WithMessage("Los servicios no incluidos deben ser un array JSON válido")
            .When(x => x.NoIncluye != null);

        // FechaInicio
        RuleFor(x => x.FechaInicio)
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-1))
            .WithMessage("La fecha de inicio debe ser actual o futura")
            .When(x => x.FechaInicio.HasValue);

        // FechaFin
        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio")
            .When(x => x.FechaInicio.HasValue && x.FechaFin.HasValue);

        // Tipo de Paquete
        RuleFor(x => x.TipoPaquete)
            .MaximumLength(50)
            .WithMessage("El tipo de paquete no puede exceder 50 caracteres")
            .Must(tipo => tipo == null || _tiposPaquetePermitidos.Contains(tipo.ToLower()))
            .WithMessage($"Tipo de paquete inválido. Valores permitidos: {string.Join(", ", _tiposPaquetePermitidos)}")
            .When(x => x.TipoPaquete != null);

        // Nivel de Dificultad
        RuleFor(x => x.NivelDificultad)
            .MaximumLength(20)
            .WithMessage("El nivel de dificultad no puede exceder 20 caracteres")
            .Must(nivel => nivel == null || _nivelesDificultadPermitidos.Contains(nivel.ToLower()))
            .WithMessage($"Nivel de dificultad inválido. Valores permitidos: {string.Join(", ", _nivelesDificultadPermitidos)}")
            .When(x => x.NivelDificultad != null);

        // Edad Mínima
        RuleFor(x => x.EdadMinima)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La edad mínima no puede ser negativa")
            .LessThanOrEqualTo(99)
            .WithMessage("La edad mínima no puede exceder 99 años")
            .When(x => x.EdadMinima.HasValue);

        // Número Mínimo de Personas
        RuleFor(x => x.NumeroMinimoPersonas)
            .GreaterThan(0)
            .WithMessage("El número mínimo de personas debe ser mayor a 0")
            .LessThanOrEqualTo(x => x.NumeroMaximoPersonas ?? int.MaxValue)
            .WithMessage("El número mínimo no puede ser mayor al número máximo")
            .When(x => x.NumeroMinimoPersonas.HasValue);

        // Número Máximo de Personas
        RuleFor(x => x.NumeroMaximoPersonas)
            .GreaterThan(0)
            .WithMessage("El número máximo de personas debe ser mayor a 0")
            .GreaterThanOrEqualTo(x => x.NumeroMinimoPersonas ?? 0)
            .WithMessage("El número máximo debe ser mayor o igual al número mínimo")
            .When(x => x.NumeroMaximoPersonas.HasValue);

        // Requisitos (JSON)
        RuleFor(x => x.Requisitos)
            .Must(BeValidJson)
            .WithMessage("Los requisitos deben ser un array JSON válido")
            .When(x => x.Requisitos != null);

        // Recomendaciones (JSON)
        RuleFor(x => x.Recomendaciones)
            .Must(BeValidJson)
            .WithMessage("Las recomendaciones deben ser un array JSON válido")
            .When(x => x.Recomendaciones != null);

        // Imágenes (JSON)
        RuleFor(x => x.Imagenes)
            .Must(BeValidJson)
            .WithMessage("Las imágenes deben ser un array JSON válido")
            .When(x => x.Imagenes != null);

        // Itinerario Resumido
        RuleFor(x => x.ItinerarioResumido)
            .MaximumLength(10000)
            .WithMessage("El itinerario resumido no puede exceder 10,000 caracteres")
            .When(x => x.ItinerarioResumido != null);

        // Políticas de Cancelación
        RuleFor(x => x.PoliticasCancelacion)
            .MaximumLength(2000)
            .WithMessage("Las políticas de cancelación no pueden exceder 2000 caracteres")
            .When(x => x.PoliticasCancelacion != null);
    }

    /// <summary>
    /// Valida que una cadena sea JSON válido
    /// </summary>
    private bool BeValidJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return true;

        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

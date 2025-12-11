using FluentValidation;
using G2rismBeta.API.DTOs.ServicioAdicional;
using System.Text.Json;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para ServicioAdicionalCreateDto
/// </summary>
public class ServicioAdicionalCreateDtoValidator : AbstractValidator<ServicioAdicionalCreateDto>
{
    private static readonly string[] TiposPermitidos = { "tour", "guia", "actividad", "transporte_interno" };
    private static readonly string[] UnidadesPermitidas = { "persona", "grupo", "hora", "dia" };

    public ServicioAdicionalCreateDtoValidator()
    {
        // ID Proveedor
        RuleFor(x => x.IdProveedor)
            .GreaterThan(0)
            .WithMessage("El ID del proveedor debe ser mayor a 0");

        // Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del servicio es requerido")
            .MaximumLength(200)
            .WithMessage("El nombre no puede exceder 200 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-&'.0-9]+$")
            .WithMessage("El nombre solo puede contener letras, números, espacios y caracteres: - & ' .");

        // Tipo
        RuleFor(x => x.Tipo)
            .NotEmpty()
            .WithMessage("El tipo de servicio es requerido")
            .MaximumLength(50)
            .WithMessage("El tipo no puede exceder 50 caracteres")
            .Must(tipo => TiposPermitidos.Contains(tipo.ToLower()))
            .WithMessage($"Tipo inválido. Valores permitidos: {string.Join(", ", TiposPermitidos)}");

        // Descripción
        RuleFor(x => x.Descripcion)
            .MaximumLength(5000)
            .WithMessage("La descripción no puede exceder 5000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));

        // Precio
        RuleFor(x => x.Precio)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor a 0")
            .LessThanOrEqualTo(9999999.99m)
            .WithMessage("El precio no puede exceder 9,999,999.99");

        // Unidad
        RuleFor(x => x.Unidad)
            .NotEmpty()
            .WithMessage("La unidad de medida es requerida")
            .MaximumLength(50)
            .WithMessage("La unidad no puede exceder 50 caracteres")
            .Must(unidad => UnidadesPermitidas.Contains(unidad.ToLower()))
            .WithMessage($"Unidad inválida. Valores permitidos: {string.Join(", ", UnidadesPermitidas)}");

        // Tiempo estimado (formato HH:mm o H:mm)
        RuleFor(x => x.TiempoEstimado)
            .Must(BeValidTimeFormat)
            .WithMessage("El tiempo estimado debe estar en formato 'H:mm' o 'HH:mm' (ejemplo: '2:30' para 2 horas y 30 minutos)")
            .Must(BeWithin24Hours)
            .WithMessage("El tiempo estimado no puede exceder 24 horas")
            .When(x => !string.IsNullOrEmpty(x.TiempoEstimado));

        // Ubicación
        RuleFor(x => x.Ubicacion)
            .MaximumLength(500)
            .WithMessage("La ubicación no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Ubicacion));

        // Requisitos
        RuleFor(x => x.Requisitos)
            .MaximumLength(2000)
            .WithMessage("Los requisitos no pueden exceder 2000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Requisitos));

        // Capacidad máxima
        RuleFor(x => x.CapacidadMaxima)
            .GreaterThan(0)
            .WithMessage("La capacidad máxima debe ser mayor a 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("La capacidad máxima no puede exceder 1000")
            .When(x => x.CapacidadMaxima.HasValue);

        // Edad mínima
        RuleFor(x => x.EdadMinima)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La edad mínima no puede ser negativa")
            .LessThanOrEqualTo(100)
            .WithMessage("La edad mínima no puede exceder 100 años")
            .When(x => x.EdadMinima.HasValue);

        // Idiomas disponibles (JSON)
        RuleFor(x => x.IdiomasDisponibles)
            .Must(BeValidJson)
            .WithMessage("Los idiomas deben ser un array JSON válido (ej: [\"Español\", \"Inglés\"])")
            .When(x => !string.IsNullOrEmpty(x.IdiomasDisponibles));
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

    /// <summary>
    /// Valida que el tiempo esté en formato H:mm o HH:mm (ejemplo: "2:30" o "02:30")
    /// </summary>
    private bool BeValidTimeFormat(string? tiempo)
    {
        if (string.IsNullOrEmpty(tiempo))
            return true;

        var parts = tiempo.Split(':');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out int horas) || !int.TryParse(parts[1], out int minutos))
            return false;

        return horas >= 0 && minutos >= 0 && minutos < 60;
    }

    /// <summary>
    /// Valida que el tiempo no exceda 24 horas
    /// </summary>
    private bool BeWithin24Hours(string? tiempo)
    {
        if (string.IsNullOrEmpty(tiempo))
            return true;

        var parts = tiempo.Split(':');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out int horas) || !int.TryParse(parts[1], out int minutos))
            return false;

        int totalMinutos = (horas * 60) + minutos;
        return totalMinutos <= 1440; // 24 horas = 1440 minutos
    }
}

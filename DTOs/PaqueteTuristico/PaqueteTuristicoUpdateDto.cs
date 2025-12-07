namespace G2rismBeta.API.DTOs.PaqueteTuristico;

/// <summary>
/// DTO para actualizar un paquete turístico existente
/// Todos los campos son opcionales para permitir actualizaciones parciales
/// </summary>
public class PaqueteTuristicoUpdateDto
{
    /// <summary>
    /// Nombre del paquete turístico (máx 200 caracteres)
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Detalle descriptivo del paquete
    /// </summary>
    public string? Detalle { get; set; }

    /// <summary>
    /// Destino principal del paquete (máx 100 caracteres)
    /// </summary>
    public string? DestinoPrincipal { get; set; }

    /// <summary>
    /// Destinos adicionales incluidos en formato JSON array
    /// </summary>
    public string? DestinosAdicionales { get; set; }

    /// <summary>
    /// Duración del paquete en días (debe ser mayor a 0 si se proporciona)
    /// </summary>
    public int? Duracion { get; set; }

    /// <summary>
    /// Precio total del paquete (debe ser mayor a 0 si se proporciona)
    /// </summary>
    public decimal? Precio { get; set; }

    /// <summary>
    /// Cupos disponibles para el paquete
    /// </summary>
    public int? CuposDisponibles { get; set; }

    /// <summary>
    /// Servicios incluidos en formato JSON array
    /// </summary>
    public string? Incluye { get; set; }

    /// <summary>
    /// Servicios NO incluidos en formato JSON array
    /// </summary>
    public string? NoIncluye { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete
    /// </summary>
    public DateTime? FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización del paquete
    /// </summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Tipo de paquete: aventura, familiar, empresarial, lujo, cultural, ecologico, romantico
    /// </summary>
    public string? TipoPaquete { get; set; }

    /// <summary>
    /// Nivel de dificultad: bajo, medio, alto
    /// </summary>
    public string? NivelDificultad { get; set; }

    /// <summary>
    /// Edad mínima requerida
    /// </summary>
    public int? EdadMinima { get; set; }

    /// <summary>
    /// Número mínimo de personas requeridas
    /// </summary>
    public int? NumeroMinimoPersonas { get; set; }

    /// <summary>
    /// Número máximo de personas permitidas
    /// </summary>
    public int? NumeroMaximoPersonas { get; set; }

    /// <summary>
    /// Requisitos especiales en formato JSON array
    /// </summary>
    public string? Requisitos { get; set; }

    /// <summary>
    /// Recomendaciones para el viaje en formato JSON array
    /// </summary>
    public string? Recomendaciones { get; set; }

    /// <summary>
    /// URLs de imágenes en formato JSON array
    /// </summary>
    public string? Imagenes { get; set; }

    /// <summary>
    /// Itinerario resumido del paquete
    /// </summary>
    public string? ItinerarioResumido { get; set; }

    /// <summary>
    /// Políticas de cancelación
    /// </summary>
    public string? PoliticasCancelacion { get; set; }

    /// <summary>
    /// Estado del paquete: true = activo, false = inactivo
    /// </summary>
    public bool? Estado { get; set; }
}

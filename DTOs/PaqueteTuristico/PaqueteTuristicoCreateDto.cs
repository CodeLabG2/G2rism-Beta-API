namespace G2rismBeta.API.DTOs.PaqueteTuristico;

/// <summary>
/// DTO para crear un nuevo paquete turístico
/// </summary>
public class PaqueteTuristicoCreateDto
{
    /// <summary>
    /// Nombre del paquete turístico (máx 200 caracteres)
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Detalle descriptivo del paquete (opcional)
    /// </summary>
    public string? Detalle { get; set; }

    /// <summary>
    /// Destino principal del paquete (máx 100 caracteres)
    /// </summary>
    public string DestinoPrincipal { get; set; } = string.Empty;

    /// <summary>
    /// Destinos adicionales incluidos en formato JSON array
    /// Ejemplo: ["Bogotá", "Cartagena", "Santa Marta"]
    /// </summary>
    public string? DestinosAdicionales { get; set; }

    /// <summary>
    /// Duración del paquete en días (debe ser mayor a 0)
    /// </summary>
    public int Duracion { get; set; }

    /// <summary>
    /// Precio total del paquete (debe ser mayor a 0)
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cupos disponibles para el paquete (debe ser mayor a 0)
    /// </summary>
    public int CuposDisponibles { get; set; }

    /// <summary>
    /// Servicios incluidos en el paquete en formato JSON array
    /// Ejemplo: ["Hospedaje", "Alimentación completa", "Transporte terrestre", "Guía turístico"]
    /// </summary>
    public string? Incluye { get; set; }

    /// <summary>
    /// Servicios NO incluidos en formato JSON array
    /// Ejemplo: ["Boletos aéreos", "Gastos personales", "Propinas"]
    /// </summary>
    public string? NoIncluye { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete (opcional para paquetes con fechas flexibles)
    /// </summary>
    public DateTime? FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización del paquete (opcional)
    /// </summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Tipo de paquete: aventura, familiar, empresarial, lujo, cultural, ecologico, romantico (opcional, máx 50 caracteres)
    /// </summary>
    public string? TipoPaquete { get; set; }

    /// <summary>
    /// Nivel de dificultad: bajo, medio, alto (opcional, máx 20 caracteres)
    /// </summary>
    public string? NivelDificultad { get; set; }

    /// <summary>
    /// Edad mínima requerida (opcional)
    /// </summary>
    public int? EdadMinima { get; set; }

    /// <summary>
    /// Número mínimo de personas requeridas (opcional)
    /// </summary>
    public int? NumeroMinimoPersonas { get; set; }

    /// <summary>
    /// Número máximo de personas permitidas (opcional)
    /// </summary>
    public int? NumeroMaximoPersonas { get; set; }

    /// <summary>
    /// Requisitos especiales en formato JSON array (opcional)
    /// Ejemplo: ["Pasaporte vigente", "Vacunas actualizadas", "Seguro médico"]
    /// </summary>
    public string? Requisitos { get; set; }

    /// <summary>
    /// Recomendaciones para el viaje en formato JSON array (opcional)
    /// Ejemplo: ["Ropa cómoda", "Protector solar", "Repelente de insectos"]
    /// </summary>
    public string? Recomendaciones { get; set; }

    /// <summary>
    /// URLs de imágenes en formato JSON array (opcional)
    /// Ejemplo: ["url1.jpg", "url2.jpg"]
    /// </summary>
    public string? Imagenes { get; set; }

    /// <summary>
    /// Itinerario resumido del paquete (opcional)
    /// </summary>
    public string? ItinerarioResumido { get; set; }

    /// <summary>
    /// Políticas de cancelación (opcional)
    /// </summary>
    public string? PoliticasCancelacion { get; set; }

    /// <summary>
    /// Estado del paquete: true = activo, false = inactivo (default: true)
    /// </summary>
    public bool Estado { get; set; } = true;
}

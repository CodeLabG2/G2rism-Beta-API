namespace G2rismBeta.API.DTOs.PaqueteTuristico;

/// <summary>
/// DTO para respuesta de paquete turístico
/// Incluye todas las propiedades del modelo más las computadas
/// </summary>
public class PaqueteTuristicoResponseDto
{
    /// <summary>
    /// Identificador único del paquete turístico
    /// </summary>
    public int IdPaquete { get; set; }

    /// <summary>
    /// Nombre del paquete turístico
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Detalle descriptivo del paquete
    /// </summary>
    public string? Detalle { get; set; }

    /// <summary>
    /// Destino principal del paquete
    /// </summary>
    public string DestinoPrincipal { get; set; } = string.Empty;

    /// <summary>
    /// Destinos adicionales incluidos (JSON)
    /// </summary>
    public string? DestinosAdicionales { get; set; }

    /// <summary>
    /// Duración del paquete en días
    /// </summary>
    public int Duracion { get; set; }

    /// <summary>
    /// Precio total del paquete
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cupos disponibles
    /// </summary>
    public int CuposDisponibles { get; set; }

    /// <summary>
    /// Servicios incluidos (JSON)
    /// </summary>
    public string? Incluye { get; set; }

    /// <summary>
    /// Servicios NO incluidos (JSON)
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
    /// Tipo de paquete
    /// </summary>
    public string? TipoPaquete { get; set; }

    /// <summary>
    /// Nivel de dificultad
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
    /// Requisitos especiales (JSON)
    /// </summary>
    public string? Requisitos { get; set; }

    /// <summary>
    /// Recomendaciones (JSON)
    /// </summary>
    public string? Recomendaciones { get; set; }

    /// <summary>
    /// URLs de imágenes (JSON)
    /// </summary>
    public string? Imagenes { get; set; }

    /// <summary>
    /// Itinerario resumido
    /// </summary>
    public string? ItinerarioResumido { get; set; }

    /// <summary>
    /// Políticas de cancelación
    /// </summary>
    public string? PoliticasCancelacion { get; set; }

    /// <summary>
    /// Estado del paquete
    /// </summary>
    public bool Estado { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    // ============================================
    // PROPIEDADES COMPUTADAS (del modelo)
    // ============================================

    /// <summary>
    /// Indica si el paquete está activo
    /// </summary>
    public bool EstaActivo { get; set; }

    /// <summary>
    /// Indica si el paquete está disponible
    /// </summary>
    public bool EstaDisponible { get; set; }

    /// <summary>
    /// Indica si el paquete tiene fechas definidas
    /// </summary>
    public bool TieneFechasDefinidas { get; set; }

    /// <summary>
    /// Indica si el paquete está vigente
    /// </summary>
    public bool EstaVigente { get; set; }

    /// <summary>
    /// Indica si el paquete está próximo a iniciar
    /// </summary>
    public bool ProximoAIniciar { get; set; }

    /// <summary>
    /// Nombre completo con destino
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Duración formateada (ej: "7 días / 6 noches")
    /// </summary>
    public string DuracionFormateada { get; set; } = string.Empty;

    /// <summary>
    /// Precio formateado (ej: "$1,500,000 COP")
    /// </summary>
    public string PrecioFormateado { get; set; } = string.Empty;

    /// <summary>
    /// Estado de disponibilidad como texto
    /// </summary>
    public string EstadoDisponibilidad { get; set; } = string.Empty;
}

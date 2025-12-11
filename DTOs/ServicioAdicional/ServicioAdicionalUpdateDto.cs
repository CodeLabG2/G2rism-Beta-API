namespace G2rismBeta.API.DTOs.ServicioAdicional;

/// <summary>
/// DTO para actualizar un servicio adicional existente
/// Todos los campos son opcionales para permitir actualizaciones parciales
/// </summary>
public class ServicioAdicionalUpdateDto
{
    /// <summary>
    /// ID del proveedor asociado (opcional)
    /// </summary>
    public int? IdProveedor { get; set; }

    /// <summary>
    /// Nombre del servicio (opcional, máx 200 caracteres)
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Tipo de servicio: tour, guia, actividad, transporte_interno (opcional, máx 50 caracteres)
    /// </summary>
    public string? Tipo { get; set; }

    /// <summary>
    /// Descripción detallada del servicio (opcional)
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Precio del servicio (opcional)
    /// </summary>
    public decimal? Precio { get; set; }

    /// <summary>
    /// Unidad de medida: persona, grupo, hora, dia (opcional, máx 50 caracteres)
    /// </summary>
    public string? Unidad { get; set; }

    /// <summary>
    /// Indica si el servicio está disponible (opcional)
    /// </summary>
    public bool? Disponibilidad { get; set; }

    /// <summary>
    /// Tiempo estimado del servicio en formato "HH:mm" o "H:mm" (ejemplo: "2:30" = 2 horas y 30 minutos) (opcional)
    /// </summary>
    public string? TiempoEstimado { get; set; }

    /// <summary>
    /// Ubicación o punto de encuentro del servicio (opcional, máx 500 caracteres)
    /// </summary>
    public string? Ubicacion { get; set; }

    /// <summary>
    /// Requisitos o condiciones del servicio (opcional)
    /// </summary>
    public string? Requisitos { get; set; }

    /// <summary>
    /// Capacidad máxima (opcional)
    /// </summary>
    public int? CapacidadMaxima { get; set; }

    /// <summary>
    /// Edad mínima requerida (opcional)
    /// </summary>
    public int? EdadMinima { get; set; }

    /// <summary>
    /// Idiomas disponibles en formato JSON array (opcional)
    /// </summary>
    public string? IdiomasDisponibles { get; set; }

    /// <summary>
    /// Estado del servicio (opcional)
    /// </summary>
    public bool? Estado { get; set; }
}

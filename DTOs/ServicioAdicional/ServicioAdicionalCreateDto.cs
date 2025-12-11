namespace G2rismBeta.API.DTOs.ServicioAdicional;

/// <summary>
/// DTO para crear un nuevo servicio adicional
/// </summary>
public class ServicioAdicionalCreateDto
{
    /// <summary>
    /// ID del proveedor asociado (debe existir en la tabla proveedores)
    /// </summary>
    public int IdProveedor { get; set; }

    /// <summary>
    /// Nombre del servicio (máx 200 caracteres)
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de servicio: tour, guia, actividad, transporte_interno (máx 50 caracteres)
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del servicio (opcional)
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Precio del servicio (debe ser mayor a 0)
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Unidad de medida: persona, grupo, hora, dia (máx 50 caracteres)
    /// </summary>
    public string Unidad { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el servicio está disponible (default: true)
    /// </summary>
    public bool Disponibilidad { get; set; } = true;

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
    /// Capacidad máxima (personas, grupos, etc.) (opcional)
    /// </summary>
    public int? CapacidadMaxima { get; set; }

    /// <summary>
    /// Edad mínima requerida (opcional)
    /// </summary>
    public int? EdadMinima { get; set; }

    /// <summary>
    /// Idiomas disponibles en formato JSON array (ej: ["Español", "Inglés", "Francés"])
    /// </summary>
    public string? IdiomasDisponibles { get; set; }

    /// <summary>
    /// Estado del servicio: true = activo, false = inactivo (default: true)
    /// </summary>
    public bool Estado { get; set; } = true;
}

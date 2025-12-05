namespace G2rismBeta.API.DTOs.Vuelo;

/// <summary>
/// DTO de respuesta para vuelo con información completa
/// </summary>
public class VueloResponseDto
{
    /// <summary>
    /// ID del vuelo
    /// </summary>
    public int IdVuelo { get; set; }

    /// <summary>
    /// ID de la aerolínea
    /// </summary>
    public int IdAerolinea { get; set; }

    /// <summary>
    /// Nombre de la aerolínea
    /// </summary>
    public string? NombreAerolinea { get; set; }

    /// <summary>
    /// Código IATA de la aerolínea
    /// </summary>
    public string? CodigoIataAerolinea { get; set; }

    /// <summary>
    /// ID del proveedor
    /// </summary>
    public int IdProveedor { get; set; }

    /// <summary>
    /// Nombre del proveedor
    /// </summary>
    public string? NombreProveedor { get; set; }

    /// <summary>
    /// Número de vuelo (ej: AA1234)
    /// </summary>
    public string NumeroVuelo { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad o aeropuerto de origen
    /// </summary>
    public string Origen { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad o aeropuerto de destino
    /// </summary>
    public string Destino { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de salida del vuelo
    /// </summary>
    public DateTime FechaSalida { get; set; }

    /// <summary>
    /// Fecha de llegada del vuelo
    /// </summary>
    public DateTime FechaLlegada { get; set; }

    /// <summary>
    /// Hora de salida (formato HH:mm:ss)
    /// </summary>
    public TimeSpan HoraSalida { get; set; }

    /// <summary>
    /// Hora de llegada (formato HH:mm:ss)
    /// </summary>
    public TimeSpan HoraLlegada { get; set; }

    /// <summary>
    /// Cupos disponibles
    /// </summary>
    public int CuposDisponibles { get; set; }

    /// <summary>
    /// Cupos totales del vuelo
    /// </summary>
    public int CuposTotales { get; set; }

    /// <summary>
    /// Precio clase económica
    /// </summary>
    public decimal PrecioEconomica { get; set; }

    /// <summary>
    /// Precio clase ejecutiva (opcional)
    /// </summary>
    public decimal? PrecioEjecutiva { get; set; }

    /// <summary>
    /// Duración del vuelo en minutos
    /// </summary>
    public int DuracionMinutos { get; set; }

    /// <summary>
    /// Duración formateada (ej: "2h 30m")
    /// </summary>
    public string DuracionFormateada { get; set; } = string.Empty;

    /// <summary>
    /// Número de escalas (0 = vuelo directo)
    /// </summary>
    public int Escalas { get; set; }

    /// <summary>
    /// Indica si es vuelo directo
    /// </summary>
    public bool EsVueloDirecto { get; set; }

    /// <summary>
    /// Indica si tiene disponibilidad
    /// </summary>
    public bool TieneDisponibilidad { get; set; }

    /// <summary>
    /// Estado del vuelo
    /// </summary>
    public bool Estado { get; set; }

    /// <summary>
    /// Indica si el vuelo está activo (estado + fecha futura)
    /// </summary>
    public bool EstaActivo { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    public string? Observaciones { get; set; }
}

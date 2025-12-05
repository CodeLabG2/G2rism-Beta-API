namespace G2rismBeta.API.DTOs.Vuelo;

/// <summary>
/// DTO para crear un nuevo vuelo
/// </summary>
public class VueloCreateDto
{
    /// <summary>
    /// ID de la aerolínea
    /// </summary>
    public int IdAerolinea { get; set; }

    /// <summary>
    /// ID del proveedor
    /// </summary>
    public int IdProveedor { get; set; }

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
    /// Número de escalas (0 = vuelo directo)
    /// </summary>
    public int Escalas { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    public string? Observaciones { get; set; }
}

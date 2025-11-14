namespace G2rismBeta.API.DTOs.Aerolinea;

/// <summary>
/// DTO de respuesta para aerolínea con toda la información
/// </summary>
public class AerolineaResponseDto
{
    /// <summary>
    /// Identificador único de la aerolínea
    /// </summary>
    public int IdAerolinea { get; set; }

    /// <summary>
    /// Nombre oficial de la aerolínea
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código IATA de 2 caracteres
    /// </summary>
    public string CodigoIata { get; set; } = string.Empty;

    /// <summary>
    /// Código ICAO de 3 caracteres
    /// </summary>
    public string CodigoIcao { get; set; } = string.Empty;

    /// <summary>
    /// País de origen
    /// </summary>
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Sitio web oficial
    /// </summary>
    public string? SitioWeb { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string TelefonoContacto { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto
    /// </summary>
    public string EmailContacto { get; set; } = string.Empty;

    /// <summary>
    /// Políticas de equipaje
    /// </summary>
    public string? PoliticasEquipaje { get; set; }

    /// <summary>
    /// Estado de la aerolínea
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    // ===================================
    // PROPIEDADES CALCULADAS
    // ===================================

    /// <summary>
    /// Indica si la aerolínea está activa
    /// </summary>
    public bool EstaActiva { get; set; }

    /// <summary>
    /// Nombre completo con código IATA
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Indica si tiene políticas de equipaje definidas
    /// </summary>
    public bool TienePoliticasEquipaje { get; set; }
}

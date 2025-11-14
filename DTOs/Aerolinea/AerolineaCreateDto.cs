namespace G2rismBeta.API.DTOs.Aerolinea;

/// <summary>
/// DTO para crear una nueva aerolínea
/// </summary>
public class AerolineaCreateDto
{
    /// <summary>
    /// Nombre oficial de la aerolínea
    /// </summary>
    /// <example>Avianca</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código IATA de 2 caracteres (ej: AV)
    /// </summary>
    /// <example>AV</example>
    public string CodigoIata { get; set; } = string.Empty;

    /// <summary>
    /// Código ICAO de 3 caracteres (ej: AVA)
    /// </summary>
    /// <example>AVA</example>
    public string CodigoIcao { get; set; } = string.Empty;

    /// <summary>
    /// País de origen de la aerolínea
    /// </summary>
    /// <example>Colombia</example>
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Sitio web oficial (opcional)
    /// </summary>
    /// <example>https://www.avianca.com</example>
    public string? SitioWeb { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    /// <example>+57 1 401 3434</example>
    public string TelefonoContacto { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto
    /// </summary>
    /// <example>contacto@avianca.com</example>
    public string EmailContacto { get; set; } = string.Empty;

    /// <summary>
    /// Políticas de equipaje (opcional)
    /// </summary>
    /// <example>Equipaje de mano: 10kg. Equipaje facturado: 23kg en clase económica, 32kg en ejecutiva.</example>
    public string? PoliticasEquipaje { get; set; }

    /// <summary>
    /// Estado inicial (por defecto: Activa)
    /// </summary>
    /// <example>Activa</example>
    public string Estado { get; set; } = "Activa";
}

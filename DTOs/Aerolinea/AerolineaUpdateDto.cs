namespace G2rismBeta.API.DTOs.Aerolinea;

/// <summary>
/// DTO para actualizar una aerolínea existente
/// </summary>
public class AerolineaUpdateDto
{
    /// <summary>
    /// Nombre oficial de la aerolínea
    /// </summary>
    /// <example>Avianca S.A.</example>
    public string? Nombre { get; set; }

    /// <summary>
    /// País de origen de la aerolínea
    /// </summary>
    /// <example>Colombia</example>
    public string? Pais { get; set; }

    /// <summary>
    /// Sitio web oficial
    /// </summary>
    /// <example>https://www.avianca.com</example>
    public string? SitioWeb { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    /// <example>+57 1 401 3434</example>
    public string? TelefonoContacto { get; set; }

    /// <summary>
    /// Email de contacto
    /// </summary>
    /// <example>contacto@avianca.com</example>
    public string? EmailContacto { get; set; }

    /// <summary>
    /// Políticas de equipaje
    /// </summary>
    /// <example>Equipaje de mano: 10kg. Equipaje facturado: 23kg en clase económica, 32kg en ejecutiva.</example>
    public string? PoliticasEquipaje { get; set; }

    /// <summary>
    /// Estado de la aerolínea (Activa/Inactiva)
    /// </summary>
    /// <example>Activa</example>
    public string? Estado { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Representa una aerolínea que opera vuelos en el sistema
/// </summary>
[Table("aerolineas")]
public class Aerolinea
{
    /// <summary>
    /// Identificador único de la aerolínea
    /// </summary>
    [Key]
    [Column("id_aerolinea")]
    public int IdAerolinea { get; set; }

    /// <summary>
    /// Nombre oficial de la aerolínea
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código IATA de 2 caracteres (ej: AA, LA, AV)
    /// </summary>
    [Required(ErrorMessage = "El código IATA es obligatorio")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "El código IATA debe tener exactamente 2 caracteres")]
    [Column("codigo_iata")]
    public string CodigoIata { get; set; } = string.Empty;

    /// <summary>
    /// Código ICAO de 3 caracteres (ej: AAL, LAN, AVA)
    /// </summary>
    [Required(ErrorMessage = "El código ICAO es obligatorio")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código ICAO debe tener exactamente 3 caracteres")]
    [Column("codigo_icao")]
    public string CodigoIcao { get; set; } = string.Empty;

    /// <summary>
    /// País de origen de la aerolínea
    /// </summary>
    [Required(ErrorMessage = "El país es obligatorio")]
    [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
    [Column("pais")]
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Sitio web oficial de la aerolínea
    /// </summary>
    [StringLength(200, ErrorMessage = "El sitio web no puede exceder 200 caracteres")]
    [Column("sitio_web")]
    public string? SitioWeb { get; set; }

    /// <summary>
    /// Teléfono de contacto de la aerolínea
    /// </summary>
    [Required(ErrorMessage = "El teléfono de contacto es obligatorio")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Column("telefono_contacto")]
    public string TelefonoContacto { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto de la aerolínea
    /// </summary>
    [Required(ErrorMessage = "El email de contacto es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    [Column("email_contacto")]
    public string EmailContacto { get; set; } = string.Empty;

    /// <summary>
    /// Políticas de equipaje de la aerolínea (formato texto)
    /// </summary>
    [Column("politicas_equipaje", TypeName = "TEXT")]
    public string? PoliticasEquipaje { get; set; }

    /// <summary>
    /// Estado de la aerolínea (Activa/Inactiva)
    /// </summary>
    [Required]
    [StringLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Activa";

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de última modificación del registro
    /// </summary>
    [Column("fecha_modificacion")]
    public DateTime? FechaModificacion { get; set; }

    // ===================================
    // RELACIONES
    // ===================================

    /// <summary>
    /// Vuelos operados por esta aerolínea
    /// </summary>
    public virtual ICollection<Vuelo>? Vuelos { get; set; }

    // ===================================
    // PROPIEDADES CALCULADAS (No mapeadas a BD)
    // ===================================

    /// <summary>
    /// Indica si la aerolínea está activa
    /// </summary>
    [NotMapped]
    public bool EstaActiva => Estado?.ToLower() == "activa";

    /// <summary>
    /// Nombre completo con código IATA
    /// </summary>
    [NotMapped]
    public string NombreCompleto => $"{Nombre} ({CodigoIata})";

    /// <summary>
    /// Indica si tiene políticas de equipaje definidas
    /// </summary>
    [NotMapped]
    public bool TienePoliticasEquipaje => !string.IsNullOrWhiteSpace(PoliticasEquipaje);
}

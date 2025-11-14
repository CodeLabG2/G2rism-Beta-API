using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Modelo temporal de Vuelo (se completará en Fase 3)
/// </summary>
[Table("vuelos")]
public class Vuelo
{
    [Key]
    [Column("id_vuelo")]
    public int IdVuelo { get; set; }

    [Column("id_aerolinea")]
    public int IdAerolinea { get; set; }

    // Relaciones
    public virtual Aerolinea? Aerolinea { get; set; }
}

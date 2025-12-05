using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Models;

/// <summary>
/// Modelo de Vuelo - Sistema de reservas de vuelos
/// </summary>
[Table("vuelos")]
public class Vuelo
{
    [Key]
    [Column("id_vuelo")]
    public int IdVuelo { get; set; }

    [Column("id_aerolinea")]
    public int IdAerolinea { get; set; }

    [Column("id_proveedor")]
    public int IdProveedor { get; set; }

    [Required]
    [Column("numero_vuelo")]
    [MaxLength(10)]
    public string NumeroVuelo { get; set; } = string.Empty;

    [Required]
    [Column("origen")]
    [MaxLength(100)]
    public string Origen { get; set; } = string.Empty;

    [Required]
    [Column("destino")]
    [MaxLength(100)]
    public string Destino { get; set; } = string.Empty;

    [Column("fecha_salida")]
    public DateTime FechaSalida { get; set; }

    [Column("fecha_llegada")]
    public DateTime FechaLlegada { get; set; }

    [Column("hora_salida")]
    public TimeSpan HoraSalida { get; set; }

    [Column("hora_llegada")]
    public TimeSpan HoraLlegada { get; set; }

    [Column("cupos_disponibles")]
    public int CuposDisponibles { get; set; }

    [Column("cupos_totales")]
    public int CuposTotales { get; set; }

    [Column("precio_economica")]
    [Precision(10, 2)]
    public decimal PrecioEconomica { get; set; }

    [Column("precio_ejecutiva")]
    [Precision(10, 2)]
    public decimal? PrecioEjecutiva { get; set; }

    [Column("duracion_minutos")]
    public int DuracionMinutos { get; set; }

    [Column("escalas")]
    public int Escalas { get; set; }

    [Column("estado")]
    public bool Estado { get; set; } = true;

    [Column("observaciones")]
    [MaxLength(500)]
    public string? Observaciones { get; set; }

    // Propiedades computadas
    [NotMapped]
    public bool TieneDisponibilidad => CuposDisponibles > 0;

    [NotMapped]
    public bool EsVueloDirecto => Escalas == 0;

    [NotMapped]
    public string DuracionFormateada => $"{DuracionMinutos / 60}h {DuracionMinutos % 60}m";

    [NotMapped]
    public bool EstaActivo => Estado && FechaSalida > DateTime.Now;

    // Relaciones
    public virtual Aerolinea? Aerolinea { get; set; }
    public virtual Proveedor? Proveedor { get; set; }
}

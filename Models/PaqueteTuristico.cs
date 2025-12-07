using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Representa un paquete turístico ofrecido en el sistema
/// (VERSIÓN SIMPLIFICADA MVP - Sin itinerarios detallados)
/// </summary>
[Table("paquetes_turisticos")]
public class PaqueteTuristico
{
    /// <summary>
    /// Identificador único del paquete turístico
    /// </summary>
    [Key]
    [Column("id_paquete")]
    public int IdPaquete { get; set; }

    /// <summary>
    /// Nombre del paquete turístico
    /// </summary>
    [Required]
    [StringLength(200)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Detalle descriptivo del paquete
    /// </summary>
    [Column("detalle", TypeName = "text")]
    public string? Detalle { get; set; }

    /// <summary>
    /// Destino principal del paquete
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("destino_principal")]
    public string DestinoPrincipal { get; set; } = string.Empty;

    /// <summary>
    /// Destinos adicionales incluidos (JSON array)
    /// </summary>
    [Column("destinos_adicionales", TypeName = "json")]
    public string? DestinosAdicionales { get; set; }

    /// <summary>
    /// Duración del paquete en días
    /// </summary>
    [Required]
    [Column("duracion")]
    public int Duracion { get; set; }

    /// <summary>
    /// Precio total del paquete
    /// </summary>
    [Required]
    [Column("precio", TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Cupos disponibles para el paquete
    /// </summary>
    [Required]
    [Column("cupos_disponibles")]
    public int CuposDisponibles { get; set; }

    /// <summary>
    /// Servicios incluidos en el paquete (JSON array)
    /// Ejemplo: ["Hospedaje", "Alimentación completa", "Transporte terrestre", "Guía turístico"]
    /// </summary>
    [Column("incluye", TypeName = "json")]
    public string? Incluye { get; set; }

    /// <summary>
    /// Servicios NO incluidos en el paquete (JSON array)
    /// Ejemplo: ["Boletos aéreos", "Gastos personales", "Propinas"]
    /// </summary>
    [Column("no_incluye", TypeName = "json")]
    public string? NoIncluye { get; set; }

    /// <summary>
    /// Fecha de inicio del paquete (opcional para paquetes con fechas flexibles)
    /// </summary>
    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización del paquete
    /// </summary>
    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Tipo de paquete: aventura, familiar, empresarial, lujo, cultural, ecologico, romantico
    /// </summary>
    [StringLength(50)]
    [Column("tipo_paquete")]
    public string? TipoPaquete { get; set; }

    /// <summary>
    /// Nivel de dificultad: bajo, medio, alto
    /// </summary>
    [StringLength(20)]
    [Column("nivel_dificultad")]
    public string? NivelDificultad { get; set; }

    /// <summary>
    /// Edad mínima requerida para el paquete
    /// </summary>
    [Column("edad_minima")]
    public int? EdadMinima { get; set; }

    /// <summary>
    /// Número mínimo de personas requeridas
    /// </summary>
    [Column("numero_minimo_personas")]
    public int? NumeroMinimoPersonas { get; set; }

    /// <summary>
    /// Número máximo de personas permitidas
    /// </summary>
    [Column("numero_maximo_personas")]
    public int? NumeroMaximoPersonas { get; set; }

    /// <summary>
    /// Requisitos especiales (JSON array)
    /// Ejemplo: ["Pasaporte vigente", "Vacunas actualizadas", "Seguro médico"]
    /// </summary>
    [Column("requisitos", TypeName = "json")]
    public string? Requisitos { get; set; }

    /// <summary>
    /// Recomendaciones para el viaje (JSON array)
    /// </summary>
    [Column("recomendaciones", TypeName = "json")]
    public string? Recomendaciones { get; set; }

    /// <summary>
    /// URLs de imágenes del paquete (JSON array)
    /// </summary>
    [Column("imagenes", TypeName = "json")]
    public string? Imagenes { get; set; }

    /// <summary>
    /// Itinerario resumido del paquete (texto libre)
    /// </summary>
    [Column("itinerario_resumido", TypeName = "text")]
    public string? ItinerarioResumido { get; set; }

    /// <summary>
    /// Políticas de cancelación
    /// </summary>
    [Column("politicas_cancelacion", TypeName = "text")]
    public string? PoliticasCancelacion { get; set; }

    /// <summary>
    /// Estado del paquete (activo/inactivo)
    /// </summary>
    [Required]
    [Column("estado")]
    public bool Estado { get; set; } = true;

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

    // ============================================
    // PROPIEDADES COMPUTADAS
    // ============================================

    /// <summary>
    /// Indica si el paquete está activo
    /// </summary>
    [NotMapped]
    public bool EstaActivo => Estado;

    /// <summary>
    /// Indica si el paquete está disponible (tiene cupos y está activo)
    /// </summary>
    [NotMapped]
    public bool EstaDisponible => Estado && CuposDisponibles > 0;

    /// <summary>
    /// Indica si el paquete tiene fechas definidas
    /// </summary>
    [NotMapped]
    public bool TieneFechasDefinidas => FechaInicio.HasValue && FechaFin.HasValue;

    /// <summary>
    /// Indica si el paquete está vigente (dentro del rango de fechas)
    /// </summary>
    [NotMapped]
    public bool EstaVigente
    {
        get
        {
            if (!TieneFechasDefinidas)
                return true; // Paquetes sin fechas siempre están vigentes

            var ahora = DateTime.Now;
            return ahora >= FechaInicio && ahora <= FechaFin;
        }
    }

    /// <summary>
    /// Indica si el paquete está próximo a iniciar (menos de 7 días)
    /// </summary>
    [NotMapped]
    public bool ProximoAIniciar
    {
        get
        {
            if (!FechaInicio.HasValue)
                return false;

            var diasRestantes = (FechaInicio.Value - DateTime.Now).Days;
            return diasRestantes > 0 && diasRestantes <= 7;
        }
    }

    /// <summary>
    /// Nombre completo con destino
    /// </summary>
    [NotMapped]
    public string NombreCompleto => $"{Nombre} - {DestinoPrincipal}";

    /// <summary>
    /// Duración formateada
    /// </summary>
    [NotMapped]
    public string DuracionFormateada
    {
        get
        {
            if (Duracion == 1)
                return "1 día";

            return $"{Duracion} días / {Duracion - 1} noches";
        }
    }

    /// <summary>
    /// Precio por persona formateado
    /// </summary>
    [NotMapped]
    public string PrecioFormateado => $"${Precio:N0} COP";

    /// <summary>
    /// Estado de disponibilidad como texto
    /// </summary>
    [NotMapped]
    public string EstadoDisponibilidad
    {
        get
        {
            if (!Estado)
                return "Inactivo";

            if (CuposDisponibles == 0)
                return "Agotado";

            if (CuposDisponibles <= 5)
                return $"Últimos {CuposDisponibles} cupos";

            return "Disponible";
        }
    }
}

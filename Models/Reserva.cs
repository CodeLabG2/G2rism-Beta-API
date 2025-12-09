using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2rismBeta.API.Models;

/// <summary>
/// Entidad que representa una Reserva en el sistema de turismo.
/// Es la tabla principal del módulo de reservas, relaciona clientes, empleados y servicios.
/// </summary>
[Table("reservas")]
public class Reserva
{
    #region Primary Key

    /// <summary>
    /// Identificador único de la reserva
    /// </summary>
    [Key]
    [Column("id_reserva")]
    public int IdReserva { get; set; }

    #endregion

    #region Foreign Keys

    /// <summary>
    /// ID del cliente que realiza la reserva
    /// </summary>
    [Required(ErrorMessage = "El cliente es obligatorio")]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    /// <summary>
    /// ID del empleado que gestiona la reserva
    /// </summary>
    [Required(ErrorMessage = "El empleado es obligatorio")]
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    #endregion

    #region Basic Information

    /// <summary>
    /// Descripción general de la reserva
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Fecha de inicio del viaje
    /// </summary>
    [Required(ErrorMessage = "La fecha de inicio del viaje es obligatoria")]
    [Column("fecha_inicio_viaje", TypeName = "DATE")]
    public DateTime FechaInicioViaje { get; set; }

    /// <summary>
    /// Fecha de fin del viaje
    /// </summary>
    [Required(ErrorMessage = "La fecha de fin del viaje es obligatoria")]
    [Column("fecha_fin_viaje", TypeName = "DATE")]
    public DateTime FechaFinViaje { get; set; }

    /// <summary>
    /// Número total de pasajeros en la reserva
    /// </summary>
    [Required(ErrorMessage = "El número de pasajeros es obligatorio")]
    [Range(1, 100, ErrorMessage = "El número de pasajeros debe estar entre 1 y 100")]
    [Column("numero_pasajeros")]
    public int NumeroPasajeros { get; set; }

    #endregion

    #region Financial Information

    /// <summary>
    /// Monto total de la reserva (incluye todos los servicios)
    /// Se calcula automáticamente al agregar servicios
    /// </summary>
    [Required]
    [Column("monto_total", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El monto total debe ser positivo")]
    public decimal MontoTotal { get; set; } = 0;

    /// <summary>
    /// Monto pagado hasta el momento
    /// </summary>
    [Required]
    [Column("monto_pagado", TypeName = "DECIMAL(10,2)")]
    [Range(0, 999999999.99, ErrorMessage = "El monto pagado debe ser positivo")]
    public decimal MontoPagado { get; set; } = 0;

    /// <summary>
    /// Saldo pendiente de pago
    /// Se calcula automáticamente: MontoTotal - MontoPagado
    /// </summary>
    [Required]
    [Column("saldo_pendiente", TypeName = "DECIMAL(10,2)")]
    public decimal SaldoPendiente { get; set; } = 0;

    #endregion

    #region Status and Observations

    /// <summary>
    /// Estado actual de la reserva
    /// Valores: pendiente, confirmada, cancelada, completada
    /// </summary>
    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
    [Column("estado")]
    public string Estado { get; set; } = "pendiente";

    /// <summary>
    /// Observaciones adicionales sobre la reserva
    /// </summary>
    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    #endregion

    #region Audit Fields

    /// <summary>
    /// Fecha y hora de creación de la reserva
    /// </summary>
    [Required]
    [Column("fecha_hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha y hora de la última modificación de la reserva
    /// </summary>
    [Column("fecha_modificacion")]
    public DateTime? FechaModificacion { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Cliente asociado a la reserva (relación N:1)
    /// </summary>
    [ForeignKey("IdCliente")]
    public virtual Cliente? Cliente { get; set; }

    /// <summary>
    /// Empleado que gestiona la reserva (relación N:1)
    /// </summary>
    [ForeignKey("IdEmpleado")]
    public virtual Empleado? Empleado { get; set; }

    /// <summary>
    /// Hoteles incluidos en la reserva (relación 1:N)
    /// Una reserva puede tener múltiples hoteles
    /// </summary>
    public virtual ICollection<ReservaHotel> ReservasHoteles { get; set; } = new List<ReservaHotel>();

    // /// <summary>
    // /// Vuelos incluidos en la reserva (relación 1:N)
    // /// Una reserva puede tener múltiples vuelos
    // /// </summary>
    // public virtual ICollection<ReservaVuelo> ReservasVuelos { get; set; } = new List<ReservaVuelo>();

    // /// <summary>
    // /// Paquetes turísticos incluidos en la reserva (relación 1:N)
    // /// Una reserva puede tener múltiples paquetes
    // /// </summary>
    // public virtual ICollection<ReservaPaquete> ReservasPaquetes { get; set; } = new List<ReservaPaquete>();

    // /// <summary>
    // /// Servicios adicionales incluidos en la reserva (relación 1:N)
    // /// Una reserva puede tener múltiples servicios adicionales
    // /// </summary>
    // public virtual ICollection<ReservaServicio> ReservasServicios { get; set; } = new List<ReservaServicio>();

    // TODO: Agregar en Día 5
    // public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    #endregion

    #region Computed Properties

    /// <summary>
    /// Duración del viaje en días (calculado)
    /// </summary>
    [NotMapped]
    public int DuracionDias
    {
        get
        {
            var dias = (FechaFinViaje - FechaInicioViaje).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    /// <summary>
    /// Indica si la reserva está completamente pagada
    /// </summary>
    [NotMapped]
    public bool EstaPagada => MontoPagado >= MontoTotal && MontoTotal > 0;

    /// <summary>
    /// Indica si la reserva tiene saldo pendiente
    /// </summary>
    [NotMapped]
    public bool TieneSaldoPendiente => SaldoPendiente > 0;

    /// <summary>
    /// Porcentaje pagado de la reserva (0-100)
    /// </summary>
    [NotMapped]
    public decimal PorcentajePagado
    {
        get
        {
            if (MontoTotal == 0) return 0;
            return Math.Round((MontoPagado / MontoTotal) * 100, 2);
        }
    }

    /// <summary>
    /// Indica si el viaje ya comenzó
    /// </summary>
    [NotMapped]
    public bool ViajeIniciado => DateTime.Today >= FechaInicioViaje;

    /// <summary>
    /// Indica si el viaje ya finalizó
    /// </summary>
    [NotMapped]
    public bool ViajeCompleto => DateTime.Today > FechaFinViaje;

    /// <summary>
    /// Días restantes hasta el inicio del viaje
    /// </summary>
    [NotMapped]
    public int DiasHastaViaje
    {
        get
        {
            var dias = (FechaInicioViaje - DateTime.Today).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    #endregion
}

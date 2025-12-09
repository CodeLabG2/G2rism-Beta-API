using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del repositorio para la gestión de paquetes en reservas
/// </summary>
public interface IReservaPaqueteRepository : IGenericRepository<ReservaPaquete>
{
    /// <summary>
    /// Obtiene todos los paquetes de una reserva específica
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de paquetes de la reserva</returns>
    Task<IEnumerable<ReservaPaquete>> GetByReservaIdAsync(int idReserva);

    /// <summary>
    /// Obtiene una relación reserva-paquete con todos sus datos relacionados
    /// </summary>
    /// <param name="id">ID de la relación</param>
    /// <returns>Relación con datos del paquete incluidos</returns>
    Task<ReservaPaquete?> GetByIdWithDetailsAsync(int id);

    /// <summary>
    /// Obtiene todos los paquetes de una reserva con detalles completos
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de paquetes con detalles</returns>
    Task<IEnumerable<ReservaPaquete>> GetByReservaIdWithDetailsAsync(int idReserva);

    /// <summary>
    /// Verifica si existe una relación específica entre reserva y paquete
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="idPaquete">ID del paquete</param>
    /// <returns>True si existe la relación</returns>
    Task<bool> ExistsReservaPaqueteAsync(int idReserva, int idPaquete);

    /// <summary>
    /// Calcula el subtotal de todos los paquetes de una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Suma de todos los subtotales de paquetes</returns>
    Task<decimal> GetTotalPaquetesByReservaAsync(int idReserva);
}

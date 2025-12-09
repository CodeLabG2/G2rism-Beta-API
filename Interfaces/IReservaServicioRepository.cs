using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el repositorio de ReservaServicio
/// Define las operaciones de acceso a datos para la relación Reservas-Servicios
/// </summary>
public interface IReservaServicioRepository : IGenericRepository<ReservaServicio>
{
    /// <summary>
    /// Obtiene todos los servicios de una reserva específica con información del servicio
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de servicios de la reserva</returns>
    Task<IEnumerable<ReservaServicio>> GetServiciosByReservaIdAsync(int idReserva);

    /// <summary>
    /// Obtiene una reserva de servicio específica con información completa del servicio
    /// </summary>
    /// <param name="id">ID de la reserva de servicio</param>
    /// <returns>ReservaServicio con información completa</returns>
    Task<ReservaServicio?> GetReservaServicioConDetallesAsync(int id);

    /// <summary>
    /// Verifica si existe una reserva de servicio
    /// </summary>
    /// <param name="id">ID de la reserva de servicio</param>
    /// <returns>True si existe</returns>
    Task<bool> ExisteReservaServicioAsync(int id);

    /// <summary>
    /// Cuenta cuántos servicios tiene una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Cantidad de servicios</returns>
    Task<int> ContarServiciosPorReservaAsync(int idReserva);

    /// <summary>
    /// Obtiene servicios de una reserva por estado
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="estado">Estado a filtrar</param>
    /// <returns>Lista de servicios filtrados</returns>
    Task<IEnumerable<ReservaServicio>> GetServiciosByReservaYEstadoAsync(int idReserva, string estado);
}
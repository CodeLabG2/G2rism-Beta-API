using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del repositorio de ReservaHotel con métodos personalizados
/// </summary>
public interface IReservaHotelRepository : IGenericRepository<ReservaHotel>
{
    /// <summary>
    /// Obtiene todos los hoteles asociados a una reserva específica
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de hoteles de la reserva con información completa</returns>
    Task<IEnumerable<ReservaHotel>> GetHotelesByReservaAsync(int idReserva);

    /// <summary>
    /// Obtiene una relación ReservaHotel específica con datos completos del hotel
    /// </summary>
    /// <param name="id">ID de la relación ReservaHotel</param>
    /// <returns>ReservaHotel con datos del hotel incluidos</returns>
    Task<ReservaHotel?> GetByIdConHotelAsync(int id);

    /// <summary>
    /// Verifica si un hotel ya está agregado a una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="idHotel">ID del hotel</param>
    /// <returns>True si el hotel ya está en la reserva</returns>
    Task<bool> ExisteHotelEnReservaAsync(int idReserva, int idHotel);

    /// <summary>
    /// Obtiene todas las reservas de un hotel específico en un rango de fechas
    /// </summary>
    /// <param name="idHotel">ID del hotel</param>
    /// <param name="fechaInicio">Fecha de inicio del rango</param>
    /// <param name="fechaFin">Fecha de fin del rango</param>
    /// <returns>Lista de reservas del hotel en ese período</returns>
    Task<IEnumerable<ReservaHotel>> GetReservasPorHotelYFechasAsync(int idHotel, DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Calcula el total de hoteles en una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Suma de subtotales de todos los hoteles</returns>
    Task<decimal> CalcularTotalHotelesPorReservaAsync(int idReserva);
}

using G2rismBeta.API.DTOs.ReservaHotel;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del servicio de gestión de relaciones Reserva-Hotel
/// </summary>
public interface IReservaHotelService
{
    /// <summary>
    /// Agrega un hotel a una reserva existente
    /// Calcula automáticamente el subtotal y actualiza el monto total de la reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="dto">Datos del hotel a agregar</param>
    /// <returns>DTO de respuesta con la información del hotel agregado</returns>
    Task<ReservaHotelResponseDto> AgregarHotelAReservaAsync(int idReserva, ReservaHotelCreateDto dto);

    /// <summary>
    /// Obtiene todos los hoteles de una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de hoteles con información completa</returns>
    Task<IEnumerable<ReservaHotelResponseDto>> ObtenerHotelesPorReservaAsync(int idReserva);

    /// <summary>
    /// Obtiene la información detallada de un hotel específico en una reserva
    /// </summary>
    /// <param name="id">ID de la relación ReservaHotel</param>
    /// <returns>Información detallada del hotel en la reserva</returns>
    Task<ReservaHotelResponseDto> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Elimina un hotel de una reserva
    /// Actualiza automáticamente el monto total de la reserva
    /// </summary>
    /// <param name="id">ID de la relación ReservaHotel</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> EliminarHotelDeReservaAsync(int id);

    /// <summary>
    /// Verifica la disponibilidad de un hotel en fechas específicas
    /// </summary>
    /// <param name="idHotel">ID del hotel</param>
    /// <param name="fechaCheckin">Fecha de check-in deseada</param>
    /// <param name="fechaCheckout">Fecha de check-out deseada</param>
    /// <returns>True si el hotel tiene disponibilidad</returns>
    Task<bool> VerificarDisponibilidadAsync(int idHotel, DateTime fechaCheckin, DateTime fechaCheckout);
}
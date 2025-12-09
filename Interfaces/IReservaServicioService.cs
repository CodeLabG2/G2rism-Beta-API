using G2rismBeta.API.DTOs.ReservaServicio;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el servicio de ReservaServicio
/// Define la lógica de negocio para la gestión de servicios adicionales en reservas
/// </summary>
public interface IReservaServicioService
{
    /// <summary>
    /// Agrega un servicio adicional a una reserva existente
    /// </summary>
    /// <param name="dto">Datos del servicio a agregar</param>
    /// <returns>ReservaServicioResponseDto con el servicio agregado</returns>
    Task<ReservaServicioResponseDto> AgregarServicioAReservaAsync(ReservaServicioCreateDto dto);

    /// <summary>
    /// Obtiene todos los servicios de una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de servicios de la reserva</returns>
    Task<IEnumerable<ReservaServicioResponseDto>> GetServiciosPorReservaAsync(int idReserva);

    /// <summary>
    /// Obtiene un servicio específico de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva de servicio</param>
    /// <returns>Detalles del servicio en la reserva</returns>
    Task<ReservaServicioResponseDto> GetReservaServicioPorIdAsync(int id);

    /// <summary>
    /// Elimina un servicio de una reserva
    /// </summary>
    /// <param name="id">ID de la reserva de servicio</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> EliminarServicioDeReservaAsync(int id);

    /// <summary>
    /// Obtiene servicios de una reserva filtrados por estado
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="estado">Estado a filtrar</param>
    /// <returns>Lista de servicios filtrados</returns>
    Task<IEnumerable<ReservaServicioResponseDto>> GetServiciosPorReservaYEstadoAsync(int idReserva, string estado);
}
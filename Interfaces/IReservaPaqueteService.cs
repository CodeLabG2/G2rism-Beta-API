using G2rismBeta.API.DTOs.ReservaPaquete;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del servicio para gestionar paquetes turísticos en reservas
/// </summary>
public interface IReservaPaqueteService
{
    /// <summary>
    /// Agrega un paquete turístico a una reserva existente
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="createDto">Datos del paquete a agregar</param>
    /// <returns>Paquete agregado con todos sus detalles</returns>
    Task<ReservaPaqueteResponseDto> AgregarPaqueteAReservaAsync(int idReserva, ReservaPaqueteCreateDto createDto);

    /// <summary>
    /// Obtiene todos los paquetes de una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <returns>Lista de paquetes de la reserva</returns>
    Task<IEnumerable<ReservaPaqueteResponseDto>> ObtenerPaquetesPorReservaAsync(int idReserva);

    /// <summary>
    /// Obtiene un paquete específico de una reserva
    /// </summary>
    /// <param name="id">ID de la relación reserva-paquete</param>
    /// <returns>Detalles del paquete</returns>
    Task<ReservaPaqueteResponseDto> ObtenerPaquetePorIdAsync(int id);

    /// <summary>
    /// Elimina un paquete de una reserva
    /// </summary>
    /// <param name="idReserva">ID de la reserva</param>
    /// <param name="idReservaPaquete">ID de la relación reserva-paquete</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> EliminarPaqueteDeReservaAsync(int idReserva, int idReservaPaquete);
}
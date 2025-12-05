using G2rismBeta.API.DTOs.Vuelo;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el servicio de vuelos
/// Define la lógica de negocio
/// </summary>
public interface IVueloService
{
    /// <summary>
    /// Obtiene todos los vuelos
    /// </summary>
    /// <returns>Lista de vuelos (DTOs)</returns>
    Task<IEnumerable<VueloResponseDto>> GetAllAsync();

    /// <summary>
    /// Obtiene un vuelo por su ID
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <returns>Vuelo encontrado o null</returns>
    Task<VueloResponseDto?> GetByIdAsync(int id);

    /// <summary>
    /// Busca vuelos por origen y destino
    /// </summary>
    /// <param name="origen">Ciudad/aeropuerto de origen</param>
    /// <param name="destino">Ciudad/aeropuerto de destino</param>
    /// <returns>Lista de vuelos</returns>
    Task<IEnumerable<VueloResponseDto>> GetByOrigenDestinoAsync(string origen, string destino);

    /// <summary>
    /// Busca vuelos disponibles (con cupos)
    /// </summary>
    /// <returns>Lista de vuelos disponibles</returns>
    Task<IEnumerable<VueloResponseDto>> GetDisponiblesAsync();

    /// <summary>
    /// Busca vuelos por múltiples criterios
    /// </summary>
    /// <param name="origen">Origen (opcional)</param>
    /// <param name="destino">Destino (opcional)</param>
    /// <param name="fecha">Fecha (opcional)</param>
    /// <returns>Lista de vuelos</returns>
    Task<IEnumerable<VueloResponseDto>> BuscarAsync(string? origen, string? destino, DateTime? fecha);

    /// <summary>
    /// Crea un nuevo vuelo
    /// </summary>
    /// <param name="createDto">Datos del vuelo</param>
    /// <returns>Vuelo creado</returns>
    Task<VueloResponseDto> CreateAsync(VueloCreateDto createDto);

    /// <summary>
    /// Actualiza un vuelo existente
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Vuelo actualizado</returns>
    Task<VueloResponseDto> UpdateAsync(int id, VueloUpdateDto updateDto);

    /// <summary>
    /// Elimina un vuelo (soft delete)
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <returns>True si se eliminó</returns>
    Task<bool> DeleteAsync(int id);
}

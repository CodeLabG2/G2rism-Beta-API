using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el repositorio de aerolíneas
/// Define las operaciones de acceso a datos
/// </summary>
public interface IAerolineaRepository
{
    /// <summary>
    /// Obtiene todas las aerolíneas
    /// </summary>
    /// <returns>Lista de aerolíneas</returns>
    Task<IEnumerable<Aerolinea>> GetAllAsync();

    /// <summary>
    /// Obtiene una aerolínea por su ID
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<Aerolinea?> GetByIdAsync(int id);

    /// <summary>
    /// Busca una aerolínea por su código IATA
    /// </summary>
    /// <param name="codigoIata">Código IATA (2 caracteres)</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<Aerolinea?> GetByCodigoIataAsync(string codigoIata);

    /// <summary>
    /// Busca una aerolínea por su código ICAO
    /// </summary>
    /// <param name="codigoIcao">Código ICAO (3 caracteres)</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<Aerolinea?> GetByCodigoIcaoAsync(string codigoIcao);

    /// <summary>
    /// Obtiene aerolíneas filtradas por país
    /// </summary>
    /// <param name="pais">Nombre del país</param>
    /// <returns>Lista de aerolíneas del país</returns>
    Task<IEnumerable<Aerolinea>> GetByPaisAsync(string pais);

    /// <summary>
    /// Obtiene aerolíneas filtradas por estado
    /// </summary>
    /// <param name="estado">Estado (Activa/Inactiva)</param>
    /// <returns>Lista de aerolíneas con ese estado</returns>
    Task<IEnumerable<Aerolinea>> GetByEstadoAsync(string estado);

    /// <summary>
    /// Verifica si existe una aerolínea con el código IATA dado
    /// </summary>
    /// <param name="codigoIata">Código IATA</param>
    /// <param name="excludeId">ID a excluir (para updates)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistsCodigoIataAsync(string codigoIata, int? excludeId = null);

    /// <summary>
    /// Verifica si existe una aerolínea con el código ICAO dado
    /// </summary>
    /// <param name="codigoIcao">Código ICAO</param>
    /// <param name="excludeId">ID a excluir (para updates)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistsCodigoIcaoAsync(string codigoIcao, int? excludeId = null);

    /// <summary>
    /// Crea una nueva aerolínea
    /// </summary>
    /// <param name="aerolinea">Aerolínea a crear</param>
    /// <returns>Aerolínea creada</returns>
    Task<Aerolinea> CreateAsync(Aerolinea aerolinea);

    /// <summary>
    /// Actualiza una aerolínea existente
    /// </summary>
    /// <param name="aerolinea">Aerolínea con datos actualizados</param>
    /// <returns>True si se actualizó, False si no</returns>
    Task<bool> UpdateAsync(Aerolinea aerolinea);

    /// <summary>
    /// Elimina una aerolínea (cambio de estado)
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>True si se eliminó, False si no</returns>
    Task<bool> DeleteAsync(int id);
}

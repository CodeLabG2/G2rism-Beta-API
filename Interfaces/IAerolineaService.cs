using G2rismBeta.API.DTOs.Aerolinea;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el servicio de aerolíneas
/// Define la lógica de negocio
/// </summary>
public interface IAerolineaService
{
    /// <summary>
    /// Obtiene todas las aerolíneas
    /// </summary>
    /// <returns>Lista de aerolíneas (DTOs)</returns>
    Task<IEnumerable<AerolineaResponseDto>> GetAllAsync();

    /// <summary>
    /// Obtiene una aerolínea por su ID
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<AerolineaResponseDto?> GetByIdAsync(int id);

    /// <summary>
    /// Busca una aerolínea por su código IATA
    /// </summary>
    /// <param name="codigoIata">Código IATA</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<AerolineaResponseDto?> GetByCodigoIataAsync(string codigoIata);

    /// <summary>
    /// Busca una aerolínea por su código ICAO
    /// </summary>
    /// <param name="codigoIcao">Código ICAO</param>
    /// <returns>Aerolínea encontrada o null</returns>
    Task<AerolineaResponseDto?> GetByCodigoIcaoAsync(string codigoIcao);

    /// <summary>
    /// Obtiene aerolíneas de un país específico
    /// </summary>
    /// <param name="pais">Nombre del país</param>
    /// <returns>Lista de aerolíneas</returns>
    Task<IEnumerable<AerolineaResponseDto>> GetByPaisAsync(string pais);

    /// <summary>
    /// Obtiene aerolíneas por estado
    /// </summary>
    /// <param name="estado">Estado (Activa/Inactiva)</param>
    /// <returns>Lista de aerolíneas</returns>
    Task<IEnumerable<AerolineaResponseDto>> GetByEstadoAsync(string estado);

    /// <summary>
    /// Crea una nueva aerolínea
    /// </summary>
    /// <param name="createDto">Datos de la nueva aerolínea</param>
    /// <returns>Aerolínea creada</returns>
    Task<AerolineaResponseDto> CreateAsync(AerolineaCreateDto createDto);

    /// <summary>
    /// Actualiza una aerolínea existente
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Aerolínea actualizada o null si no existe</returns>
    Task<AerolineaResponseDto?> UpdateAsync(int id, AerolineaUpdateDto updateDto);

    /// <summary>
    /// Elimina una aerolínea (cambio de estado)
    /// </summary>
    /// <param name="id">ID de la aerolínea</param>
    /// <returns>True si se eliminó, False si no existe</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Verifica si un código IATA ya existe
    /// </summary>
    /// <param name="codigoIata">Código IATA</param>
    /// <param name="excludeId">ID a excluir (para updates)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistsCodigoIataAsync(string codigoIata, int? excludeId = null);

    /// <summary>
    /// Verifica si un código ICAO ya existe
    /// </summary>
    /// <param name="codigoIcao">Código ICAO</param>
    /// <param name="excludeId">ID a excluir (para updates)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistsCodigoIcaoAsync(string codigoIcao, int? excludeId = null);
}

using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz para el repositorio de vuelos
/// Define las operaciones de acceso a datos
/// </summary>
public interface IVueloRepository
{
    /// <summary>
    /// Obtiene todos los vuelos con sus relaciones
    /// </summary>
    /// <returns>Lista de vuelos</returns>
    Task<IEnumerable<Vuelo>> GetAllAsync();

    /// <summary>
    /// Obtiene un vuelo por su ID con sus relaciones
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <returns>Vuelo encontrado o null</returns>
    Task<Vuelo?> GetByIdAsync(int id);

    /// <summary>
    /// Busca vuelos por origen y destino
    /// </summary>
    /// <param name="origen">Ciudad/aeropuerto de origen</param>
    /// <param name="destino">Ciudad/aeropuerto de destino</param>
    /// <returns>Lista de vuelos que coinciden</returns>
    Task<IEnumerable<Vuelo>> GetByOrigenDestinoAsync(string origen, string destino);

    /// <summary>
    /// Busca vuelos por fecha de salida
    /// </summary>
    /// <param name="fecha">Fecha de salida</param>
    /// <returns>Lista de vuelos en esa fecha</returns>
    Task<IEnumerable<Vuelo>> GetByFechaSalidaAsync(DateTime fecha);

    /// <summary>
    /// Busca vuelos disponibles (con cupos > 0 y activos)
    /// </summary>
    /// <returns>Lista de vuelos disponibles</returns>
    Task<IEnumerable<Vuelo>> GetDisponiblesAsync();

    /// <summary>
    /// Busca vuelos por aerolínea
    /// </summary>
    /// <param name="idAerolinea">ID de la aerolínea</param>
    /// <returns>Lista de vuelos de la aerolínea</returns>
    Task<IEnumerable<Vuelo>> GetByAerolineaAsync(int idAerolinea);

    /// <summary>
    /// Busca vuelos por proveedor
    /// </summary>
    /// <param name="idProveedor">ID del proveedor</param>
    /// <returns>Lista de vuelos del proveedor</returns>
    Task<IEnumerable<Vuelo>> GetByProveedorAsync(int idProveedor);

    /// <summary>
    /// Busca un vuelo por su número
    /// </summary>
    /// <param name="numeroVuelo">Número de vuelo</param>
    /// <returns>Vuelo encontrado o null</returns>
    Task<Vuelo?> GetByNumeroVueloAsync(string numeroVuelo);

    /// <summary>
    /// Verifica si existe un vuelo con el número dado
    /// </summary>
    /// <param name="numeroVuelo">Número de vuelo</param>
    /// <param name="excludeId">ID a excluir (para updates)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistsNumeroVueloAsync(string numeroVuelo, int? excludeId = null);

    /// <summary>
    /// Crea un nuevo vuelo
    /// </summary>
    /// <param name="vuelo">Vuelo a crear</param>
    /// <returns>Vuelo creado</returns>
    Task<Vuelo> CreateAsync(Vuelo vuelo);

    /// <summary>
    /// Actualiza un vuelo existente
    /// </summary>
    /// <param name="vuelo">Vuelo con datos actualizados</param>
    /// <returns>True si se actualizó, False si no</returns>
    Task<bool> UpdateAsync(Vuelo vuelo);

    /// <summary>
    /// Elimina un vuelo (cambio de estado)
    /// </summary>
    /// <param name="id">ID del vuelo</param>
    /// <returns>True si se eliminó, False si no</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Busca vuelos por múltiples criterios
    /// </summary>
    /// <param name="origen">Origen (opcional)</param>
    /// <param name="destino">Destino (opcional)</param>
    /// <param name="fecha">Fecha (opcional)</param>
    /// <returns>Lista de vuelos que cumplen los criterios</returns>
    Task<IEnumerable<Vuelo>> BuscarAsync(string? origen, string? destino, DateTime? fecha);
}

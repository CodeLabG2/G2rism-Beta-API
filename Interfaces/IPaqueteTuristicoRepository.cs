using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del repositorio de paquetes turísticos con métodos de búsqueda avanzada
/// </summary>
public interface IPaqueteTuristicoRepository : IGenericRepository<PaqueteTuristico>
{
    /// <summary>
    /// Obtiene todos los paquetes turísticos
    /// </summary>
    /// <returns>Lista completa de paquetes turísticos</returns>
    Task<IEnumerable<PaqueteTuristico>> GetAllAsync();

    /// <summary>
    /// Obtiene un paquete turístico por su ID
    /// </summary>
    /// <param name="id">ID del paquete</param>
    /// <returns>Paquete turístico o null si no existe</returns>
    Task<PaqueteTuristico?> GetByIdAsync(int id);

    /// <summary>
    /// Busca paquetes turísticos por destino principal
    /// </summary>
    /// <param name="destino">Nombre del destino</param>
    /// <returns>Lista de paquetes con ese destino principal</returns>
    Task<IEnumerable<PaqueteTuristico>> GetByDestinoAsync(string destino);

    /// <summary>
    /// Busca paquetes turísticos por tipo
    /// </summary>
    /// <param name="tipo">Tipo de paquete (aventura, familiar, empresarial, lujo, cultural, ecologico, romantico)</param>
    /// <returns>Lista de paquetes del tipo especificado</returns>
    Task<IEnumerable<PaqueteTuristico>> GetByTipoAsync(string tipo);

    /// <summary>
    /// Obtiene todos los paquetes disponibles (activos y con cupos)
    /// </summary>
    /// <returns>Lista de paquetes disponibles</returns>
    Task<IEnumerable<PaqueteTuristico>> GetDisponiblesAsync();

    /// <summary>
    /// Busca paquetes por rango de duración
    /// </summary>
    /// <param name="diasMin">Duración mínima en días</param>
    /// <param name="diasMax">Duración máxima en días</param>
    /// <returns>Lista de paquetes en el rango de duración</returns>
    Task<IEnumerable<PaqueteTuristico>> GetByDuracionAsync(int diasMin, int diasMax);

    /// <summary>
    /// Busca paquetes por rango de precio
    /// </summary>
    /// <param name="precioMin">Precio mínimo</param>
    /// <param name="precioMax">Precio máximo</param>
    /// <returns>Lista de paquetes en el rango de precio</returns>
    Task<IEnumerable<PaqueteTuristico>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax);

    /// <summary>
    /// Busca paquetes por nivel de dificultad
    /// </summary>
    /// <param name="nivel">Nivel de dificultad (bajo, medio, alto)</param>
    /// <returns>Lista de paquetes con ese nivel de dificultad</returns>
    Task<IEnumerable<PaqueteTuristico>> GetByNivelDificultadAsync(string nivel);

    /// <summary>
    /// Obtiene paquetes próximos a iniciar (dentro de los próximos 7 días)
    /// </summary>
    /// <returns>Lista de paquetes próximos a iniciar</returns>
    Task<IEnumerable<PaqueteTuristico>> GetProximosAIniciarAsync();

    /// <summary>
    /// Obtiene paquetes vigentes (dentro del rango de fechas activo)
    /// </summary>
    /// <returns>Lista de paquetes vigentes</returns>
    Task<IEnumerable<PaqueteTuristico>> GetVigentesAsync();

    /// <summary>
    /// Verifica si existe un paquete con el mismo nombre
    /// </summary>
    /// <param name="nombre">Nombre del paquete</param>
    /// <param name="idPaqueteExcluir">ID del paquete a excluir (para actualizaciones)</param>
    /// <returns>True si existe, False si no</returns>
    Task<bool> ExistePorNombreAsync(string nombre, int? idPaqueteExcluir = null);
}

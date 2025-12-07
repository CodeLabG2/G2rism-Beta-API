using G2rismBeta.API.DTOs.PaqueteTuristico;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interfaz del servicio de paquetes turísticos con lógica de negocio
/// </summary>
public interface IPaqueteTuristicoService
{
    /// <summary>
    /// Obtiene todos los paquetes turísticos
    /// </summary>
    /// <returns>Lista de paquetes turísticos</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetAllAsync();

    /// <summary>
    /// Obtiene un paquete turístico por su ID
    /// </summary>
    /// <param name="id">ID del paquete</param>
    /// <returns>Paquete encontrado</returns>
    /// <exception cref="KeyNotFoundException">Si el paquete no existe</exception>
    Task<PaqueteTuristicoResponseDto> GetByIdAsync(int id);

    /// <summary>
    /// Busca paquetes por destino principal
    /// </summary>
    /// <param name="destino">Nombre del destino</param>
    /// <returns>Lista de paquetes con ese destino</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByDestinoAsync(string destino);

    /// <summary>
    /// Busca paquetes por tipo
    /// </summary>
    /// <param name="tipo">Tipo de paquete</param>
    /// <returns>Lista de paquetes del tipo especificado</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByTipoAsync(string tipo);

    /// <summary>
    /// Obtiene todos los paquetes disponibles
    /// </summary>
    /// <returns>Lista de paquetes disponibles</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetDisponiblesAsync();

    /// <summary>
    /// Busca paquetes por rango de duración
    /// </summary>
    /// <param name="diasMin">Duración mínima en días</param>
    /// <param name="diasMax">Duración máxima en días</param>
    /// <returns>Lista de paquetes en el rango de duración</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByDuracionAsync(int diasMin, int diasMax);

    /// <summary>
    /// Busca paquetes por rango de precio
    /// </summary>
    /// <param name="precioMin">Precio mínimo</param>
    /// <param name="precioMax">Precio máximo</param>
    /// <returns>Lista de paquetes en el rango de precio</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax);

    /// <summary>
    /// Busca paquetes por nivel de dificultad
    /// </summary>
    /// <param name="nivel">Nivel de dificultad</param>
    /// <returns>Lista de paquetes con ese nivel</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByNivelDificultadAsync(string nivel);

    /// <summary>
    /// Obtiene paquetes próximos a iniciar
    /// </summary>
    /// <returns>Lista de paquetes próximos a iniciar</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetProximosAIniciarAsync();

    /// <summary>
    /// Obtiene paquetes vigentes
    /// </summary>
    /// <returns>Lista de paquetes vigentes</returns>
    Task<IEnumerable<PaqueteTuristicoResponseDto>> GetVigentesAsync();

    /// <summary>
    /// Crea un nuevo paquete turístico
    /// </summary>
    /// <param name="paqueteDto">Datos del paquete a crear</param>
    /// <returns>Paquete creado</returns>
    /// <exception cref="ArgumentException">Si ya existe un paquete con el mismo nombre</exception>
    Task<PaqueteTuristicoResponseDto> CreateAsync(PaqueteTuristicoCreateDto paqueteDto);

    /// <summary>
    /// Actualiza un paquete turístico existente
    /// </summary>
    /// <param name="id">ID del paquete a actualizar</param>
    /// <param name="paqueteDto">Datos a actualizar</param>
    /// <returns>Paquete actualizado</returns>
    /// <exception cref="KeyNotFoundException">Si el paquete no existe</exception>
    /// <exception cref="ArgumentException">Si ya existe otro paquete con el mismo nombre</exception>
    Task<PaqueteTuristicoResponseDto> UpdateAsync(int id, PaqueteTuristicoUpdateDto paqueteDto);

    /// <summary>
    /// Elimina un paquete (soft delete - cambia estado a inactivo)
    /// </summary>
    /// <param name="id">ID del paquete a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    /// <exception cref="KeyNotFoundException">Si el paquete no existe</exception>
    Task<bool> DeleteAsync(int id);
}

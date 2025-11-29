using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interface del repositorio de Códigos de Recuperación
/// Gestiona códigos de 6 dígitos para recuperación de contraseña
/// </summary>
public interface ICodigoRecuperacionRepository
{
    // ========================================
    // MÉTODOS DE CONSULTA
    // ========================================

    /// <summary>
    /// Obtener un código por su valor (6 dígitos)
    /// </summary>
    Task<CodigoRecuperacion?> GetByCodigoAsync(string codigo);

    /// <summary>
    /// Obtener un código por su ID
    /// </summary>
    Task<CodigoRecuperacion?> GetByIdAsync(int idCodigo);

    /// <summary>
    /// Obtener todos los códigos de un usuario
    /// </summary>
    Task<IEnumerable<CodigoRecuperacion>> GetByUsuarioIdAsync(int idUsuario);

    /// <summary>
    /// Obtener códigos activos (no usados, no expirados, no bloqueados) de un usuario
    /// </summary>
    Task<IEnumerable<CodigoRecuperacion>> GetCodigosActivosAsync(int idUsuario);

    // ========================================
    // MÉTODOS DE VALIDACIÓN
    // ========================================

    /// <summary>
    /// Validar si un código es válido (existe, no está usado, no está expirado, no bloqueado)
    /// Incrementa el contador de intentos si el código es incorrecto
    /// </summary>
    Task<bool> ValidarCodigoAsync(string codigo);

    /// <summary>
    /// Verificar si un código está expirado
    /// </summary>
    Task<bool> EstaExpiradoAsync(string codigo);

    /// <summary>
    /// Verificar si un código ya fue usado
    /// </summary>
    Task<bool> EstaUsadoAsync(string codigo);

    /// <summary>
    /// Verificar si un código está bloqueado por exceso de intentos
    /// </summary>
    Task<bool> EstaBloqueadoAsync(string codigo);

    // ========================================
    // MÉTODOS DE GESTIÓN
    // ========================================

    /// <summary>
    /// Crear un nuevo código
    /// </summary>
    Task<CodigoRecuperacion> CrearCodigoAsync(CodigoRecuperacion codigo);

    /// <summary>
    /// Marcar un código como usado
    /// </summary>
    Task MarcarComoUsadoAsync(string codigo);

    /// <summary>
    /// Incrementar contador de intentos fallidos
    /// Bloquea el código si supera el máximo de intentos (5)
    /// </summary>
    Task IncrementarIntentosAsync(string codigo);

    /// <summary>
    /// Bloquear un código por exceso de intentos
    /// </summary>
    Task BloquearCodigoAsync(string codigo);

    /// <summary>
    /// Invalidar todos los códigos activos de un usuario
    /// (Útil cuando el usuario cambia la contraseña o solicita un nuevo código)
    /// </summary>
    Task InvalidarCodigosActivosAsync(int idUsuario);

    /// <summary>
    /// Eliminar códigos expirados del sistema (limpieza)
    /// </summary>
    Task LimpiarCodigosExpiradosAsync();

    /// <summary>
    /// Eliminar todos los códigos de un usuario
    /// </summary>
    Task EliminarCodigosDeUsuarioAsync(int idUsuario);

    // ========================================
    // MÉTODOS DE UTILIDAD
    // ========================================

    /// <summary>
    /// Guardar cambios en la base de datos
    /// </summary>
    Task<bool> SaveChangesAsync();
}

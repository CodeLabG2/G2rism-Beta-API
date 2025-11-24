using G2rismBeta.API.Models;

namespace G2rismBeta.API.Interfaces;

/// <summary>
/// Interface del servicio de Usuarios
/// Define la lógica de negocio para gestión de usuarios
/// </summary>
public interface IUsuarioService
{
    // ========================================
    // OPERACIONES CRUD
    // ========================================

    /// <summary>
    /// Crear un nuevo usuario
    /// La contraseña se recibe en texto plano y se hashea aquí
    /// </summary>
    Task<Usuario> CrearUsuarioAsync(Usuario usuario, string password, List<int>? rolesIds = null);

    /// <summary>
    /// Obtener todos los usuarios
    /// </summary>
    Task<IEnumerable<Usuario>> GetAllUsuariosAsync();

    /// <summary>
    /// Obtener todos los usuarios con sus roles
    /// </summary>
    Task<IEnumerable<Usuario>> GetAllUsuariosConRolesAsync();

    /// <summary>
    /// Obtener un usuario por ID
    /// </summary>
    Task<Usuario?> GetUsuarioByIdAsync(int idUsuario);

    /// <summary>
    /// Obtener un usuario por ID con sus roles
    /// </summary>
    Task<Usuario?> GetUsuarioByIdConRolesAsync(int idUsuario);

    /// <summary>
    /// Obtener usuarios activos
    /// </summary>
    Task<IEnumerable<Usuario>> GetUsuariosActivosAsync();

    /// <summary>
    /// Obtener usuarios por tipo
    /// </summary>
    Task<IEnumerable<Usuario>> GetUsuariosByTipoAsync(string tipo);

    /// <summary>
    /// Actualizar un usuario
    /// </summary>
    Task<Usuario> ActualizarUsuarioAsync(Usuario usuario);

    /// <summary>
    /// Cambiar el estado de un usuario (activar/desactivar)
    /// </summary>
    Task<bool> CambiarEstadoUsuarioAsync(int idUsuario, bool nuevoEstado);

    /// <summary>
    /// Eliminar un usuario (soft delete o hard delete según negocio)
    /// </summary>
    Task<bool> EliminarUsuarioAsync(int idUsuario);

    // ========================================
    // GESTIÓN DE CONTRASEÑAS
    // ========================================

    /// <summary>
    /// Cambiar la contraseña de un usuario
    /// </summary>
    Task<bool> CambiarPasswordAsync(int idUsuario, string passwordActual, string passwordNueva);

    /// <summary>
    /// Cambiar la contraseña sin validar la actual (para admin o recuperación)
    /// </summary>
    Task<bool> ResetPasswordAsync(int idUsuario, string passwordNueva);

    // ========================================
    // GESTIÓN DE ROLES
    // ========================================

    /// <summary>
    /// Asignar roles a un usuario
    /// </summary>
    Task AsignarRolesAsync(int idUsuario, List<int> rolesIds, int? asignadoPor = null);

    /// <summary>
    /// Remover un rol de un usuario
    /// </summary>
    Task<bool> RemoverRolAsync(int idUsuario, int idRol);

    /// <summary>
    /// Obtener los roles de un usuario
    /// </summary>
    Task<IEnumerable<Rol>> GetRolesDeUsuarioAsync(int idUsuario);

    // ========================================
    // SEGURIDAD
    // ========================================

    /// <summary>
    /// Bloquear un usuario
    /// </summary>
    Task<bool> BloquearUsuarioAsync(int idUsuario);

    /// <summary>
    /// Desbloquear un usuario
    /// </summary>
    Task<bool> DesbloquearUsuarioAsync(int idUsuario);

    /// <summary>
    /// Obtener usuarios bloqueados
    /// </summary>
    Task<IEnumerable<Usuario>> GetUsuariosBloqueadosAsync();

    // ========================================
    // VALIDACIONES
    // ========================================

    /// <summary>
    /// Validar si un username ya existe
    /// </summary>
    Task<bool> ExisteUsernameAsync(string username, int? idUsuarioExcluir = null);

    /// <summary>
    /// Validar si un email ya existe
    /// </summary>
    Task<bool> ExisteEmailAsync(string email, int? idUsuarioExcluir = null);

    /// <summary>
    /// Validar si los roles son compatibles con el tipo de usuario
    /// </summary>
    Task<(bool esValido, string? mensajeError)> ValidarRolesParaTipoUsuarioAsync(int idUsuario, List<int> rolesIds);

    /// <summary>
    /// Validar si se puede asignar el rol de Súper Administrador
    /// </summary>
    Task<(bool esValido, string? mensajeError)> ValidarAsignacionSuperAdministradorAsync(int idUsuario, List<int> rolesIds);
}
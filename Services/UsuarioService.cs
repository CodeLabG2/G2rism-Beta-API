using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using G2rismBeta.API.Helpers;
using G2rismBeta.API.Constants;

namespace G2rismBeta.API.Services;

/// <summary>
/// Implementación del servicio de Usuarios
/// Contiene toda la lógica de negocio para gestión de usuarios
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly ITokenRecuperacionRepository _tokenRepository;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository,
        ITokenRecuperacionRepository tokenRepository)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _tokenRepository = tokenRepository;
    }

    // ========================================
    // OPERACIONES CRUD
    // ========================================

    /// <summary>
    /// Crear un nuevo usuario con validaciones de negocio
    /// </summary>
    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario, string password, List<int>? rolesIds = null)
    {
        // 1. Validar fortaleza de la contraseña
        var (esValida, errorPassword) = PasswordHasher.ValidatePasswordStrength(password);
        if (!esValida)
        {
            throw new ArgumentException(errorPassword ?? "La contraseña no cumple los requisitos");
        }

        // 2. Validar que el username no exista
        if (await _usuarioRepository.ExistsByUsernameAsync(usuario.Username))
        {
            throw new InvalidOperationException($"El username '{usuario.Username}' ya está en uso");
        }

        // 3. Validar que el email no exista
        if (await _usuarioRepository.ExistsByEmailAsync(usuario.Email))
        {
            throw new InvalidOperationException($"El email '{usuario.Email}' ya está registrado");
        }

        // 4. Hashear la contraseña
        usuario.PasswordHash = PasswordHasher.HashPassword(password);

        // 5. Establecer valores por defecto
        usuario.FechaCreacion = DateTime.Now;
        usuario.Estado = true;
        usuario.Bloqueado = false;
        usuario.IntentosFallidos = 0;

        // 6. Crear el usuario
        var usuarioCreado = await _usuarioRepository.AddAsync(usuario);
        await _usuarioRepository.SaveChangesAsync();

        // 7. Asignar roles si se proporcionaron (CON VALIDACIONES)
        if (rolesIds != null && rolesIds.Any())
        {
            // Validar que los roles sean compatibles con el tipo de usuario
            foreach (var idRol in rolesIds)
            {
                if (!RoleConstants.EsRolValidoParaTipoUsuario(idRol, usuarioCreado.TipoUsuario))
                {
                    var rolesPermitidos = RoleConstants.GetRolesPermitidos(usuarioCreado.TipoUsuario);
                    var nombresRolesPermitidos = string.Join(", ", rolesPermitidos.Select(r => ObtenerNombreRol(r)));

                    throw new InvalidOperationException(
                        $"El usuario de tipo '{usuarioCreado.TipoUsuario}' no puede tener el rol con ID {idRol}. " +
                        $"Roles permitidos: {nombresRolesPermitidos}");
                }
            }

            // Validar restricción de Súper Administrador único
            if (rolesIds.Contains(RoleConstants.SUPER_ADMINISTRADOR_ID))
            {
                var existeSuperAdmin = await _usuarioRolRepository.ExisteSuperAdministradorAsync();
                if (existeSuperAdmin)
                {
                    var idSuperAdminActual = await _usuarioRolRepository.ObtenerIdSuperAdministradorAsync();
                    var superAdminActual = await _usuarioRepository.GetByIdAsync(idSuperAdminActual!.Value);

                    throw new InvalidOperationException(
                        $"Ya existe un Súper Administrador en el sistema (Usuario: {superAdminActual?.Username}, ID: {idSuperAdminActual}). " +
                        $"Solo puede haber un Súper Administrador a la vez.");
                }
            }

            // Si todas las validaciones pasan, asignar los roles
            await _usuarioRolRepository.AsignarRolesMultiplesAsync(usuarioCreado.IdUsuario, rolesIds);
        }

        return usuarioCreado;
    }

    /// <summary>
    /// Obtener todos los usuarios
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    /// <summary>
    /// Obtener todos los usuarios con sus roles
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetAllUsuariosConRolesAsync()
    {
        return await _usuarioRepository.GetAllWithRolesAsync();
    }

    /// <summary>
    /// Obtener un usuario por ID
    /// </summary>
    public async Task<Usuario?> GetUsuarioByIdAsync(int idUsuario)
    {
        return await _usuarioRepository.GetByIdAsync(idUsuario);
    }

    /// <summary>
    /// Obtener un usuario por ID con sus roles
    /// </summary>
    public async Task<Usuario?> GetUsuarioByIdConRolesAsync(int idUsuario)
    {
        return await _usuarioRepository.GetByIdWithRolesAsync(idUsuario);
    }

    /// <summary>
    /// Obtener usuarios activos
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetUsuariosActivosAsync()
    {
        return await _usuarioRepository.GetUsuariosActivosAsync();
    }

    /// <summary>
    /// Obtener usuarios por tipo
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetUsuariosByTipoAsync(string tipo)
    {
        if (tipo.ToLower() != "cliente" && tipo.ToLower() != "empleado")
        {
            throw new ArgumentException("El tipo debe ser 'cliente' o 'empleado'");
        }

        return await _usuarioRepository.GetUsuariosByTipoAsync(tipo);
    }

    /// <summary>
    /// Actualizar un usuario
    /// </summary>
    public async Task<Usuario> ActualizarUsuarioAsync(Usuario usuario)
    {
        // Validar que el usuario existe
        var usuarioExistente = await _usuarioRepository.GetByIdAsync(usuario.IdUsuario);
        if (usuarioExistente == null)
        {
            throw new KeyNotFoundException($"No se encontró el usuario con ID {usuario.IdUsuario}");
        }

        // Validar username (si cambió)
        if (usuario.Username != usuarioExistente.Username)
        {
            if (await _usuarioRepository.ExistsByUsernameExceptIdAsync(usuario.Username, usuario.IdUsuario))
            {
                throw new InvalidOperationException($"El username '{usuario.Username}' ya está en uso");
            }
        }

        // Validar email (si cambió)
        if (usuario.Email != usuarioExistente.Email)
        {
            if (await _usuarioRepository.ExistsByEmailExceptIdAsync(usuario.Email, usuario.IdUsuario))
            {
                throw new InvalidOperationException($"El email '{usuario.Email}' ya está registrado");
            }
        }

        // NO actualizar la contraseña aquí (hay métodos específicos)
        usuario.PasswordHash = usuarioExistente.PasswordHash;

        // Actualizar fecha de modificación
        usuario.FechaModificacion = DateTime.Now;

        var usuarioActualizado = await _usuarioRepository.UpdateAsync(usuario);
        await _usuarioRepository.SaveChangesAsync();

        return usuarioActualizado;
    }

    /// <summary>
    /// Cambiar el estado de un usuario
    /// </summary>
    public async Task<bool> CambiarEstadoUsuarioAsync(int idUsuario, bool nuevoEstado)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
            return false;

        usuario.Estado = nuevoEstado;
        usuario.FechaModificacion = DateTime.Now;

        await _usuarioRepository.UpdateAsync(usuario);
        return await _usuarioRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Eliminar un usuario
    /// </summary>
    public async Task<bool> EliminarUsuarioAsync(int idUsuario)
    {
        // Por seguridad, hacer soft delete cambiando el estado
        return await CambiarEstadoUsuarioAsync(idUsuario, false);

        // Si necesitan hard delete:
        // return await _usuarioRepository.DeleteAsync(idUsuario);
    }

    // ========================================
    // GESTIÓN DE CONTRASEÑAS
    // ========================================

    /// <summary>
    /// Cambiar contraseña validando la actual
    /// </summary>
    public async Task<bool> CambiarPasswordAsync(int idUsuario, string passwordActual, string passwordNueva)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
            return false;

        // Validar que la contraseña actual sea correcta
        if (!PasswordHasher.VerifyPassword(passwordActual, usuario.PasswordHash))
        {
            throw new UnauthorizedAccessException("La contraseña actual es incorrecta");
        }

        // Validar fortaleza de la nueva contraseña
        var (esValida, errorPassword) = PasswordHasher.ValidatePasswordStrength(passwordNueva);
        if (!esValida)
        {
            throw new ArgumentException(errorPassword ?? "La contraseña no cumple los requisitos");
        }

        // Hashear la nueva contraseña
        usuario.PasswordHash = PasswordHasher.HashPassword(passwordNueva);
        usuario.FechaModificacion = DateTime.Now;

        await _usuarioRepository.UpdateAsync(usuario);
        await _usuarioRepository.SaveChangesAsync();

        // Invalidar tokens activos
        await _tokenRepository.InvalidarTokensActivosAsync(idUsuario);

        return true;
    }

    /// <summary>
    /// Reset de contraseña sin validar la actual
    /// </summary>
    public async Task<bool> ResetPasswordAsync(int idUsuario, string passwordNueva)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
            return false;

        // Validar fortaleza de la nueva contraseña
        var (esValida, errorPassword) = PasswordHasher.ValidatePasswordStrength(passwordNueva);
        if (!esValida)
        {
            throw new ArgumentException(errorPassword ?? "La contraseña no cumple los requisitos");
        }

        // Hashear la nueva contraseña
        usuario.PasswordHash = PasswordHasher.HashPassword(passwordNueva);
        usuario.FechaModificacion = DateTime.Now;

        // Desbloquear y resetear intentos
        usuario.Bloqueado = false;
        usuario.IntentosFallidos = 0;

        await _usuarioRepository.UpdateAsync(usuario);
        await _usuarioRepository.SaveChangesAsync();

        // Invalidar tokens activos
        await _tokenRepository.InvalidarTokensActivosAsync(idUsuario);

        return true;
    }

    // ========================================
    // GESTIÓN DE ROLES
    // ========================================

    /// <summary>
    /// Asignar roles a un usuario con validaciones de negocio
    /// </summary>
    public async Task AsignarRolesAsync(int idUsuario, List<int> rolesIds, int? asignadoPor = null)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
        {
            throw new KeyNotFoundException($"No se encontró el usuario con ID {idUsuario}");
        }

        // VALIDACIÓN 1: Verificar que los roles sean compatibles con el tipo de usuario
        var (esValidoTipo, errorTipo) = await ValidarRolesParaTipoUsuarioAsync(idUsuario, rolesIds);
        if (!esValidoTipo)
        {
            throw new InvalidOperationException(errorTipo!);
        }

        // VALIDACIÓN 2: Verificar restricción de Súper Administrador único
        var (esValidoSuperAdmin, errorSuperAdmin) = await ValidarAsignacionSuperAdministradorAsync(idUsuario, rolesIds);
        if (!esValidoSuperAdmin)
        {
            throw new InvalidOperationException(errorSuperAdmin!);
        }

        // Si todas las validaciones pasan, asignar los roles
        await _usuarioRolRepository.ReemplazarRolesAsync(idUsuario, rolesIds, asignadoPor);
    }

    /// <summary>
    /// Remover un rol de un usuario
    /// </summary>
    public async Task<bool> RemoverRolAsync(int idUsuario, int idRol)
    {
        return await _usuarioRolRepository.RemoverRolAsync(idUsuario, idRol);
    }

    /// <summary>
    /// Obtener los roles de un usuario
    /// </summary>
    public async Task<IEnumerable<Rol>> GetRolesDeUsuarioAsync(int idUsuario)
    {
        var usuarioRoles = await _usuarioRolRepository.GetRolesByUsuarioIdAsync(idUsuario);
        return usuarioRoles.Select(ur => ur.Rol).ToList();
    }

    // ========================================
    // SEGURIDAD
    // ========================================

    /// <summary>
    /// Bloquear un usuario
    /// </summary>
    public async Task<bool> BloquearUsuarioAsync(int idUsuario)
    {
        await _usuarioRepository.BloquearUsuarioAsync(idUsuario);
        return true;
    }

    /// <summary>
    /// Desbloquear un usuario
    /// </summary>
    public async Task<bool> DesbloquearUsuarioAsync(int idUsuario)
    {
        await _usuarioRepository.DesbloquearUsuarioAsync(idUsuario);
        return true;
    }

    /// <summary>
    /// Obtener usuarios bloqueados
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetUsuariosBloqueadosAsync()
    {
        return await _usuarioRepository.GetUsuariosBloqueadosAsync();
    }

    // ========================================
    // VALIDACIONES
    // ========================================

    /// <summary>
    /// Validar si un username existe
    /// </summary>
    public async Task<bool> ExisteUsernameAsync(string username, int? idUsuarioExcluir = null)
    {
        if (idUsuarioExcluir.HasValue)
        {
            return await _usuarioRepository.ExistsByUsernameExceptIdAsync(username, idUsuarioExcluir.Value);
        }

        return await _usuarioRepository.ExistsByUsernameAsync(username);
    }

    /// <summary>
    /// Validar si un email existe
    /// </summary>
    public async Task<bool> ExisteEmailAsync(string email, int? idUsuarioExcluir = null)
    {
        if (idUsuarioExcluir.HasValue)
        {
            return await _usuarioRepository.ExistsByEmailExceptIdAsync(email, idUsuarioExcluir.Value);
        }

        return await _usuarioRepository.ExistsByEmailAsync(email);
    }

    /// <summary>
    /// Validar si los roles son compatibles con el tipo de usuario
    /// </summary>
    public async Task<(bool esValido, string? mensajeError)> ValidarRolesParaTipoUsuarioAsync(int idUsuario, List<int> rolesIds)
    {
        // Obtener el usuario para verificar su tipo
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
        {
            return (false, $"No se encontró el usuario con ID {idUsuario}");
        }

        // Verificar cada rol contra el tipo de usuario
        foreach (var idRol in rolesIds)
        {
            if (!RoleConstants.EsRolValidoParaTipoUsuario(idRol, usuario.TipoUsuario))
            {
                // Construir mensaje de error detallado
                var rolesPermitidos = RoleConstants.GetRolesPermitidos(usuario.TipoUsuario);
                var nombresRolesPermitidos = string.Join(", ", rolesPermitidos.Select(r => ObtenerNombreRol(r)));

                return (false,
                    $"El usuario de tipo '{usuario.TipoUsuario}' no puede tener el rol con ID {idRol}. " +
                    $"Roles permitidos para usuarios de tipo '{usuario.TipoUsuario}': {nombresRolesPermitidos}");
            }
        }

        return (true, null);
    }

    /// <summary>
    /// Validar si se puede asignar el rol de Súper Administrador
    /// </summary>
    public async Task<(bool esValido, string? mensajeError)> ValidarAsignacionSuperAdministradorAsync(int idUsuario, List<int> rolesIds)
    {
        // Verificar si se está intentando asignar el rol de Súper Administrador
        if (!rolesIds.Contains(RoleConstants.SUPER_ADMINISTRADOR_ID))
        {
            // No se está asignando Súper Admin, validación pasa
            return (true, null);
        }

        // Verificar si ya existe otro usuario con el rol de Súper Administrador
        var existeSuperAdmin = await _usuarioRolRepository.ExisteSuperAdministradorAsync(excluirIdUsuario: idUsuario);

        if (existeSuperAdmin)
        {
            // Obtener el ID del usuario que actualmente tiene el rol
            var idSuperAdminActual = await _usuarioRolRepository.ObtenerIdSuperAdministradorAsync();
            var superAdminActual = await _usuarioRepository.GetByIdAsync(idSuperAdminActual!.Value);

            return (false,
                $"Ya existe un Súper Administrador en el sistema (Usuario: {superAdminActual?.Username}, ID: {idSuperAdminActual}). " +
                $"Solo puede haber un Súper Administrador a la vez. Primero debe remover el rol del usuario actual.");
        }

        return (true, null);
    }

    /// <summary>
    /// Método auxiliar para obtener el nombre de un rol por su ID
    /// </summary>
    private static string ObtenerNombreRol(int idRol)
    {
        return idRol switch
        {
            RoleConstants.SUPER_ADMINISTRADOR_ID => RoleConstants.SUPER_ADMINISTRADOR,
            RoleConstants.ADMINISTRADOR_ID => RoleConstants.ADMINISTRADOR,
            RoleConstants.EMPLEADO_ID => RoleConstants.EMPLEADO,
            RoleConstants.CLIENTE_ID => RoleConstants.CLIENTE,
            _ => $"Rol {idRol}"
        };
    }
}
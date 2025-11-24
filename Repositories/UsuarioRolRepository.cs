using Microsoft.EntityFrameworkCore;
using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using G2rismBeta.API.Constants;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio de Usuarios-Roles
/// </summary>
public class UsuarioRolRepository : IUsuarioRolRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRolRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========================================
    // MÉTODOS DE CONSULTA
    // ========================================

    /// <summary>
    /// Obtener todos los roles de un usuario con navegación
    /// </summary>
    public async Task<IEnumerable<UsuarioRol>> GetRolesByUsuarioIdAsync(int idUsuario)
    {
        return await _context.UsuariosRoles
            .Include(ur => ur.Rol)
            .Where(ur => ur.IdUsuario == idUsuario)
            .ToListAsync();
    }

    /// <summary>
    /// Obtener todos los usuarios que tienen un rol
    /// </summary>
    public async Task<IEnumerable<UsuarioRol>> GetUsuariosByRolIdAsync(int idRol)
    {
        return await _context.UsuariosRoles
            .Include(ur => ur.Usuario)
            .Where(ur => ur.IdRol == idRol)
            .ToListAsync();
    }

    /// <summary>
    /// Verificar si un usuario tiene un rol
    /// </summary>
    public async Task<bool> UsuarioTieneRolAsync(int idUsuario, int idRol)
    {
        return await _context.UsuariosRoles
            .AnyAsync(ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol);
    }

    /// <summary>
    /// Obtener relación específica usuario-rol
    /// </summary>
    public async Task<UsuarioRol?> GetByUsuarioAndRolAsync(int idUsuario, int idRol)
    {
        return await _context.UsuariosRoles
            .Include(ur => ur.Rol)
            .Include(ur => ur.Usuario)
            .FirstOrDefaultAsync(ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol);
    }

    /// <summary>
    /// Verificar si ya existe un usuario con el rol de Súper Administrador (excluyendo un usuario específico)
    /// </summary>
    public async Task<bool> ExisteSuperAdministradorAsync(int? excluirIdUsuario = null)
    {
        var query = _context.UsuariosRoles
            .Where(ur => ur.IdRol == RoleConstants.SUPER_ADMINISTRADOR_ID);

        if (excluirIdUsuario.HasValue)
        {
            query = query.Where(ur => ur.IdUsuario != excluirIdUsuario.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Contar cuántos usuarios tienen asignado el rol de Súper Administrador
    /// </summary>
    public async Task<int> ContarSuperAdministradoresAsync()
    {
        return await _context.UsuariosRoles
            .Where(ur => ur.IdRol == RoleConstants.SUPER_ADMINISTRADOR_ID)
            .CountAsync();
    }

    /// <summary>
    /// Obtener el ID del usuario que actualmente tiene el rol de Súper Administrador
    /// </summary>
    public async Task<int?> ObtenerIdSuperAdministradorAsync()
    {
        var usuarioRol = await _context.UsuariosRoles
            .Where(ur => ur.IdRol == RoleConstants.SUPER_ADMINISTRADOR_ID)
            .FirstOrDefaultAsync();

        return usuarioRol?.IdUsuario;
    }

    // ========================================
    // MÉTODOS DE ASIGNACIÓN
    // ========================================

    /// <summary>
    /// Asignar un rol a un usuario
    /// </summary>
    public async Task<UsuarioRol> AsignarRolAsync(UsuarioRol usuarioRol)
    {
        // Verificar si ya existe la asignación
        var existe = await UsuarioTieneRolAsync(usuarioRol.IdUsuario, usuarioRol.IdRol);
        if (existe)
        {
            throw new InvalidOperationException("El usuario ya tiene este rol asignado");
        }

        await _context.UsuariosRoles.AddAsync(usuarioRol);
        await _context.SaveChangesAsync();

        return usuarioRol;
    }

    /// <summary>
    /// Asignar múltiples roles a un usuario
    /// </summary>
    public async Task AsignarRolesMultiplesAsync(int idUsuario, List<int> idsRoles, int? asignadoPor = null)
    {
        // Obtener roles que el usuario NO tiene aún
        var rolesActuales = await _context.UsuariosRoles
            .Where(ur => ur.IdUsuario == idUsuario)
            .Select(ur => ur.IdRol)
            .ToListAsync();

        var nuevosRoles = idsRoles.Except(rolesActuales).ToList();

        if (nuevosRoles.Any())
        {
            var asignaciones = nuevosRoles.Select(idRol => new UsuarioRol
            {
                IdUsuario = idUsuario,
                IdRol = idRol,
                FechaAsignacion = DateTime.Now,
                AsignadoPor = asignadoPor
            }).ToList();

            await _context.UsuariosRoles.AddRangeAsync(asignaciones);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Remover un rol de un usuario
    /// </summary>
    public async Task<bool> RemoverRolAsync(int idUsuario, int idRol)
    {
        var usuarioRol = await _context.UsuariosRoles
            .FirstOrDefaultAsync(ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol);

        if (usuarioRol == null)
            return false;

        _context.UsuariosRoles.Remove(usuarioRol);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Remover todos los roles de un usuario
    /// </summary>
    public async Task RemoverTodosLosRolesAsync(int idUsuario)
    {
        var roles = await _context.UsuariosRoles
            .Where(ur => ur.IdUsuario == idUsuario)
            .ToListAsync();

        if (roles.Any())
        {
            _context.UsuariosRoles.RemoveRange(roles);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Reemplazar todos los roles de un usuario
    /// </summary>
    public async Task ReemplazarRolesAsync(int idUsuario, List<int> nuevosIdsRoles, int? asignadoPor = null)
    {
        // Primero eliminar todos los roles actuales
        await RemoverTodosLosRolesAsync(idUsuario);

        // Luego asignar los nuevos roles
        if (nuevosIdsRoles.Any())
        {
            var nuevasAsignaciones = nuevosIdsRoles.Select(idRol => new UsuarioRol
            {
                IdUsuario = idUsuario,
                IdRol = idRol,
                FechaAsignacion = DateTime.Now,
                AsignadoPor = asignadoPor
            }).ToList();

            await _context.UsuariosRoles.AddRangeAsync(nuevasAsignaciones);
            await _context.SaveChangesAsync();
        }
    }

    // ========================================
    // MÉTODOS DE UTILIDAD
    // ========================================

    /// <summary>
    /// Guardar cambios en la base de datos
    /// </summary>
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
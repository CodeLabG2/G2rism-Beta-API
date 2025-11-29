using Microsoft.EntityFrameworkCore;
using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio de Códigos de Recuperación
/// Gestiona códigos de 6 dígitos para recuperación de contraseña
/// </summary>
public class CodigoRecuperacionRepository : ICodigoRecuperacionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CodigoRecuperacionRepository> _logger;
    private const int MAX_INTENTOS = 5;

    public CodigoRecuperacionRepository(
        ApplicationDbContext context,
        ILogger<CodigoRecuperacionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ========================================
    // MÉTODOS DE CONSULTA
    // ========================================

    /// <summary>
    /// Obtener un código por su valor (6 dígitos)
    /// </summary>
    public async Task<CodigoRecuperacion?> GetByCodigoAsync(string codigo)
    {
        return await _context.CodigosRecuperacion
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Codigo == codigo);
    }

    /// <summary>
    /// Obtener un código por su ID
    /// </summary>
    public async Task<CodigoRecuperacion?> GetByIdAsync(int idCodigo)
    {
        return await _context.CodigosRecuperacion
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.IdCodigo == idCodigo);
    }

    /// <summary>
    /// Obtener todos los códigos de un usuario
    /// </summary>
    public async Task<IEnumerable<CodigoRecuperacion>> GetByUsuarioIdAsync(int idUsuario)
    {
        return await _context.CodigosRecuperacion
            .Where(c => c.IdUsuario == idUsuario)
            .OrderByDescending(c => c.FechaGeneracion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtener códigos activos de un usuario
    /// </summary>
    public async Task<IEnumerable<CodigoRecuperacion>> GetCodigosActivosAsync(int idUsuario)
    {
        var ahora = DateTime.Now;

        return await _context.CodigosRecuperacion
            .Where(c => c.IdUsuario == idUsuario &&
                       !c.Usado &&
                       !c.Bloqueado &&
                       c.FechaExpiracion > ahora)
            .OrderByDescending(c => c.FechaGeneracion)
            .ToListAsync();
    }

    // ========================================
    // MÉTODOS DE VALIDACIÓN
    // ========================================

    /// <summary>
    /// Validar si un código es válido
    /// </summary>
    public async Task<bool> ValidarCodigoAsync(string codigo)
    {
        var ahora = DateTime.Now;

        return await _context.CodigosRecuperacion
            .AnyAsync(c => c.Codigo == codigo &&
                          !c.Usado &&
                          !c.Bloqueado &&
                          c.FechaExpiracion > ahora);
    }

    /// <summary>
    /// Verificar si un código está expirado
    /// </summary>
    public async Task<bool> EstaExpiradoAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        if (codigoObj == null)
            return true; // Si no existe, considerarlo expirado

        return codigoObj.FechaExpiracion <= DateTime.Now;
    }

    /// <summary>
    /// Verificar si un código ya fue usado
    /// </summary>
    public async Task<bool> EstaUsadoAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        return codigoObj?.Usado ?? false;
    }

    /// <summary>
    /// Verificar si un código está bloqueado
    /// </summary>
    public async Task<bool> EstaBloqueadoAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        return codigoObj?.Bloqueado ?? false;
    }

    // ========================================
    // MÉTODOS DE GESTIÓN
    // ========================================

    /// <summary>
    /// Crear un nuevo código
    /// </summary>
    public async Task<CodigoRecuperacion> CrearCodigoAsync(CodigoRecuperacion codigo)
    {
        await _context.CodigosRecuperacion.AddAsync(codigo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("✅ Código de recuperación creado | Usuario ID: {IdUsuario} | Código: {Codigo} | Expira: {Expiracion}",
            codigo.IdUsuario, codigo.Codigo, codigo.FechaExpiracion);

        return codigo;
    }

    /// <summary>
    /// Marcar un código como usado
    /// </summary>
    public async Task MarcarComoUsadoAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        if (codigoObj != null)
        {
            codigoObj.Usado = true;
            codigoObj.FechaUso = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Código marcado como usado | Código: {Codigo} | Usuario ID: {IdUsuario}",
                codigo, codigoObj.IdUsuario);
        }
    }

    /// <summary>
    /// Incrementar contador de intentos fallidos
    /// Bloquea el código si supera el máximo
    /// </summary>
    public async Task IncrementarIntentosAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        if (codigoObj != null)
        {
            codigoObj.IntentosValidacion++;

            if (codigoObj.IntentosValidacion >= MAX_INTENTOS)
            {
                codigoObj.Bloqueado = true;
                _logger.LogWarning("⚠️ Código BLOQUEADO por exceso de intentos | Código: {Codigo} | Intentos: {Intentos}",
                    codigo, codigoObj.IntentosValidacion);
            }
            else
            {
                _logger.LogWarning("⚠️ Intento fallido | Código: {Codigo} | Intento: {Intento}/{Max}",
                    codigo, codigoObj.IntentosValidacion, MAX_INTENTOS);
            }

            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Bloquear un código por exceso de intentos
    /// </summary>
    public async Task BloquearCodigoAsync(string codigo)
    {
        var codigoObj = await _context.CodigosRecuperacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);

        if (codigoObj != null)
        {
            codigoObj.Bloqueado = true;
            await _context.SaveChangesAsync();

            _logger.LogWarning("🔒 Código bloqueado manualmente | Código: {Codigo} | Usuario ID: {IdUsuario}",
                codigo, codigoObj.IdUsuario);
        }
    }

    /// <summary>
    /// Invalidar todos los códigos activos de un usuario
    /// </summary>
    public async Task InvalidarCodigosActivosAsync(int idUsuario)
    {
        var codigosActivos = await GetCodigosActivosAsync(idUsuario);

        foreach (var codigo in codigosActivos)
        {
            codigo.Usado = true;
            codigo.FechaUso = DateTime.Now;
        }

        if (codigosActivos.Any())
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("🗑️ Códigos activos invalidados | Usuario ID: {IdUsuario} | Cantidad: {Cantidad}",
                idUsuario, codigosActivos.Count());
        }
    }

    /// <summary>
    /// Limpiar códigos expirados (tarea de mantenimiento)
    /// </summary>
    public async Task LimpiarCodigosExpiradosAsync()
    {
        var ahora = DateTime.Now;

        var codigosExpirados = await _context.CodigosRecuperacion
            .Where(c => c.FechaExpiracion < ahora)
            .ToListAsync();

        if (codigosExpirados.Any())
        {
            _context.CodigosRecuperacion.RemoveRange(codigosExpirados);
            await _context.SaveChangesAsync();

            _logger.LogInformation("🧹 Códigos expirados eliminados | Cantidad: {Cantidad}", codigosExpirados.Count);
        }
    }

    /// <summary>
    /// Eliminar todos los códigos de un usuario
    /// </summary>
    public async Task EliminarCodigosDeUsuarioAsync(int idUsuario)
    {
        var codigos = await _context.CodigosRecuperacion
            .Where(c => c.IdUsuario == idUsuario)
            .ToListAsync();

        if (codigos.Any())
        {
            _context.CodigosRecuperacion.RemoveRange(codigos);
            await _context.SaveChangesAsync();

            _logger.LogInformation("🗑️ Todos los códigos del usuario eliminados | Usuario ID: {IdUsuario} | Cantidad: {Cantidad}",
                idUsuario, codigos.Count);
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

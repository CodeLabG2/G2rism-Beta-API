using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio de aerolíneas
/// Maneja todas las operaciones de acceso a datos
/// </summary>
public class AerolineaRepository : IAerolineaRepository
{
    private readonly ApplicationDbContext _context;

    public AerolineaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Aerolinea>> GetAllAsync()
    {
        return await _context.Aerolineas
            .OrderBy(a => a.Nombre)
            .ToListAsync();
    }

    public async Task<Aerolinea?> GetByIdAsync(int id)
    {
        return await _context.Aerolineas
            .FirstOrDefaultAsync(a => a.IdAerolinea == id);
    }

    public async Task<Aerolinea?> GetByCodigoIataAsync(string codigoIata)
    {
        var codigoUpper = codigoIata.ToUpper();
        return await _context.Aerolineas
            .FirstOrDefaultAsync(a => a.CodigoIata.ToUpper() == codigoUpper);
    }

    public async Task<Aerolinea?> GetByCodigoIcaoAsync(string codigoIcao)
    {
        var codigoUpper = codigoIcao.ToUpper();
        return await _context.Aerolineas
            .FirstOrDefaultAsync(a => a.CodigoIcao.ToUpper() == codigoUpper);
    }

    public async Task<IEnumerable<Aerolinea>> GetByPaisAsync(string pais)
    {
        return await _context.Aerolineas
            .Where(a => a.Pais.ToLower().Contains(pais.ToLower()))
            .OrderBy(a => a.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Aerolinea>> GetByEstadoAsync(string estado)
    {
        return await _context.Aerolineas
            .Where(a => a.Estado.ToLower() == estado.ToLower())
            .OrderBy(a => a.Nombre)
            .ToListAsync();
    }

    public async Task<bool> ExistsCodigoIataAsync(string codigoIata, int? excludeId = null)
    {
        var codigoUpper = codigoIata.ToUpper();
        var query = _context.Aerolineas
            .Where(a => a.CodigoIata.ToUpper() == codigoUpper);

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.IdAerolinea != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsCodigoIcaoAsync(string codigoIcao, int? excludeId = null)
    {
        var codigoUpper = codigoIcao.ToUpper();
        var query = _context.Aerolineas
            .Where(a => a.CodigoIcao.ToUpper() == codigoUpper);

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.IdAerolinea != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Aerolinea> CreateAsync(Aerolinea aerolinea)
    {
        // Normalizar códigos a mayúsculas
        aerolinea.CodigoIata = aerolinea.CodigoIata.ToUpper();
        aerolinea.CodigoIcao = aerolinea.CodigoIcao.ToUpper();
        aerolinea.FechaCreacion = DateTime.Now;

        _context.Aerolineas.Add(aerolinea);
        await _context.SaveChangesAsync();
        return aerolinea;
    }

    public async Task<bool> UpdateAsync(Aerolinea aerolinea)
    {
        var exists = await _context.Aerolineas
            .AnyAsync(a => a.IdAerolinea == aerolinea.IdAerolinea);

        if (!exists) return false;

        aerolinea.FechaModificacion = DateTime.Now;
        _context.Aerolineas.Update(aerolinea);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var aerolinea = await GetByIdAsync(id);
        if (aerolinea == null) return false;

        // Soft delete: cambiar estado a "Inactiva"
        aerolinea.Estado = "Inactiva";
        aerolinea.FechaModificacion = DateTime.Now;

        _context.Aerolineas.Update(aerolinea);
        await _context.SaveChangesAsync();
        return true;
    }
}

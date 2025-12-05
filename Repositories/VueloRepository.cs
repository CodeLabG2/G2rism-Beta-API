using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio de vuelos
/// Maneja todas las operaciones de acceso a datos
/// </summary>
public class VueloRepository : IVueloRepository
{
    private readonly ApplicationDbContext _context;

    public VueloRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vuelo>> GetAllAsync()
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<Vuelo?> GetByIdAsync(int id)
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .FirstOrDefaultAsync(v => v.IdVuelo == id);
    }

    public async Task<IEnumerable<Vuelo>> GetByOrigenDestinoAsync(string origen, string destino)
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .Where(v => v.Origen.ToLower().Contains(origen.ToLower())
                     && v.Destino.ToLower().Contains(destino.ToLower()))
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vuelo>> GetByFechaSalidaAsync(DateTime fecha)
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .Where(v => v.FechaSalida.Date == fecha.Date)
            .OrderBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vuelo>> GetDisponiblesAsync()
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .Where(v => v.CuposDisponibles > 0
                     && v.Estado
                     && v.FechaSalida > DateTime.Now)
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vuelo>> GetByAerolineaAsync(int idAerolinea)
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .Where(v => v.IdAerolinea == idAerolinea)
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vuelo>> GetByProveedorAsync(int idProveedor)
    {
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .Where(v => v.IdProveedor == idProveedor)
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }

    public async Task<Vuelo?> GetByNumeroVueloAsync(string numeroVuelo)
    {
        var numeroUpper = numeroVuelo.ToUpper();
        return await _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .FirstOrDefaultAsync(v => v.NumeroVuelo.ToUpper() == numeroUpper);
    }

    public async Task<bool> ExistsNumeroVueloAsync(string numeroVuelo, int? excludeId = null)
    {
        var numeroUpper = numeroVuelo.ToUpper();
        var query = _context.Vuelos
            .Where(v => v.NumeroVuelo.ToUpper() == numeroUpper);

        if (excludeId.HasValue)
        {
            query = query.Where(v => v.IdVuelo != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Vuelo> CreateAsync(Vuelo vuelo)
    {
        _context.Vuelos.Add(vuelo);
        await _context.SaveChangesAsync();

        // Recargar con relaciones
        return (await GetByIdAsync(vuelo.IdVuelo))!;
    }

    public async Task<bool> UpdateAsync(Vuelo vuelo)
    {
        _context.Vuelos.Update(vuelo);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vuelo = await GetByIdAsync(id);
        if (vuelo == null)
            return false;

        vuelo.Estado = false;
        return await UpdateAsync(vuelo);
    }

    public async Task<IEnumerable<Vuelo>> BuscarAsync(string? origen, string? destino, DateTime? fecha)
    {
        var query = _context.Vuelos
            .Include(v => v.Aerolinea)
            .Include(v => v.Proveedor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(origen))
        {
            query = query.Where(v => v.Origen.ToLower().Contains(origen.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(destino))
        {
            query = query.Where(v => v.Destino.ToLower().Contains(destino.ToLower()));
        }

        if (fecha.HasValue)
        {
            query = query.Where(v => v.FechaSalida.Date == fecha.Value.Date);
        }

        return await query
            .OrderBy(v => v.FechaSalida)
            .ThenBy(v => v.HoraSalida)
            .ToListAsync();
    }
}

using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio de paquetes turísticos con búsquedas avanzadas
/// </summary>
public class PaqueteTuristicoRepository : GenericRepository<PaqueteTuristico>, IPaqueteTuristicoRepository
{
    public PaqueteTuristicoRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetAllAsync()
    {
        return await _context.PaquetesTuristicos
            .OrderBy(p => p.DestinoPrincipal)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<PaqueteTuristico?> GetByIdAsync(int id)
    {
        return await _context.PaquetesTuristicos
            .FirstOrDefaultAsync(p => p.IdPaquete == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetByDestinoAsync(string destino)
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.DestinoPrincipal.ToLower().Contains(destino.ToLower()))
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetByTipoAsync(string tipo)
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.TipoPaquete != null && p.TipoPaquete.ToLower() == tipo.ToLower())
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetDisponiblesAsync()
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.Estado && p.CuposDisponibles > 0)
            .OrderBy(p => p.DestinoPrincipal)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetByDuracionAsync(int diasMin, int diasMax)
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.Duracion >= diasMin && p.Duracion <= diasMax)
            .OrderBy(p => p.Duracion)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.Precio >= precioMin && p.Precio <= precioMax)
            .OrderBy(p => p.Precio)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetByNivelDificultadAsync(string nivel)
    {
        return await _context.PaquetesTuristicos
            .Where(p => p.NivelDificultad != null && p.NivelDificultad.ToLower() == nivel.ToLower())
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetProximosAIniciarAsync()
    {
        var hoy = DateTime.Now.Date;
        var dentroDeUnaSemana = hoy.AddDays(7);

        return await _context.PaquetesTuristicos
            .Where(p => p.Estado &&
                        p.FechaInicio.HasValue &&
                        p.FechaInicio.Value.Date >= hoy &&
                        p.FechaInicio.Value.Date <= dentroDeUnaSemana)
            .OrderBy(p => p.FechaInicio)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristico>> GetVigentesAsync()
    {
        var hoy = DateTime.Now.Date;

        return await _context.PaquetesTuristicos
            .Where(p => p.Estado &&
                        ((p.FechaInicio.HasValue && p.FechaFin.HasValue &&
                          p.FechaInicio.Value.Date <= hoy &&
                          p.FechaFin.Value.Date >= hoy) ||
                         (!p.FechaInicio.HasValue && !p.FechaFin.HasValue)))
            .OrderBy(p => p.DestinoPrincipal)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistePorNombreAsync(string nombre, int? idPaqueteExcluir = null)
    {
        var query = _context.PaquetesTuristicos
            .Where(p => p.Nombre.ToLower() == nombre.ToLower());

        if (idPaqueteExcluir.HasValue)
        {
            query = query.Where(p => p.IdPaquete != idPaqueteExcluir.Value);
        }

        return await query.AnyAsync();
    }
}

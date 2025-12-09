using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Repositorio para la gestión de ReservaHotel con métodos personalizados
/// </summary>
public class ReservaHotelRepository : GenericRepository<ReservaHotel>, IReservaHotelRepository
{
    public ReservaHotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReservaHotel>> GetHotelesByReservaAsync(int idReserva)
    {
        return await _context.ReservasHoteles
            .Include(rh => rh.Hotel)
                .ThenInclude(h => h.Proveedor)
            .Where(rh => rh.IdReserva == idReserva)
            .OrderBy(rh => rh.FechaCheckin)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ReservaHotel?> GetByIdConHotelAsync(int id)
    {
        return await _context.ReservasHoteles
            .Include(rh => rh.Hotel)
                .ThenInclude(h => h.Proveedor)
            .Include(rh => rh.Reserva)
            .FirstOrDefaultAsync(rh => rh.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> ExisteHotelEnReservaAsync(int idReserva, int idHotel)
    {
        return await _context.ReservasHoteles
            .AnyAsync(rh => rh.IdReserva == idReserva && rh.IdHotel == idHotel);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReservaHotel>> GetReservasPorHotelYFechasAsync(int idHotel, DateTime fechaInicio, DateTime fechaFin)
    {
        return await _context.ReservasHoteles
            .Include(rh => rh.Reserva)
                .ThenInclude(r => r.Cliente)
            .Where(rh => rh.IdHotel == idHotel &&
                        rh.FechaCheckin <= fechaFin &&
                        rh.FechaCheckout >= fechaInicio)
            .OrderBy(rh => rh.FechaCheckin)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<decimal> CalcularTotalHotelesPorReservaAsync(int idReserva)
    {
        var total = await _context.ReservasHoteles
            .Where(rh => rh.IdReserva == idReserva)
            .SumAsync(rh => rh.Subtotal);

        return total;
    }
}

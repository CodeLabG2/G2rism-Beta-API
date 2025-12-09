using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de paquetes en reservas
/// </summary>
public class ReservaPaqueteRepository : GenericRepository<ReservaPaquete>, IReservaPaqueteRepository
{
    public ReservaPaqueteRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene todos los paquetes de una reserva específica
    /// </summary>
    public async Task<IEnumerable<ReservaPaquete>> GetByReservaIdAsync(int idReserva)
    {
        return await _context.Set<ReservaPaquete>()
            .Where(rp => rp.IdReserva == idReserva)
            .OrderBy(rp => rp.FechaInicioPaquete)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una relación reserva-paquete con todos sus datos relacionados
    /// </summary>
    public async Task<ReservaPaquete?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<ReservaPaquete>()
            .Include(rp => rp.Paquete)
            .Include(rp => rp.Reserva)
            .FirstOrDefaultAsync(rp => rp.Id == id);
    }

    /// <summary>
    /// Obtiene todos los paquetes de una reserva con detalles completos
    /// </summary>
    public async Task<IEnumerable<ReservaPaquete>> GetByReservaIdWithDetailsAsync(int idReserva)
    {
        return await _context.Set<ReservaPaquete>()
            .Include(rp => rp.Paquete)
            .Where(rp => rp.IdReserva == idReserva)
            .OrderBy(rp => rp.FechaInicioPaquete)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe una relación específica entre reserva y paquete
    /// </summary>
    public async Task<bool> ExistsReservaPaqueteAsync(int idReserva, int idPaquete)
    {
        return await _context.Set<ReservaPaquete>()
            .AnyAsync(rp => rp.IdReserva == idReserva && rp.IdPaquete == idPaquete);
    }

    /// <summary>
    /// Calcula el subtotal de todos los paquetes de una reserva
    /// </summary>
    public async Task<decimal> GetTotalPaquetesByReservaAsync(int idReserva)
    {
        var paquetes = await _context.Set<ReservaPaquete>()
            .Where(rp => rp.IdReserva == idReserva)
            .ToListAsync();

        return paquetes.Sum(rp => rp.Subtotal);
    }
}
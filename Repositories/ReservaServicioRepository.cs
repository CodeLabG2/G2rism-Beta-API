using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;

namespace G2rismBeta.API.Repositories;

/// <summary>
/// Repositorio para la entidad ReservaServicio
/// Implementa operaciones de acceso a datos para la relación Reservas-Servicios
/// </summary>
public class ReservaServicioRepository : GenericRepository<ReservaServicio>, IReservaServicioRepository
{
    public ReservaServicioRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene todos los servicios de una reserva con información del servicio y proveedor
    /// </summary>
    public async Task<IEnumerable<ReservaServicio>> GetServiciosByReservaIdAsync(int idReserva)
    {
        return await _context.ReservasServicios
            .Where(rs => rs.IdReserva == idReserva)
            .Include(rs => rs.Servicio)
                .ThenInclude(s => s!.Proveedor)
            .OrderBy(rs => rs.FechaAgregado)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una reserva de servicio con información completa del servicio
    /// </summary>
    public async Task<ReservaServicio?> GetReservaServicioConDetallesAsync(int id)
    {
        return await _context.ReservasServicios
            .Include(rs => rs.Reserva)
            .Include(rs => rs.Servicio)
                .ThenInclude(s => s!.Proveedor)
            .FirstOrDefaultAsync(rs => rs.Id == id);
    }

    /// <summary>
    /// Verifica si existe una reserva de servicio
    /// </summary>
    public async Task<bool> ExisteReservaServicioAsync(int id)
    {
        return await _context.ReservasServicios.AnyAsync(rs => rs.Id == id);
    }

    /// <summary>
    /// Cuenta cuántos servicios tiene una reserva
    /// </summary>
    public async Task<int> ContarServiciosPorReservaAsync(int idReserva)
    {
        return await _context.ReservasServicios
            .Where(rs => rs.IdReserva == idReserva)
            .CountAsync();
    }

    /// <summary>
    /// Obtiene servicios de una reserva filtrados por estado
    /// </summary>
    public async Task<IEnumerable<ReservaServicio>> GetServiciosByReservaYEstadoAsync(int idReserva, string estado)
    {
        return await _context.ReservasServicios
            .Where(rs => rs.IdReserva == idReserva && rs.Estado.ToLower() == estado.ToLower())
            .Include(rs => rs.Servicio)
                .ThenInclude(s => s!.Proveedor)
            .OrderBy(rs => rs.FechaServicio ?? DateTime.MaxValue)
            .ThenBy(rs => rs.FechaAgregado)
            .ToListAsync();
    }
}
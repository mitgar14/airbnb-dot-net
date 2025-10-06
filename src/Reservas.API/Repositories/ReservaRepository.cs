using Microsoft.EntityFrameworkCore;
using Reservas.API.Data;
using Reservas.API.Data.Entities;

namespace Reservas.API.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly AppDbContext _context;

    public ReservaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reserva>> GetAllAsync()
    {
        return await _context.Reservas.ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetByUserIdAsync(string userId)
    {
        return await _context.Reservas
            .Where(r => r.ClientId == userId)
            .ToListAsync();
    }

    public async Task<Reserva?> GetByReservationIdAsync(int reservationId)
    {
        return await _context.Reservas.FindAsync(reservationId);
    }

    public async Task<IEnumerable<Reserva>> GetByHostIdAsync(string hostId)
    {
        return await _context.Reservas
            .Where(r => r.HostId == hostId)
            .ToListAsync();
    }

    public async Task<Reserva> CreateAsync(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task<bool> DeleteAsync(int reservationId)
    {
        var reserva = await _context.Reservas.FindAsync(reservationId);
        if (reserva == null)
        {
            return false;
        }

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync();
        return true;
    }
}

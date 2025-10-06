using Reservas.API.Data.Entities;

namespace Reservas.API.Repositories;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> GetAllAsync();
    Task<IEnumerable<Reserva>> GetByUserIdAsync(string userId);
    Task<Reserva?> GetByReservationIdAsync(int reservationId);
    Task<IEnumerable<Reserva>> GetByHostIdAsync(string hostId);
    Task<Reserva> CreateAsync(Reserva reserva);
    Task<bool> DeleteAsync(int reservationId);
}

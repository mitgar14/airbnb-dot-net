using Reservas.API.DTOs;

namespace Reservas.API.Services;

public interface IReservaService
{
    Task<IEnumerable<ReservaDto>> GetAllReservasAsync();
    Task<ReservaDto> CreateReservaAsync(CreateReservaDto createReservaDto, string currentUserId, string currentUserRole);
    Task<IEnumerable<ReservaDto>> GetReservasByUserIdAsync(string userId, string currentUserId, string currentUserRole);
    Task<IEnumerable<ReservaDto>> GetReservasByHostIdAsync(string hostId, string currentUserId, string currentUserRole);
    Task<ReservaDto?> GetReservaByIdAsync(int reservationId, string currentUserId, string currentUserRole);
    Task DeleteReservaAsync(int reservationId, string currentUserId, string currentUserRole);
}

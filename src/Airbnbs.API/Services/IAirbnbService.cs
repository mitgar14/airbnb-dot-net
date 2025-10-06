using Airbnbs.API.DTOs;

namespace Airbnbs.API.Services;

public interface IAirbnbService
{
    Task<IEnumerable<AirbnbDto>> GetAllAirbnbsAsync();
    Task<AirbnbDto?> GetAirbnbByIdAsync(string id);
    Task<IEnumerable<AirbnbDto>> GetAirbnbsByHostIdAsync(string hostId);
    Task<IEnumerable<AirbnbDto>> GetAirbnbsByRoomTypeAsync(string roomType);
    Task<AirbnbDto> CreateAirbnbAsync(CreateAirbnbDto createAirbnbDto, string currentUserId, string currentUserRole);
    Task<AirbnbDto> UpdateAirbnbAsync(string id, UpdateAirbnbDto updateAirbnbDto, string currentUserId, string currentUserRole);
    Task DeleteAirbnbAsync(string id, string currentUserId, string currentUserRole);
}

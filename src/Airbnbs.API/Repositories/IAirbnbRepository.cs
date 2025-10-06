using Airbnbs.API.Data.Entities;

namespace Airbnbs.API.Repositories;

public interface IAirbnbRepository
{
    Task<IEnumerable<Data.Entities.Airbnb>> GetAllAsync();
    Task<Data.Entities.Airbnb?> GetByIdAsync(string id);
    Task<IEnumerable<Data.Entities.Airbnb>> GetByHostIdAsync(string hostId);
    Task<IEnumerable<Data.Entities.Airbnb>> GetByRoomTypeAsync(string roomType);
    Task<Data.Entities.Airbnb> CreateAsync(Data.Entities.Airbnb airbnb);
    Task<Data.Entities.Airbnb?> UpdateAsync(string id, Data.Entities.Airbnb airbnb);
    Task<bool> DeleteAsync(string id);
}

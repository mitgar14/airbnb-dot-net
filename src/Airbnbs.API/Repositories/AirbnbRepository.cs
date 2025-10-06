using Microsoft.EntityFrameworkCore;
using Airbnbs.API.Data;
using Airbnbs.API.Data.Entities;

namespace Airbnbs.API.Repositories;

public class AirbnbRepository : IAirbnbRepository
{
    private readonly AppDbContext _context;

    public AirbnbRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Data.Entities.Airbnb>> GetAllAsync()
    {
        return await _context.Airbnbs.ToListAsync();
    }

    public async Task<Data.Entities.Airbnb?> GetByIdAsync(string id)
    {
        return await _context.Airbnbs.FindAsync(id);
    }

    public async Task<IEnumerable<Data.Entities.Airbnb>> GetByHostIdAsync(string hostId)
    {
        return await _context.Airbnbs
            .Where(a => a.HostId == hostId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Data.Entities.Airbnb>> GetByRoomTypeAsync(string roomType)
    {
        return await _context.Airbnbs
            .Where(a => a.RoomType == roomType)
            .ToListAsync();
    }

    public async Task<Data.Entities.Airbnb> CreateAsync(Data.Entities.Airbnb airbnb)
    {
        _context.Airbnbs.Add(airbnb);
        await _context.SaveChangesAsync();
        return airbnb;
    }

    public async Task<Data.Entities.Airbnb?> UpdateAsync(string id, Data.Entities.Airbnb airbnb)
    {
        var existingAirbnb = await _context.Airbnbs.FindAsync(id);
        if (existingAirbnb == null)
        {
            return null;
        }

        existingAirbnb.Name = airbnb.Name;
        existingAirbnb.HostId = airbnb.HostId;
        existingAirbnb.HostName = airbnb.HostName;
        existingAirbnb.NeighbourhoodGroup = airbnb.NeighbourhoodGroup;
        existingAirbnb.Neighbourhood = airbnb.Neighbourhood;
        existingAirbnb.Latitude = airbnb.Latitude;
        existingAirbnb.Longitude = airbnb.Longitude;
        existingAirbnb.RoomType = airbnb.RoomType;
        existingAirbnb.Price = airbnb.Price;
        existingAirbnb.MinimumNights = airbnb.MinimumNights;
        existingAirbnb.NumberOfReviews = airbnb.NumberOfReviews;
        existingAirbnb.Rating = airbnb.Rating;
        existingAirbnb.Rooms = airbnb.Rooms;
        existingAirbnb.Beds = airbnb.Beds;
        existingAirbnb.Bathrooms = airbnb.Bathrooms;

        await _context.SaveChangesAsync();
        return existingAirbnb;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var airbnb = await _context.Airbnbs.FindAsync(id);
        if (airbnb == null)
        {
            return false;
        }

        _context.Airbnbs.Remove(airbnb);
        await _context.SaveChangesAsync();
        return true;
    }
}

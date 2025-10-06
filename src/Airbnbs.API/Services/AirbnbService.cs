using AutoMapper;
using Airbnbs.API.Data.Entities;
using Airbnbs.API.DTOs;
using Airbnbs.API.Repositories;

namespace Airbnbs.API.Services;

public class AirbnbService : IAirbnbService
{
    private readonly IAirbnbRepository _repository;
    private readonly IMapper _mapper;

    public AirbnbService(IAirbnbRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AirbnbDto>> GetAllAirbnbsAsync()
    {
        var airbnbs = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AirbnbDto>>(airbnbs);
    }

    public async Task<AirbnbDto?> GetAirbnbByIdAsync(string id)
    {
        var airbnb = await _repository.GetByIdAsync(id);
        return airbnb != null ? _mapper.Map<AirbnbDto>(airbnb) : null;
    }

    public async Task<IEnumerable<AirbnbDto>> GetAirbnbsByHostIdAsync(string hostId)
    {
        var airbnbs = await _repository.GetByHostIdAsync(hostId);
        return _mapper.Map<IEnumerable<AirbnbDto>>(airbnbs);
    }

    public async Task<IEnumerable<AirbnbDto>> GetAirbnbsByRoomTypeAsync(string roomType)
    {
        var airbnbs = await _repository.GetByRoomTypeAsync(roomType);
        return _mapper.Map<IEnumerable<AirbnbDto>>(airbnbs);
    }

    public async Task<AirbnbDto> CreateAirbnbAsync(CreateAirbnbDto createAirbnbDto, string currentUserId, string currentUserRole)
    {
        // Validar que Host solo pueda crear para sí mismo
        if (currentUserRole == "Host" && createAirbnbDto.HostId != currentUserId)
        {
            throw new UnauthorizedAccessException("Un Host solo puede crear propiedades para sí mismo");
        }
        
        // Admin puede crear para cualquier host
        var airbnb = _mapper.Map<Data.Entities.Airbnb>(createAirbnbDto);
        var createdAirbnb = await _repository.CreateAsync(airbnb);
        return _mapper.Map<AirbnbDto>(createdAirbnb);
    }

    public async Task<AirbnbDto> UpdateAirbnbAsync(string id, UpdateAirbnbDto updateAirbnbDto, string currentUserId, string currentUserRole)
    {
        var existingAirbnb = await _repository.GetByIdAsync(id);
        if (existingAirbnb == null)
            throw new KeyNotFoundException($"Propiedad con ID {id} no encontrada");
        
        // Validar ownership - Host solo puede editar sus propiedades
        if (currentUserRole == "Host" && existingAirbnb.HostId != currentUserId)
        {
            throw new UnauthorizedAccessException("No tiene permiso para modificar esta propiedad");
        }
        
        // Admin puede editar cualquier propiedad
        
        // Actualización parcial - solo actualizar campos no nulos
        if (updateAirbnbDto.Name != null) existingAirbnb.Name = updateAirbnbDto.Name;
        if (updateAirbnbDto.HostId != null) existingAirbnb.HostId = updateAirbnbDto.HostId;
        if (updateAirbnbDto.HostName != null) existingAirbnb.HostName = updateAirbnbDto.HostName;
        if (updateAirbnbDto.NeighbourhoodGroup != null) existingAirbnb.NeighbourhoodGroup = updateAirbnbDto.NeighbourhoodGroup;
        if (updateAirbnbDto.Neighbourhood != null) existingAirbnb.Neighbourhood = updateAirbnbDto.Neighbourhood;
        if (updateAirbnbDto.Latitude != null) existingAirbnb.Latitude = updateAirbnbDto.Latitude;
        if (updateAirbnbDto.Longitude != null) existingAirbnb.Longitude = updateAirbnbDto.Longitude;
        if (updateAirbnbDto.RoomType != null) existingAirbnb.RoomType = updateAirbnbDto.RoomType;
        if (updateAirbnbDto.Price != null) existingAirbnb.Price = updateAirbnbDto.Price;
        if (updateAirbnbDto.MinimumNights != null) existingAirbnb.MinimumNights = updateAirbnbDto.MinimumNights;
        if (updateAirbnbDto.NumberOfReviews != null) existingAirbnb.NumberOfReviews = updateAirbnbDto.NumberOfReviews;
        if (updateAirbnbDto.Rating != null) existingAirbnb.Rating = updateAirbnbDto.Rating;
        if (updateAirbnbDto.Rooms != null) existingAirbnb.Rooms = updateAirbnbDto.Rooms;
        if (updateAirbnbDto.Beds != null) existingAirbnb.Beds = updateAirbnbDto.Beds;
        if (updateAirbnbDto.Bathrooms != null) existingAirbnb.Bathrooms = updateAirbnbDto.Bathrooms;
        
        var updatedAirbnb = await _repository.UpdateAsync(id, existingAirbnb);
        return _mapper.Map<AirbnbDto>(updatedAirbnb!);
    }

    public async Task DeleteAirbnbAsync(string id, string currentUserId, string currentUserRole)
    {
        var airbnb = await _repository.GetByIdAsync(id);
        if (airbnb == null)
            throw new KeyNotFoundException($"Propiedad con ID {id} no encontrada");
        
        // Validar ownership - Host solo puede eliminar sus propiedades
        if (currentUserRole == "Host" && airbnb.HostId != currentUserId)
        {
            throw new UnauthorizedAccessException("No tiene permiso para eliminar esta propiedad");
        }
        
        // Admin puede eliminar cualquier propiedad
        await _repository.DeleteAsync(id);
    }
}

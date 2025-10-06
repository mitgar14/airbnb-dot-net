using AutoMapper;
using Reservas.API.Data.Entities;
using Reservas.API.DTOs;
using Reservas.API.Repositories;

namespace Reservas.API.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _repository;
    private readonly IMicroserviceClient _microserviceClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservaService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReservaService(
        IReservaRepository repository, 
        IMicroserviceClient microserviceClient,
        IMapper mapper,
        ILogger<ReservaService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _microserviceClient = microserviceClient;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ReservaDto>> GetAllReservasAsync()
    {
        var reservas = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
    }

    public async Task<IEnumerable<ReservaDto>> GetReservasByUserIdAsync(string userId)
    {
        var reservas = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
    }

    public async Task<ReservaDto?> GetReservaByIdAsync(int reservationId)
    {
        var reserva = await _repository.GetByReservationIdAsync(reservationId);
        return reserva != null ? _mapper.Map<ReservaDto>(reserva) : null;
    }

    public async Task<IEnumerable<ReservaDto>> GetReservasByHostIdAsync(string hostId)
    {
        var reservas = await _repository.GetByHostIdAsync(hostId);
        return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
    }

    public async Task<ReservaDto> CreateReservaAsync(CreateReservaDto createReservaDto, string currentUserId, string currentUserRole)
    {
        // Validar que Cliente solo pueda reservar para sí mismo
        if (currentUserRole == "Cliente" && createReservaDto.ClientId != currentUserId)
        {
            throw new UnauthorizedAccessException("Un Cliente solo puede crear reservas para sí mismo");
        }
        
        // Admin puede crear para cualquier usuario
        
        // Extraer token del HttpContext
        var authToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        
        // Get client information from Usuarios API con token
        var cliente = await _microserviceClient.GetUsuarioAsync(createReservaDto.ClientId, authToken);
        if (cliente == null)
        {
            throw new InvalidOperationException($"Usuario {createReservaDto.ClientId} no encontrado");
        }

        // Get airbnb information from Airbnbs API con token
        var airbnb = await _microserviceClient.GetAirbnbAsync(createReservaDto.AirbnbId, authToken);
        if (airbnb == null)
        {
            throw new InvalidOperationException($"Airbnb {createReservaDto.AirbnbId} no encontrado");
        }

        // Create reserva with complete information
        var reserva = new Reserva
        {
            AirbnbId = airbnb.Id,
            AirbnbName = airbnb.Name,
            HostId = airbnb.HostId,
            ClientId = createReservaDto.ClientId,
            ClientName = cliente.Name,
            ReservationDate = DateTime.Now
        };

        var createdReserva = await _repository.CreateAsync(reserva);
        return _mapper.Map<ReservaDto>(createdReserva);
    }
    
    public async Task<IEnumerable<ReservaDto>> GetReservasByUserIdAsync(string userId, string currentUserId, string currentUserRole)
    {
        // Validar que Cliente solo pueda ver sus propias reservas
        if (currentUserRole == "Cliente" && userId != currentUserId)
        {
            throw new UnauthorizedAccessException("No tiene permiso para ver las reservas de otro usuario");
        }
        
        // Admin puede ver reservas de cualquier usuario
        var reservas = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
    }
    
    public async Task<IEnumerable<ReservaDto>> GetReservasByHostIdAsync(string hostId, string currentUserId, string currentUserRole)
    {
        // Validar que Host solo pueda ver reservas de sus propiedades
        if (currentUserRole == "Host" && hostId != currentUserId)
        {
            throw new UnauthorizedAccessException("No tiene permiso para ver las reservas de otro host");
        }
        
        // Admin puede ver reservas de cualquier host
        var reservas = await _repository.GetByHostIdAsync(hostId);
        return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
    }
    
    public async Task<ReservaDto?> GetReservaByIdAsync(int reservationId, string currentUserId, string currentUserRole)
    {
        var reserva = await _repository.GetByReservationIdAsync(reservationId);
        if (reserva == null)
            return null;
        
        // Admin puede ver cualquier reserva
        if (currentUserRole == "Admin")
            return _mapper.Map<ReservaDto>(reserva);
        
        // Cliente puede ver si es su reserva
        if (currentUserRole == "Cliente" && reserva.ClientId == currentUserId)
            return _mapper.Map<ReservaDto>(reserva);
        
        // Host puede ver si es reserva de su propiedad
        if (currentUserRole == "Host")
        {
            // Extraer token del HttpContext
            var authToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");
                
            var airbnb = await _microserviceClient.GetAirbnbAsync(reserva.AirbnbId, authToken);
            if (airbnb?.HostId == currentUserId)
                return _mapper.Map<ReservaDto>(reserva);
        }
        
        throw new UnauthorizedAccessException("No tiene permiso para ver esta reserva");
    }

    public async Task DeleteReservaAsync(int reservationId, string currentUserId, string currentUserRole)
    {
        var reserva = await _repository.GetByReservationIdAsync(reservationId);
        if (reserva == null)
            throw new KeyNotFoundException($"Reserva con ID {reservationId} no encontrada");
        
        // Validar que Cliente solo pueda cancelar sus propias reservas
        if (currentUserRole == "Cliente" && reserva.ClientId != currentUserId)
        {
            throw new UnauthorizedAccessException("No tiene permiso para cancelar esta reserva");
        }
        
        // Admin puede cancelar cualquier reserva
        await _repository.DeleteAsync(reservationId);
    }
}

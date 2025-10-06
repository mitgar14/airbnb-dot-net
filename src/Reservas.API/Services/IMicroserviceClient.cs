using Reservas.API.DTOs;

namespace Reservas.API.Services;

public interface IMicroserviceClient
{
    Task<UsuarioDto?> GetUsuarioAsync(string userId, string? authToken = null);
    Task<AirbnbDto?> GetAirbnbAsync(string airbnbId, string? authToken = null);
}

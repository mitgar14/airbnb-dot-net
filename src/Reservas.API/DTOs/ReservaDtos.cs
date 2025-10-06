namespace Reservas.API.DTOs;

public class ReservaDto
{
    public int ReservationId { get; set; }
    public string AirbnbId { get; set; } = string.Empty;
    public string AirbnbName { get; set; } = string.Empty;
    public string HostId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime? ReservationDate { get; set; }
}

public class CreateReservaDto
{
    public string ClientId { get; set; } = string.Empty;
    public string AirbnbId { get; set; } = string.Empty;
}

// DTOs for external service responses
public class UsuarioDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class AirbnbDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string HostId { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string NeighbourhoodGroup { get; set; } = string.Empty;
    public string Neighbourhood { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string MinimumNights { get; set; } = string.Empty;
    public string NumberOfReviews { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string Rooms { get; set; } = string.Empty;
    public string Beds { get; set; } = string.Empty;
    public string Bathrooms { get; set; } = string.Empty;
}

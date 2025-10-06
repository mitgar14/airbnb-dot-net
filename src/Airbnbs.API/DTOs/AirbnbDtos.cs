namespace Airbnbs.API.DTOs;

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

public class CreateAirbnbDto
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

public class UpdateAirbnbDto
{
    public string? Name { get; set; }
    public string? HostId { get; set; }
    public string? HostName { get; set; }
    public string? NeighbourhoodGroup { get; set; }
    public string? Neighbourhood { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? RoomType { get; set; }
    public string? Price { get; set; }
    public string? MinimumNights { get; set; }
    public string? NumberOfReviews { get; set; }
    public string? Rating { get; set; }
    public string? Rooms { get; set; }
    public string? Beds { get; set; }
    public string? Bathrooms { get; set; }
}

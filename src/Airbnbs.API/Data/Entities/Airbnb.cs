using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airbnbs.API.Data.Entities;

[Table("micro_airbnbs")]
public class Airbnb
{
    [Key]
    [Column("id")]
    [MaxLength(255)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("host_id")]
    [MaxLength(255)]
    public string HostId { get; set; } = string.Empty;

    [Required]
    [Column("host_name")]
    [MaxLength(255)]
    public string HostName { get; set; } = string.Empty;

    [Required]
    [Column("neighbourhood_group")]
    [MaxLength(255)]
    public string NeighbourhoodGroup { get; set; } = string.Empty;

    [Required]
    [Column("neighbourhood")]
    [MaxLength(255)]
    public string Neighbourhood { get; set; } = string.Empty;

    [Required]
    [Column("latitude")]
    [MaxLength(255)]
    public string Latitude { get; set; } = string.Empty;

    [Required]
    [Column("longitude")]
    [MaxLength(255)]
    public string Longitude { get; set; } = string.Empty;

    [Required]
    [Column("room_type")]
    [MaxLength(255)]
    public string RoomType { get; set; } = string.Empty;

    [Required]
    [Column("price")]
    [MaxLength(255)]
    public string Price { get; set; } = string.Empty;

    [Required]
    [Column("minimum_nights")]
    [MaxLength(255)]
    public string MinimumNights { get; set; } = string.Empty;

    [Required]
    [Column("number_of_reviews")]
    [MaxLength(255)]
    public string NumberOfReviews { get; set; } = string.Empty;

    [Required]
    [Column("rating")]
    [MaxLength(255)]
    public string Rating { get; set; } = string.Empty;

    [Required]
    [Column("rooms")]
    [MaxLength(255)]
    public string Rooms { get; set; } = string.Empty;

    [Required]
    [Column("beds")]
    [MaxLength(255)]
    public string Beds { get; set; } = string.Empty;

    [Required]
    [Column("bathrooms")]
    [MaxLength(255)]
    public string Bathrooms { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservas.API.Data.Entities;

[Table("micro_reservas")]
public class Reserva
{
    [Key]
    [Column("reservation_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReservationId { get; set; }

    [Required]
    [Column("airbnb_id")]
    [MaxLength(255)]
    public string AirbnbId { get; set; } = string.Empty;

    [Required]
    [Column("airbnb_name")]
    [MaxLength(255)]
    public string AirbnbName { get; set; } = string.Empty;

    [Required]
    [Column("host_id")]
    [MaxLength(255)]
    public string HostId { get; set; } = string.Empty;

    [Required]
    [Column("client_id")]
    [MaxLength(255)]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    [Column("client_name")]
    [MaxLength(255)]
    public string ClientName { get; set; } = string.Empty;

    [Column("reservation_date")]
    public DateTime? ReservationDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

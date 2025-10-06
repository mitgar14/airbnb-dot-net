using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Usuarios.API.Data.Entities;

[Table("micro_users")]
public class Usuario
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    [MaxLength(20)]
    public string Role { get; set; } = "user";

    [Required]
    [Column("password")]
    [MaxLength(80)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

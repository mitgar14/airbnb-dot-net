using Airbnb.Common.Constants;

namespace Usuarios.API.DTOs;

public class UsuarioDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateUsuarioDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.Cliente; // Valor por defecto: Cliente
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ValidationUsuarioDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateUsuarioDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}

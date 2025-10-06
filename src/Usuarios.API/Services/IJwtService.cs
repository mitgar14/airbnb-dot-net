namespace Usuarios.API.Services;

public interface IJwtService
{
    string GenerateToken(long userId, string email, string role);
}

using Usuarios.API.DTOs;

namespace Usuarios.API.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
    Task<UsuarioDto?> GetUsuarioByIdAsync(long userId);
    Task<ValidationUsuarioDto?> ValidateUsuarioAsync(string email, string password);
    Task<UsuarioDto> CreateUsuarioAsync(CreateUsuarioDto createUsuarioDto);
    Task<UsuarioDto> UpdateUsuarioAsync(long userId, UpdateUsuarioDto updateUsuarioDto);
    Task DeleteUsuarioAsync(long userId);
}

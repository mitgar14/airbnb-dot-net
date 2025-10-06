using Usuarios.API.Data.Entities;

namespace Usuarios.API.Repositories;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(long userId);
    Task<Usuario?> ValidateAsync(string email, string password);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task<Usuario> UpdateAsync(Usuario usuario);
    Task DeleteAsync(long userId);
}

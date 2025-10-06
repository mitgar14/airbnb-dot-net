using Microsoft.EntityFrameworkCore;
using Usuarios.API.Data;
using Usuarios.API.Data.Entities;

namespace Usuarios.API.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> GetByIdAsync(long userId)
    {
        return await _context.Usuarios.FindAsync(userId);
    }

    public async Task<Usuario?> ValidateAsync(string email, string password)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
        
        if (usuario != null && BCrypt.Net.BCrypt.Verify(password, usuario.Password))
        {
            return usuario;
        }
        
        return null;
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task DeleteAsync(long userId)
    {
        var usuario = await _context.Usuarios.FindAsync(userId);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}

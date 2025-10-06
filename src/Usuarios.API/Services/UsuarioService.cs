using AutoMapper;
using Usuarios.API.Data.Entities;
using Usuarios.API.DTOs;
using Usuarios.API.Repositories;

namespace Usuarios.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IMapper _mapper;

    public UsuarioService(IUsuarioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
    {
        var usuarios = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
    }

    public async Task<UsuarioDto?> GetUsuarioByIdAsync(long userId)
    {
        var usuario = await _repository.GetByIdAsync(userId);
        return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
    }

    public async Task<ValidationUsuarioDto?> ValidateUsuarioAsync(string email, string password)
    {
        var usuario = await _repository.ValidateAsync(email, password);
        return usuario != null ? _mapper.Map<ValidationUsuarioDto>(usuario) : null;
    }

    public async Task<UsuarioDto> CreateUsuarioAsync(CreateUsuarioDto createUsuarioDto)
    {
        var usuario = _mapper.Map<Usuario>(createUsuarioDto);
        
        // Hash the password before storing
        usuario.Password = BCrypt.Net.BCrypt.HashPassword(createUsuarioDto.Password);
        
        var createdUsuario = await _repository.CreateAsync(usuario);
        return _mapper.Map<UsuarioDto>(createdUsuario);
    }

    public async Task<UsuarioDto> UpdateUsuarioAsync(long userId, UpdateUsuarioDto updateUsuarioDto)
    {
        var existingUsuario = await _repository.GetByIdAsync(userId);
        if (existingUsuario == null)
            throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        
        // Actualización parcial - solo actualizar campos no nulos
        if (updateUsuarioDto.Name != null) 
            existingUsuario.Name = updateUsuarioDto.Name;
        
        if (updateUsuarioDto.Email != null) 
            existingUsuario.Email = updateUsuarioDto.Email;
        
        if (updateUsuarioDto.Role != null) 
            existingUsuario.Role = updateUsuarioDto.Role;
        
        if (updateUsuarioDto.Password != null)
        {
            // Hash the new password
            existingUsuario.Password = BCrypt.Net.BCrypt.HashPassword(updateUsuarioDto.Password);
        }
        
        var updatedUsuario = await _repository.UpdateAsync(existingUsuario);
        return _mapper.Map<UsuarioDto>(updatedUsuario);
    }

    public async Task DeleteUsuarioAsync(long userId)
    {
        await _repository.DeleteAsync(userId);
    }
}

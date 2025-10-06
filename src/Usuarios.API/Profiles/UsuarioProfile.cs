using AutoMapper;
using Usuarios.API.Data.Entities;
using Usuarios.API.DTOs;

namespace Usuarios.API.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDto>();
        CreateMap<Usuario, ValidationUsuarioDto>();
        CreateMap<CreateUsuarioDto, Usuario>();
    }
}

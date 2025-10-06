using AutoMapper;
using Reservas.API.Data.Entities;
using Reservas.API.DTOs;

namespace Reservas.API.Profiles;

public class ReservaProfile : Profile
{
    public ReservaProfile()
    {
        CreateMap<Reserva, ReservaDto>();
        CreateMap<CreateReservaDto, Reserva>();
    }
}

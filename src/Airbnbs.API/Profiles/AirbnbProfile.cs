using AutoMapper;
using Airbnbs.API.Data.Entities;
using Airbnbs.API.DTOs;

namespace Airbnbs.API.Profiles;

public class AirbnbProfile : Profile
{
    public AirbnbProfile()
    {
        CreateMap<Data.Entities.Airbnb, AirbnbDto>();
        CreateMap<CreateAirbnbDto, Data.Entities.Airbnb>();
        CreateMap<UpdateAirbnbDto, Data.Entities.Airbnb>();
    }
}

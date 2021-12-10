using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserMetadataDto>();
        CreateMap<User, UserDto>();
    }
}
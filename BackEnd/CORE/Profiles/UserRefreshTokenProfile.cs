using AutoMapper;
using CORE.Entities;
using CORE.Models;

namespace CORE.Profiles;

public class UserRefreshTokenProfile : Profile
{
    public UserRefreshTokenProfile()
    {
        CreateMap<UserRefreshTokenEntity, RefreshTokenModel>();
        CreateMap<RefreshTokenModel, UserRefreshTokenEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DeviceInfo, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}

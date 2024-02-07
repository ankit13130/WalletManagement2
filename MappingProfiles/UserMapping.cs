using AutoMapper;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.MappingProfile;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User,UserRequestModel>().ReverseMap();
    }
}

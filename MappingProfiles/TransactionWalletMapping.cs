using AutoMapper;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.MappingProfile;

public class TransactionWalletMapping : Profile
{
    public TransactionWalletMapping()
    {
        CreateMap<TransactionWallet, TransactionWalletRequestModel>().ReverseMap();
    }
}

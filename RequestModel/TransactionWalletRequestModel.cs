using WalletManagement2.Models;

namespace WalletManagement2.RequestModel;

public record TransactionWalletRequestModel
{
    public string Status { get; set; }
    public Transaction Transaction { get; set; }
    public Wallet Wallet { get; set; }
}

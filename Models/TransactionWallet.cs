namespace WalletManagement2.Models;

public class TransactionWallet
{
    public long TransactionWalletId { get; set; }
    public string Status { get; set; }
    public Transaction Transaction { get; set; }
    public Wallet Wallet { get; set; }
}

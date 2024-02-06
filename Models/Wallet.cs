namespace WalletManagement2.Models;

public class Wallet
{
    public long WalletId { get; set; }
    public Guid WalletName { get; set; }
    public decimal Balance { get; set; } = 0;
}

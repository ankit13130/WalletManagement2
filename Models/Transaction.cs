using System.Collections.ObjectModel;

namespace WalletManagement2.Models;

public class Transaction
{
    public long TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDateTime { get; set; } = DateTime.UtcNow;
    //public Collection<TransactionWallet> TransactionWallets { get; set;}
}

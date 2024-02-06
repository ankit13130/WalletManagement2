using Microsoft.EntityFrameworkCore;
using WalletManagement2.Models;

namespace WalletManagement2;

public class WalletContext : DbContext
{
    public WalletContext(DbContextOptions<WalletContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionWallet> TransactionWallets { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<UserCoupon> UserCoupons { get; set; }
}

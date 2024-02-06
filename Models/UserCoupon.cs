namespace WalletManagement2.Models;

public class UserCoupon
{
    public long Id { get; set; }
    public User User { get; set; }
    public Coupon Coupon { get; set; }
}

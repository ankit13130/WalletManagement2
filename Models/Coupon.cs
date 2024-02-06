using System.Collections.ObjectModel;

namespace WalletManagement2.Models;

public class Coupon : Audit
{
    public long CouponId { get; set; }
    public string CouponCode { get; set; }
    public decimal CashBack { get; set; }
}

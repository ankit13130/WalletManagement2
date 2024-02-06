namespace WalletManagement2.RequestModel;

public record CouponRequestModel
{
    public string CouponCode { get; set; }
    public decimal CashBack { get; set; }
}

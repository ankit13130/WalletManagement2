using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly WalletContext _walletContext;
    public CouponController(WalletContext walletContext)
    {
        _walletContext = walletContext;
    }

    [HttpPost("addCoupon")]
    public async Task<IActionResult> AddCoupon(CouponRequestModel couponRequestModel)
    {
        var data = _walletContext.Coupons.Where(c=>c.IsActive);
        if (data.Any(x => x.CouponCode == couponRequestModel.CouponCode))
            throw new Exception("Coupon Already Exist...");
        
        Coupon coupon = new Coupon();
        coupon.CouponCode = couponRequestModel.CouponCode;
        coupon.CashBack = couponRequestModel.CashBack;
        _walletContext.Coupons.Add(coupon);
        
        var result = await _walletContext.SaveChangesAsync();
        if (!(result > 0))
            throw new Exception("Coupon Not Added!!");
        return Ok("Coupon Added Successfully...");
    }

    [HttpDelete("removeCoupon/{couponId}")]
    public async Task<IActionResult> RemoveCoupon(long couponId)
    {
        var data = _walletContext.Coupons.Where(c => c.CouponId == couponId);
        if (!await data.AnyAsync(c => c.IsActive))
            throw new Exception("Coupon Not Found");

        Coupon coupon = data.FirstOrDefault();
        coupon.IsActive = false;
        _walletContext.Coupons.Update(coupon);
        
        var result = await _walletContext.SaveChangesAsync();
        if (!(result > 0))
            throw new Exception("Coupon Not Removed!!");
        return Ok("Coupon Removed Successfully...");
    }
}
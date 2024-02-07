using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletManagement2.CustomException;

namespace WalletManagement2.Controllers;

[Authorize(Roles = "admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly WalletContext _walletContext;
    public AdminController(WalletContext walletContext)
    {
        _walletContext = walletContext;
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = _walletContext.Users.Include(x=>x.Wallet);
        if (!await result.AnyAsync())
            throw new NotFoundException("No Users Yet!!");
        return Ok(result.ToList());
    }

    [HttpGet("getActiveUsers")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var result = _walletContext.Users.Where(x => x.IsActive).Include(x=>x.Wallet);
        if (!await result.AnyAsync())
            throw new NotFoundException("No Users Yet!!");
        return Ok(result.ToList());
    }

    [HttpGet("getUser/{userId}")]
    public async Task<IActionResult> GetCutomerByName(long userId)
    {
        var result = _walletContext.Users.Where(x => x.UserId == userId && x.IsActive).Include(x => x.Wallet);
        if (!await result.AnyAsync())
            throw new NotFoundException("User Not Found!!");
        return Ok(result.ToList());
    }

    [HttpGet("getAllWallets")]
    public async Task<IActionResult> GetAllWallets()
    {
        var result = _walletContext.Wallets;
        if (!await result.AnyAsync())
            throw new NotFoundException("Wallets Not Found!!");
        return Ok(result.ToList());
    }

    [HttpGet("getWallet/{walletId}")]
    public async Task<IActionResult> GetAccountByAccountNumber(long walletId)
    {
        var result = _walletContext.Wallets.Where(x => x.WalletId == walletId);
        if (!await result.AnyAsync())
            throw new NotFoundException("Wallet Not Created!!");
        return Ok(result.ToList());
    }

    [HttpGet("getAllTransactions")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var result = _walletContext.Transactions.Join(_walletContext.TransactionWallets,
                                                transaction => transaction.TransactionId,
                                                trasactionWallet => trasactionWallet.Transaction.TransactionId,
                                                (transaction,transactionWallet) => new
                                                {
                                                    TransactionId = transaction.TransactionId,
                                                    WalletId = transactionWallet.Wallet.WalletId,
                                                    TransactionAmount = transaction.Amount,
                                                    TransactionDate = transaction.TransactionDateTime,
                                                    Status = transactionWallet.Status,
                                                });
        if (!await result.AnyAsync())
            throw new NotFoundException("No Transactions Found !!");
        return Ok(result.ToList());
    }

    [HttpGet("getTransactionThroughWalletId/{walletId}")]
    public async Task<IActionResult> GetTransactionDetailByWalletId(long walletId)
    {
        var result = _walletContext.Transactions.Join(_walletContext.TransactionWallets,
                                                transaction => transaction.TransactionId,
                                                trasactionWallet => trasactionWallet.Transaction.TransactionId,
                                                (transaction, transactionWallet) => new
                                                {
                                                    TransactionId = transaction.TransactionId,
                                                    WalletId = transactionWallet.Wallet.WalletId,
                                                    TransactionAmount = transaction.Amount,
                                                    TransactionDate = transaction.TransactionDateTime,
                                                    Status = transactionWallet.Status,
                                                })
                                                .Where(x => x.WalletId == walletId);
        if (!await result.AnyAsync())
            throw new NotFoundException("No Transactions Found !!");
        return Ok(result.ToList());
    }

    [HttpGet("getTransactionThroughTransactionId/{transactionId}")]
    public async Task<IActionResult> GetTransactionDetailByTransactionId(long transactionId)
    {
        var result = _walletContext.Transactions.Join(_walletContext.TransactionWallets,
                                                transaction => transaction.TransactionId,
                                                trasactionWallet => trasactionWallet.Transaction.TransactionId,
                                                (transaction, transactionWallet) => new
                                                {
                                                    TransactionId = transaction.TransactionId,
                                                    WalletId = transactionWallet.Wallet.WalletId,
                                                    TransactionAmount = transaction.Amount,
                                                    TransactionDate = transaction.TransactionDateTime,
                                                    Status = transactionWallet.Status,
                                                })
                                                .Where(x => x.TransactionId == transactionId);
        if (!await result.AnyAsync())
            throw new NotFoundException("No Transactions Found !!");
        return Ok(result.ToList());
    }

    [HttpGet("getAllCoupons")]
    public async Task<IActionResult> GetAllCoupons()
    {
        var result = _walletContext.Coupons;
        if (!await result.AnyAsync())
            throw new NotFoundException("Coupons Not Available!!");
        return Ok(result.ToList());
    }

    [HttpGet("getCoupon/{couponCode}")]
    public async Task<IActionResult> GetCouponByCode(string couponCode)
    {
        var result = _walletContext.Coupons.Where(x => x.CouponCode == couponCode && x.IsActive);
        if (!await result.AnyAsync())
            throw new NotFoundException("Coupon Not Available!!");
        return Ok(result.ToList());
    }
}

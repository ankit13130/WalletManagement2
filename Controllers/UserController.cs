using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WalletManagement2.CustomException;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.Controllers;

[Authorize(Roles ="admin,user")]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly WalletContext _walletContext;
    private readonly IMapper _mapper;
    public UserController(WalletContext walletContext, IMapper mapper)
    {
        _walletContext = walletContext;
        _mapper = mapper;
    }

    [HttpPost("addMoney")]
    public async Task<IActionResult> AddMoney(long userId, decimal money, string? couponCode)
    {
        var data = _walletContext.Users.Where(x => x.UserId == userId && x.IsActive);
        var validCoupon = _walletContext.Coupons.Where(c => c.CouponCode == couponCode && c.IsActive);
        decimal CashBackAmount = 0;

        if (!data.Any())
            throw new NotFoundException("User Not Exist...");
        if (money < 1)
            throw new ValidationException("Add Money Greater Than 0");

        if (!couponCode.IsNullOrEmpty())
        {
            if (!validCoupon.Any())
                throw new ValidationException("Invalid Coupon!!");

            var validUser = _walletContext.UserCoupons.Where(x => x.User.UserId == userId && x.Coupon.CouponId == validCoupon.FirstOrDefault().CouponId);

            if (validUser.Any())
                throw new ValidationException("You Already Used This Coupon...");

            CashBackAmount = money * validCoupon.FirstOrDefault().CashBack / 100;
        }

        Wallet wallet;
        Transaction transaction;
        if (data.Any(x => x.Wallet == null))
        {
            wallet = new Wallet();
            wallet.Balance += money;
            wallet.WalletName = Guid.NewGuid();
            _walletContext.Wallets.Add(wallet);

            var userWalletUpdate = data.FirstOrDefault();
            userWalletUpdate.Wallet = wallet;
            _walletContext.Users.Update(userWalletUpdate);

            //Transaction Adding using helper method
            transaction = Transaction(money);
            TransactionWallet(wallet,"Credited",transaction);

            
        }
        else
        {
            wallet = data.Select(s=>s.Wallet).FirstOrDefault();
            wallet.Balance += money;
            _walletContext.Wallets.Update(wallet);

            //Transaction Adding using helper method
            transaction = Transaction(money);
            TransactionWallet(wallet, "Credited", transaction);
        }

        if(!(CashBackAmount == 0))
        {
            await _walletContext.SaveChangesAsync();
            wallet = data.Select(s => s.Wallet).FirstOrDefault();
            wallet.Balance += CashBackAmount;
            _walletContext.Wallets.Update(wallet);

            //Transaction Adding using helper method
            transaction = Transaction(CashBackAmount);
            TransactionWallet(wallet, "Cashback Amount Credited", transaction);

            UserCoupon userCoupon = new UserCoupon();
            userCoupon.User = data.FirstOrDefault();
            userCoupon.Coupon = validCoupon.FirstOrDefault();
            _walletContext.UserCoupons.Add(userCoupon);
        }
        var result = _walletContext.SaveChanges();
        if (result == 0)
            throw new Exception("Money Not Added");
        return Ok($"₹{money} Added Successfully with TransactionId: {transaction.TransactionId}");
    }
    
    [HttpPost("sendMoney")]
    public async Task<IActionResult> SendMoney(long senderWalletId, long receiverWalletId, decimal amount, string? couponCode)
    {
        var data = _walletContext.Users.Where(x=>x.IsActive);
        var senderWallet = data.Select(s => s.Wallet).FirstOrDefault(c => c.WalletId == senderWalletId);
        var receiverWallet = data.Select(s => s.Wallet).FirstOrDefault(c => c.WalletId == receiverWalletId);
        var validCoupon = _walletContext.Coupons.Where(c => c.CouponCode == couponCode && c.IsActive);
        decimal CashBackAmount = 0;

        if (!data.Any(z=>z.Wallet.WalletId == senderWalletId))
            throw new NotFoundException("Invalid Sender WalletId...");

        if (!data.Any(z => z.Wallet.WalletId == receiverWalletId))
            throw new NotFoundException("Invalid Receiver WalletId...");

        if (amount < 1)
            throw new ValidationException("Send Money greater than 0");

        if(data.Any(x=>x.Wallet.Balance<amount))
            throw new ValidationException("Not Sufficient Balance");
        
        if (!couponCode.IsNullOrEmpty())
        {
            if (!validCoupon.Any())
                throw new ValidationException("Invalid Coupon!!");

            var validUser = _walletContext.UserCoupons.Where(x => x.User.UserId == data.Where(x=>x.Wallet.WalletId == senderWalletId).Select(x=>x.UserId).FirstOrDefault() && x.Coupon.CouponId == validCoupon.FirstOrDefault().CouponId);

            if (validUser.Any())
                throw new ValidationException("You Already Used This Coupon...");

            CashBackAmount = (amount * validCoupon.FirstOrDefault().CashBack) / 100;
        }

        Transaction transaction = Transaction(amount);
        //Debit Amount
        TransactionWallet(senderWallet, "Debited", transaction);
        //updating debited balance
        Wallet wallet = senderWallet;
        wallet.Balance -= amount;
        _walletContext.Wallets.Update(wallet);
        //Credit Amount
        TransactionWallet(receiverWallet, "Credited", transaction);
        //updating debited balance
        wallet = receiverWallet;
        wallet.Balance += amount;
        _walletContext.Wallets.Update(wallet);

        if (!(CashBackAmount == 0))
        {
            senderWallet.Balance += CashBackAmount;
            _walletContext.Wallets.Update(senderWallet);

            transaction = Transaction(CashBackAmount);
            TransactionWallet(senderWallet, "Cashback Amount Credited", transaction);

            UserCoupon userCoupon = new UserCoupon();
            userCoupon.User = data.Where(z=>z.Wallet.WalletId == senderWalletId).FirstOrDefault();
            userCoupon.Coupon = validCoupon.FirstOrDefault();
            _walletContext.UserCoupons.Add(userCoupon);
        }

        var result = await _walletContext.SaveChangesAsync();
        if (!(result > 0))
            throw new Exception("Transaction Failed");
        return Ok($"Transaction Completed Successfully with TransactionId: {transaction.TransactionId}");
    }

    [HttpGet("getTransaction/{walletId}")]
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

    [HttpPut("updateUser/{userId}")]
    public async Task<IActionResult> UpdateUser(long userId, UserRequestModel userRequestModel)
    {
        var data = _walletContext.Users.Where(c => c.UserId == userId);
        if (!await data.AnyAsync(c => c.IsActive))
            throw new NotFoundException("User Not Found");

        User user = _mapper.Map<User>(userRequestModel);
        _walletContext.Users.Update(user);

        var result = await _walletContext.SaveChangesAsync();
        if (!(result > 0))
            throw new Exception("User Not Updated!!");
        return Ok("User Updated Successfully...");
    }

    [HttpDelete("removeUser/{userId}")]
    public async Task<IActionResult> RemoveUser(long userId)
    {
        var data = _walletContext.Users.Where(c => c.UserId == userId);
        if (!await data.AnyAsync(c => c.IsActive))
            throw new NotFoundException("User Not Found");

        User user = data.FirstOrDefault();
        user.IsActive = false;
        _walletContext.Users.Update(user);

        var result = await _walletContext.SaveChangesAsync();
        if (!(result > 0))
            throw new Exception("User Not Removed!!");
        return Ok("User Removed Successfully...");
    }

    //helper method
    private Transaction Transaction(decimal amount)
    {
        Transaction transaction = new Transaction();
        transaction.Amount = amount;
        _walletContext.Transactions.Add(transaction);
        return transaction;
    }
    private void TransactionWallet(Wallet wallet, string status, Transaction transaction)
    {
        //TransactionWallet transactionWallet = _mapper.Map<TransactionWallet>(transaction);
        TransactionWallet transactionWallet = new TransactionWallet();
        transactionWallet.Status = status;
        transactionWallet.Wallet = wallet;
        transactionWallet.Transaction = transaction;
        _walletContext.TransactionWallets.Add(transactionWallet);
    }
}

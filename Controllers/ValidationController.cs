using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WalletManagement2.CustomException;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    private readonly WalletContext _walletContext;
    private readonly IMapper _mapper;
    public ValidationController(WalletContext walletContext, IMapper mapper)
    {
        _walletContext = walletContext;
        _mapper = mapper;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Adduser(UserRequestModel userRequestModel)
    {
        var data = _walletContext.Users.Where(c => c.Username == userRequestModel.Username || c.PANNumber == userRequestModel.PANNumber || c.AdharNumber == userRequestModel.AdharNumber || c.MobileNumber == userRequestModel.MobileNumber || c.Email == userRequestModel.Email);
        
        if (data.Any(z=>z.IsActive))
            throw new Exception("User Already Exist.\nRemember to use Unique Username, PAN number, Adhar Number!!");

        User user = _mapper.Map<User>(userRequestModel);
        _walletContext.Users.Add(user);

        var added = await _walletContext.SaveChangesAsync();
        if (!(added > 0))
        {
            throw new Exception("User Not Added!!");
        }

        return Ok($"User Added successfully.\nUse following credentials to access your account\n Username -> {userRequestModel.Username}\n Password -> {userRequestModel.Password}");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Validateuser(LoginRequestModel loginRequestModel)
    {
        var user = _walletContext.Users.Where(c => c.Username.Equals(loginRequestModel.Username) && c.Password == loginRequestModel.Password && c.IsActive);
        if (user.IsNullOrEmpty())
        {
            throw new ValidationException("Invalid Credentials...");
        }

        if (await user.Where(x => x.Role == "Admin").AnyAsync())
        {
            return Ok($"Access Granted !!\nLogin Successfully as Admin : {loginRequestModel.Username}");
        }
        return Ok($"Access Granted !!\nLogin Successfully with Username : {loginRequestModel.Username}");
    }
}

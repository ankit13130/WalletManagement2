using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalletManagement2.CustomException;
using WalletManagement2.EncryptDecrypt;
using WalletManagement2.Models;
using WalletManagement2.RequestModel;

namespace WalletManagement2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    private readonly WalletContext _walletContext;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    public ValidationController(WalletContext walletContext, IMapper mapper, IConfiguration configuration)
    {
        _walletContext = walletContext;
        _mapper = mapper;
        _configuration = configuration;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Adduser(UserRequestModel userRequestModel)
    {
        var data = _walletContext.Users.Where(c => c.Username == userRequestModel.Username || c.PANNumber == userRequestModel.PANNumber || c.AdharNumber == userRequestModel.AdharNumber || c.MobileNumber == userRequestModel.MobileNumber || c.Email == userRequestModel.Email);

        if (data.Any(z=>z.IsActive))
            throw new ValidationException("User Already Exist.Remember to use Unique Username, PAN number, Adhar Number!!");

        User user = _mapper.Map<User>(userRequestModel);
        //User user = new User();
        //user.FullName = userRequestModel.FullName;
        //user.Username = userRequestModel.Username;
        //user.Password = userRequestModel.Password;
        //user.Email = userRequestModel.Email;
        //user.MobileNumber = userRequestModel.MobileNumber;
        //user.PANNumber = userRequestModel.PANNumber;
        //user.AdharNumber = userRequestModel.AdharNumber;
        //user.Gender = userRequestModel.Gender;

        EncryptionDecryption encryptDecrypt = new EncryptionDecryption();
        user.Hash = encryptDecrypt.HashPasword(userRequestModel.Password, out var salt);
        user.Salt = Convert.ToHexString(salt);
        _walletContext.Users.Add(user);

        var added = await _walletContext.SaveChangesAsync();
        if (!(added > 0))
        {
            throw new Exception("User Not Added!!");
        }

        return Ok($"{userRequestModel.Username} Added successfully.");
    }

    [HttpPost("login")]
    public IActionResult Validateuser(LoginRequestModel loginRequestModel)
    {
        IActionResult response = Unauthorized();
        User user = AuthenticateUser(loginRequestModel);
        if (user != null)
        {
            var tokenString = GenerateToken(user);
            response = Ok(tokenString);
        }

        return response;
    }

    //helper method
    private User AuthenticateUser(LoginRequestModel loginRequestModel)
    {
        User user = null;
        EncryptionDecryption encryptionDecryption = new EncryptionDecryption();
        var data = _walletContext.Users.Where(x => x.Username == loginRequestModel.Username);

        if (!data.Any() || !data.FirstOrDefault().IsActive)
            throw new Exception("User Not Found");

        user = data.FirstOrDefault();
        if (!encryptionDecryption.VerifyPassword(loginRequestModel.Password, user.Hash, Convert.FromHexString(user.Salt)))
            throw new Exception("Wrong Password");

        return user;
    }
    private string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Sid,user.UserId.ToString()),
            new Claim(ClaimTypes.Name,user.Username),
            new Claim(ClaimTypes.Role,user.Role),
        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

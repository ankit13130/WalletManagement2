using System.ComponentModel.DataAnnotations;

namespace WalletManagement2.Models;

public class User : Audit
{
    public long UserId { get; set; }
    public string FullName { get; set; }

//    [RegularExpression(@"^(?=.*[!@#$%^&*()\-_+=\[\]{};:'""<>,.?/])((?=.*\d)(?=.*[A-Z]).{8})$
//",ErrorMessage ="Invalid Username")]
    public string Username { get; set; }
    public string Password { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string PANNumber { get; set; }
    public string AdharNumber { get; set; }
    public string Gender { get; set; }
    public string Role { get; set; } = "user";
    public Wallet? Wallet { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace WalletManagement2.RequestModel;

public record UserRequestModel
{
    public string FullName { get; set; }

    [RegularExpression(@"^(?=.*[!@#$%^&*()\-_+=\[\]{};:'""<>,.?/])(?=.*[A-Z])(?=.*[a-z]{5})((?=.*\d).{8})$", ErrorMessage = "Invalid Username.(hint:#Adfred1)")]
    public string Username { get; set; }
    public string Password { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Mobile Number.(hint:8874521478)")]
    public string MobileNumber { get; set; }

    [RegularExpression(@"^(?=.*[A-Z]{5})(?=.*\d{4})(?=.*[A-Z])$", ErrorMessage = "Invalid PAN Number.(hint:WASDE1234W)")]
    public string PANNumber { get; set; }
    
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Invalid Adhar Number.(hint:887452147878)")]
    public string AdharNumber { get; set; }
    public string Gender { get; set; }
}

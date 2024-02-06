namespace WalletManagement2.RequestModel;

public record LoginRequestModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

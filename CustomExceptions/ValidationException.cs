namespace WalletManagement2.CustomException;

public class ValidationException : Exception
{
    public ValidationException() : base() { }
    public ValidationException(string? msg) : base(msg) { }

}

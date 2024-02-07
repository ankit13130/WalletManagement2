namespace WalletManagement2.CustomException;

public class NotFoundException : Exception
{
    public NotFoundException() : base() { }
    public NotFoundException(string? msg) : base(msg) { }

}

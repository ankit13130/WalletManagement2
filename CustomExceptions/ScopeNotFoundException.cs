namespace WalletManagement2.CustomExceptions;

public class ScopeNotFoundException : Exception
{
    public ScopeNotFoundException() : base() { }
    public ScopeNotFoundException(string? msg) : base(msg) { }
}

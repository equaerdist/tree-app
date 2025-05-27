namespace tree_api.Domain.Exceptions;

public class SecureException : Exception
{
    public SecureException(string message) : base(message) { }

    public SecureException(string message, Exception inner) : base(message, inner) { }
}
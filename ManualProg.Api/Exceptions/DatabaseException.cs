namespace ManualProg.Api.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException()
        : base(DefaultMessage)
    {
    }

    public DatabaseException(string message)
        : base(message)
    {
    }

    public DatabaseException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public const string DefaultMessage = "error.dbUpdate";
}

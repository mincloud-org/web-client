namespace Web.Server.Exceptions;

public class InvalidInputException : Exception
{
    public InvalidInputException() : base("The input provided is invalid.") { }

    public InvalidInputException(string message) : base(message) { }

    public InvalidInputException(string message, Exception innerException) : base(message, innerException) { }
}
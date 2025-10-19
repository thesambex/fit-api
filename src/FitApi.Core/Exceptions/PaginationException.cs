namespace FitApi.Core.Exceptions;

public class PaginationException : Exception
{
    public PaginationException()
    {
        
    }

    public PaginationException(string message) : base(message)
    {
        
    }

    public PaginationException(string message, Exception inner) : base(message, inner)
    {
        
    }
}
namespace Smart_Event_Management_System.CustomException;

public class NoTicketFoundException : Exception
{
    public override string Message { get; }

    public NoTicketFoundException(string message) : base(message)
    {
        Message = message;
    }
}
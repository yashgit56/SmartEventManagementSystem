namespace Smart_Event_Management_System.CustomException;

public class NoTicketFoundException : Exception
{
    public string Message { get; set; }

    public NoTicketFoundException(string message) : base(message)
    {
        Message = message;
    }
}
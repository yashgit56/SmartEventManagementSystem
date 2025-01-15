namespace Smart_Event_Management_System.CustomException;

public class EventCompleteException : Exception
{
    public EventCompleteException(string? message) : base(message)
    {
    }

    private string Message { get; }
}
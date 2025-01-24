namespace Smart_Event_Management_System.CustomException;

public class EventCompleteException : Exception
{
    public EventCompleteException(string message) : base(message)
    {
        Message = message;
    }

    public override string Message { get; }
}
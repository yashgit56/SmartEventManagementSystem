namespace Smart_Event_Management_System.CustomException;

public class NoEventFoundException : Exception
{
    public override string Message { get; } = null!;
    
    public NoEventFoundException(string message) : base(message)
    {
        Message = message;
    }
}
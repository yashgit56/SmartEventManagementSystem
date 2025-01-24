namespace Smart_Event_Management_System.CustomException;

public class InvalidIDException : Exception
{
    public InvalidIDException(string message) : base(message)
    {
        Message = message;
    }

    public override string Message { get; }
}
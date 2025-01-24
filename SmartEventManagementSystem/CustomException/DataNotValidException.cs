namespace Smart_Event_Management_System.CustomException;

public class DataNotValidException : Exception
{
    public DataNotValidException(string message) : base(message)
    {
        Message = message;
    }

    public override string Message { get; }
}
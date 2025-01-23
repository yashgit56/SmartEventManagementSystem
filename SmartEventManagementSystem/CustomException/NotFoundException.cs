namespace Smart_Event_Management_System.CustomException;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
        Message = message;
    }

    public string Message { get; set; }
}
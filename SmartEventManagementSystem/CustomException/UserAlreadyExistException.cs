namespace Smart_Event_Management_System.CustomException;

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException(string message) : base(message)
    {
        Message = message;
    }

    public override string Message { get; }
}
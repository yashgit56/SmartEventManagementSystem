namespace Smart_Event_Management_System.Dto.AuthenticationDto;

public class LoginRequest
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
}
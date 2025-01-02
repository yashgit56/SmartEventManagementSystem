namespace Smart_Event_Management_System.Dto.AuthenticationDto;

public class LoginRequest
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }
}
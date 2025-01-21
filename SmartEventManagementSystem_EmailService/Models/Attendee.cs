namespace SmartEventManagementSystem_EmailService.Models;

public class Attendee
{
    public Attendee(string username, string email, string phoneNumber, string hashPassword)
    {
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        HashPassword = hashPassword;
    }

    public string Username { get; set; }

    public string Email { get; set; }

    public string HashPassword { get; set; }

    public string PhoneNumber { get; set; }

    public override string ToString()
    {
        return $"Attendee: {Username} ( Email: {Email}, Phone: {PhoneNumber}";
    }
}
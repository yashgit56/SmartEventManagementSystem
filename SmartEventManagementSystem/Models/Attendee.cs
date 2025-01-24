using System.ComponentModel.DataAnnotations;

namespace Smart_Event_Management_System.Models;

public class Attendee
{
    public Attendee(string username, string email, string phoneNumber, string hashPassword)
    {
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        HashPassword = hashPassword;
    }

    public Attendee()
    {
        
    }

    [Key] public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashPassword { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public override string ToString()
    {
        return $"Attendee: {Username} (ID: {Id}), Email: {Email}, Phone: {PhoneNumber}, Tickets: {Tickets.Count}";
    }
}
using System.ComponentModel.DataAnnotations;

namespace Smart_Event_Management_System.Models;

public class Admin
{
    [Key] public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? HashPassword { get; set; }

    public Admin(string username, string email, string hashPassword)
    {
        Username = username;
        Email = email;
        HashPassword = hashPassword;
    }

    public override string ToString()
    {
        return $"Admin: {Username} (ID: {Id}), Email: {Email}";
    }
}
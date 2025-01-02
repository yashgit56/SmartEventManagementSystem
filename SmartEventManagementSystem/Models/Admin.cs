using System.ComponentModel.DataAnnotations;

namespace Smart_Event_Management_System.Models;

public class Admin
{
    [Key] public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? HashPassword { get; set; }

    public override string ToString()
    {
        return $"Admin: {Username} (ID: {Id}), Email: {Email}";
    }
}
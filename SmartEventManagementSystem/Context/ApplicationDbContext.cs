using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Attendee?> Attendees { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<Admin> Admins { get; set; }
}
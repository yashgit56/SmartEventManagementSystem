using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Attendee> Attendees { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<Admin> Admins { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Attendee)
            .WithMany(a => a.Tickets)
            .HasForeignKey(t => t.AttendeeId)
            .OnDelete(DeleteBehavior.Cascade); // When Ticket is deleted, Attendee is deleted

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Event)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade); // When Ticket is deleted, Event is deleted
    }
}
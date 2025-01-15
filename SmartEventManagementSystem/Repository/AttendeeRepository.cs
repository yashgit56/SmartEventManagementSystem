using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public class AttendeeRepository : IAttendeeRepository
{
    private readonly ApplicationDbContext _context;

    public AttendeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Attendee?>> GetAllAttendeesAsync()
    {
        return await _context.Attendees.ToListAsync();
    }

    public async Task<Attendee> GetAttendeeByIdAsync(int id)
    {
        return (await _context.Attendees.FindAsync(id))!;
    }

    public async Task<Attendee?> CreateAttendeeAsync(Attendee attendee)
    {
        var existingAttendee =
            await _context.Attendees.FirstOrDefaultAsync(a => a!.Username == attendee.Username || a!.Email == attendee.Email);

        if (existingAttendee != null)
        {
            return null;
        }
        
        _context.Attendees.Add(attendee);
        await _context.SaveChangesAsync();
        return attendee;
    }

    public async Task<bool> UpdateAttendeeAsync(int id, Attendee updatedAttendee)
    {
        var attendee = await _context.Attendees.FindAsync(id);
        if (attendee == null) return false;

        attendee.Username = updatedAttendee.Username;
        attendee.Email = updatedAttendee.Email;
        attendee.PhoneNumber = updatedAttendee.PhoneNumber;

        _context.Entry(attendee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAttendeeAsync(int id)
    {
        var attendee = await _context.Attendees.FindAsync(id);
        if (attendee == null) return false;

        _context.Attendees.Remove(attendee);
        await _context.SaveChangesAsync();
        return true;
    }

    public Attendee? GetAttendeeByUsernameAndPassword(string username, string password)
    {
        return _context.Attendees.FirstOrDefault(attendee =>
            attendee.Username == username && attendee.HashPassword == password);
    }

    public async Task<List<AttendeeWithTicketsDto>> GetAttendeesWithTicketPurchaseHistory()
    {
        return await _context.Attendees
            .Select(a => new AttendeeWithTicketsDto
            {
                Username = a.Username,
                Email = a.Email,
                Tickets = a.Tickets.Select(t => new TicketDetailsDto
                {
                    IsCheckedIn = t.IsCheckedIn,
                    Price = t.Price,
                    ticketStatus = t.ticketStatus
                }).ToList()
            })
            .ToListAsync();
    }
}
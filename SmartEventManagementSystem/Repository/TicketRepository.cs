using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByAttendeeId(int id)
    {
        return await _context.Tickets.Where(ticket => ticket.AttendeeId == id).ToListAsync();
    }

    public async Task<Ticket> GetTicketByIdAsync(int id)
    {
        return (await _context.Tickets.FindAsync(id))!;
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        var eventItem = await _context.Events.FindAsync(ticket.EventId);
        if (eventItem == null) throw new ArgumentException("Invalid Event ID");

        if (eventItem.Tickets.Count() >= eventItem.Capacity)
            throw new InvalidOperationException("Event reached its maximum capacity");

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> UpdateTicketAsync(int id, Ticket updatedTicket)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return false;

        ticket.EventId = updatedTicket.EventId;
        ticket.AttendeeId = updatedTicket.AttendeeId;
        ticket.Price = updatedTicket.Price;
        ticket.PurchaseDate = updatedTicket.PurchaseDate;
        ticket.IsCheckedIn = updatedTicket.IsCheckedIn;

        _context.Entry(ticket).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTicketAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<TicketSalesDto>> GetTicketSalesForEventAsync(int eventId)
    {
        return await _context.Tickets
            .Where(t => t.EventId == eventId)
            .Select(t => new TicketSalesDto
            {
                TicketId = t.Id,
                Price = t.Price,
                PurchaseDate = t.PurchaseDate,
                Username = t.Attendee.Username,
                Email = t.Attendee.Email
            })
            .ToListAsync();
    }
}
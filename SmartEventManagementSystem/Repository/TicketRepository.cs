using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.CustomException;
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

    public async Task<Ticket> CreateTicketAsync(TicketDto ticketDto)
    {
        var eventItem = await _context.Events.FindAsync(ticketDto.EventId);
        var attendee = await _context.Attendees.FindAsync(ticketDto.AttendeeId);
        
        if (eventItem == null) throw new ArgumentException("Invalid Event ID");
        if(attendee == null) throw new ArgumentException("Invalid Attendee ID");

        if (eventItem.Tickets.Count() >= eventItem.Capacity)
            throw new InvalidOperationException("Event reached its maximum capacity");

        if (DateTime.Now > eventItem.Date)
        {
            throw new EventCompleteException($"Event is already completed on {eventItem.Date}");
        }

        var ticket = new Ticket()
        {
            EventId = ticketDto.EventId,
            AttendeeId = ticketDto.AttendeeId,
            Price = eventItem.BasePrice,
            PurchaseDate = DateTime.Now,
            IsCheckedIn = false,
        };
        // ticket.Event = eventItem;
        // ticket.Attendee = attendee;

        attendee.Tickets.Add(ticket);
        eventItem.Tickets.Add(ticket);
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
                Username = t.Attendee!.Username,
                Email = t.Attendee.Email
            })
            .ToListAsync();
    }
}
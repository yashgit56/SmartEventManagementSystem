using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _context.Events.OrderBy(e => e.Date).ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return (await _context.Events.FindAsync(id));
    }

    public async Task<List<Event>> GetAllEventsByName(string name)
    {
        return _context.Events
            .Where(e => e.Name.ToLower() == name.ToLower())
            .ToList();
    }

    public async Task<List<Event>> GetAllEventsByLocation(string location)
    {
        return _context.Events
            .Where(e => e.Location.ToLower() == location.ToLower())
            .ToList();
    }


    public async Task<Event> CreateEventAsync(Event eventItem)
    {
        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return eventItem;
    }

    public async Task<bool> UpdateEventAsync(int id, Event updatedEvent)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return false;

        eventItem.Name = updatedEvent.Name;
        eventItem.Date = updatedEvent.Date;
        eventItem.Location = updatedEvent.Location;
        eventItem.Capacity = updatedEvent.Capacity;
        eventItem.BasePrice = updatedEvent.BasePrice;

        _context.Entry(eventItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem != null)
        {
            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<EventDetailsDto>> GetUpcomingEventsWithAttendeesAndTicketsAsync()
    {
        return await _context.Events
            .Where(e => e.Date >= DateTime.UtcNow)
            .Select(e => new EventDetailsDto
            {
                EventName = e.Name,
                date = e.Date,
                Location = e.Location,
                Tickets = e.Tickets.Select(t => new TicketDetailsDto
                {
                    Price = t.Price,
                    IsCheckedIn = t.IsCheckedIn,
                    ticketStatus = t.ticketStatus
                }).ToList(),
                Attendees = e.Tickets.Select(t => new AttendeeDetailsDto
                {
                    Username = t.Attendee.Username,
                    Email = t.Attendee.Email
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<List<EventCapacityDto>> GetEventWithTicketStatusAndCapacity()
    {
        return await _context.Events
            .GroupJoin(
                _context.Tickets,
                e => e.Id,
                t => t.EventId,
                (e, t) => new { Event = e, Ticket = t }
            )
            .Select(e => new EventCapacityDto
            {
                EventName = e.Event.Name,
                Date = e.Event.Date,
                Location = e.Event.Location,
                Capacity = e.Event.Capacity,
                TicketStatusSummary = e.Ticket
                    .GroupBy(t => t.ticketStatus)
                    .Select(g => new TicketStatusSummaryDto
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public Task<decimal> GetTotalRevenueForAnEventAsync(int eventId)
    {
        return _context.Tickets
            .Where(e => e.EventId == eventId)
            .SumAsync(t => t.Price);
    }

    public Task<List<EventPopularityDto>> GetMostPopularEventsAsync(int topN)
    {
        return _context.Events
            .OrderByDescending(e => e.Tickets.Count)
            .Take(topN)
            .Select(e => new EventPopularityDto
            {
                EventName = e.Name,
                AttendeeCount = e.Tickets.Count
            })
            .ToListAsync();
    }

    public async Task<List<EventSalesDto>> GetTicketsSoldPerEventAsync()
    {
        return await _context.Events
            .Select(e => new EventSalesDto
            {
                EventName = e.Name,
                TicketsSold = e.Tickets.Count,
                TotalRevenue = e.Tickets.Sum(t => t.Price)
            })
            .ToListAsync();
    }
}
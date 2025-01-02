using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Service;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync();

    Task<IEnumerable<Event>> GetEventByName(string name);

    Task<IEnumerable<Event>> GetEventByLocation(string location);

    Task<Event> GetEventByIdAsync(int id);

    Task<Event> CreateEventAsync(Event eventItem);

    Task<bool> UpdateEventAsync(int id, Event updatedEvent);

    Task<bool> DeleteEventAsync(int id);

    Task<List<EventDetailsDto>> GetUpcomingEventsWithAttendeesAndTicketsAsync();

    Task<List<EventCapacityDto>> GetEventWithTicketStatusAndCapacity();

    Task<decimal> GetTotalRevenueForAnEventAsync(int eventId);

    Task<List<EventPopularityDto>> GetMostPopularEventsAsync(int topN);

    Task<List<EventSalesDto>> GetTicketsSoldPerEventAsync();
}
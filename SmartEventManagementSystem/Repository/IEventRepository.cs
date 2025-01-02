using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event> GetEventByIdAsync(int id);

    Task<List<Event>> GetAllEventsByName(string name);

    Task<List<Event>> GetAllEventsByLocation(string location);

    Task<Event> CreateEventAsync(Event eventItem);
    Task<bool> UpdateEventAsync(int id, Event updatedEvent);
    Task<bool> DeleteEventAsync(int id);
    Task<List<EventDetailsDto>> GetUpcomingEventsWithAttendeesAndTicketsAsync();
    Task<List<EventCapacityDto>> GetEventWithTicketStatusAndCapacity();
    Task<decimal> GetTotalRevenueForAnEventAsync(int eventId);
    Task<List<EventPopularityDto>> GetMostPopularEventsAsync(int topN);
    Task<List<EventSalesDto>> GetTicketsSoldPerEventAsync();
}
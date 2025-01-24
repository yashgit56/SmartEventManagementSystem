using FluentValidation;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllEventsAsync();

        if (events == null || !events.Any()) // Check if events are null or empty
        {
            throw new NoEventFoundException("No events found");
        }

        return events;
    }

    public async Task<List<Event>> GetEventByName(string name)
    {
        var events = await _eventRepository.GetAllEventsByName(name);
        
        if (events == null || !events.Any())
        {
            throw new NoEventFoundException("no events found by that name");
        }

        return events;
    }

    public async Task<List<Event>> GetEventByLocation(string location)
    {
        var events = await _eventRepository.GetAllEventsByLocation(location);
        
        if (events == null || !events.Any())
        {
            throw new NoEventFoundException("no events found at that location");
        }

        return events;
    }

    public async Task<Event> GetEventByIdAsync(int id)
    {
        var eventInfo = await _eventRepository.GetEventByIdAsync(id);

        if (eventInfo == null)
        {
            throw new NoEventFoundException("no event exist with that id");
        }

        return eventInfo;
    }

    public async Task<Event> CreateEventAsync(Event eventItem)
    {
        return await _eventRepository.CreateEventAsync(eventItem);
    }

    public async Task<bool> UpdateEventAsync(int id, Event updatedEvent)
    {
        var result = await _eventRepository.UpdateEventAsync(id, updatedEvent);

        if (!result)
        {
            throw new NoEventFoundException("failed to update. no event found with that id");
        }

        return true;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var result = await _eventRepository.DeleteEventAsync(id);

        if (!result)
        {
            throw new NoEventFoundException("failed to delete. no event found with that id");
        }

        return true;
    }

    public async Task<List<EventDetailsDto>> GetUpcomingEventsWithAttendeesAndTicketsAsync()
    {
        return await _eventRepository.GetUpcomingEventsWithAttendeesAndTicketsAsync();
    }

    public async Task<List<EventCapacityDto>> GetEventWithTicketStatusAndCapacity()
    {
        return await _eventRepository.GetEventWithTicketStatusAndCapacity();
    }

    public async Task<decimal> GetTotalRevenueForAnEventAsync(int eventId)
    {
        return await _eventRepository.GetTotalRevenueForAnEventAsync(eventId);
    }

    public async Task<List<EventPopularityDto>> GetMostPopularEventsAsync(int topN)
    {
        return await _eventRepository.GetMostPopularEventsAsync(topN);
    }

    public async Task<List<EventSalesDto>> GetTicketsSoldPerEventAsync()
    {
        return await _eventRepository.GetTicketsSoldPerEventAsync();
    }
}
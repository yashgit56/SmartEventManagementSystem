using FluentValidation;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IValidator<Event> _eventValidator;

    public EventService(ApplicationDbContext context, IValidator<Event> eventValidator,
        IEventRepository eventRepository)
    {
        _eventValidator = eventValidator;
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllEventsAsync();

        return events;
    }

    public async Task<IEnumerable<Event>> GetEventByName(string name)
    {
        var events = await _eventRepository.GetAllEventsByName(name);

        return events;
    }

    public async Task<IEnumerable<Event>> GetEventByLocation(string location)
    {
        var events = await _eventRepository.GetAllEventsByLocation(location);

        return events;
    }

    public async Task<Event> GetEventByIdAsync(int id)
    {
        return await _eventRepository.GetEventByIdAsync(id);
    }

    public async Task<Event> CreateEventAsync(Event eventItem)
    {
        var validationResult = await _eventValidator.ValidateAsync(eventItem);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _eventRepository.CreateEventAsync(eventItem);
    }

    public async Task<bool> UpdateEventAsync(int id, Event updatedEvent)
    {
        var validationResult = await _eventValidator.ValidateAsync(updatedEvent);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _eventRepository.UpdateEventAsync(id, updatedEvent);
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        return await _eventRepository.DeleteEventAsync(id);
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
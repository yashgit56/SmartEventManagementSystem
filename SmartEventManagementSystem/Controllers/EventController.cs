using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _service;
    private readonly ILogger<EventController> _logger;
    private readonly IValidator<Event> _eventValidator;

    public EventController(IEventService service, IValidator<Event> validator, ILogger<EventController> logger)
    {
        _service = service;
        _eventValidator = validator;
        _logger = logger;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
    {
        var events = await _service.GetAllEventsAsync();

        if (!events.Any())
        {
            _logger.LogInformation("No Events Found");
        }

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Event Id must be greater than zero.");
        }

        var eventItem = await _service.GetEventByIdAsync(id);

        if (eventItem == null) return NotFound();

        return Ok(eventItem);
    }

    [HttpGet("byName")]
    public async Task<ActionResult<Event>> GetAllEventsByName([FromQuery] string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Event name can not be empty");
        }

        var events = await _service.GetEventByName(name);

        if (events == null || !events.Any()) return NotFound();

        return Ok(events);
    }

    [HttpGet("byLocation")]
    public async Task<ActionResult<Event>> GetAllEventsByLocation([FromQuery] string? location)
    {
        if (string.IsNullOrEmpty(location))
        {
            throw new ArgumentException("Event Location can not be empty");
        }

        var events = await _service.GetEventByLocation(location);

        if (events == null) return NotFound();

        return Ok(events);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
    {
        var validationResult = await _eventValidator.ValidateAsync(eventItem);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new ValidationFailure(error.PropertyName, error.ErrorMessage))
                .ToList();

            var validationErrorResponse = new ValidationErrorResponse()
            {
                Message = "Validation errors occurred.",
                Errors = errors
            };

            return BadRequest(validationErrorResponse);
        }

        var createdEvent = await _service.CreateEventAsync(eventItem);
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Event Id must be greater than zero.");
        }

        var validationResult = await _eventValidator.ValidateAsync(updatedEvent);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new ValidationFailure(error.PropertyName, error.ErrorMessage))
                .ToList();

            var validationErrorResponse = new ValidationErrorResponse()
            {
                Message = "Validation errors occurred.",
                Errors = errors
            };

            return BadRequest(validationErrorResponse);
        }


        var result = await _service.UpdateEventAsync(id, updatedEvent);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Event Id must be greater than zero.");
        }

        var result = await _service.DeleteEventAsync(id);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("upcoming")]
    public async Task<ActionResult<List<EventDetailsDto>>> GetUpcomingEventsWithAttendeesAndTickets()
    {
        var result = await _service.GetUpcomingEventsWithAttendeesAndTicketsAsync();
        if (result.Count == 0)
            return NotFound("No upcoming events found.");
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("capacity-status")]
    public async Task<ActionResult<List<EventCapacityDto>>> GetEventWithTicketStatusAndCapacity()
    {
        var result = await _service.GetEventWithTicketStatusAndCapacity();
        if (result.Count == 0)
            return NotFound("No events found with ticket status and capacity.");
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("revenue/{eventId}")]
    public async Task<ActionResult<decimal>> GetTotalRevenueForAnEvent(int eventId)
    {
        var result = await _service.GetTotalRevenueForAnEventAsync(eventId);
        return Ok(result);
    }
    
    [HttpGet("popular/{topN}")]
    public async Task<ActionResult<List<EventPopularityDto>>> GetMostPopularEvents(int topN)
    {
        var result = await _service.GetMostPopularEventsAsync(topN);
        if (result.Count == 0)
            return NotFound("No popular events found.");
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("sales")]
    public async Task<ActionResult<List<EventSalesDto>>> GetTicketsSoldPerEvent()
    {
        var result = await _service.GetTicketsSoldPerEventAsync();
        if (result.Count == 0)
            return NotFound("No sales data found.");
        return Ok(result);
    }
}
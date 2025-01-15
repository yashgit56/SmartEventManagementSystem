using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _service;

    public EventController(IEventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
    {
        var events = await _service.GetAllEventsAsync();

        if (!events.Any()) Console.WriteLine("No Events Found");

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        var eventItem = await _service.GetEventByIdAsync(id);

        if (eventItem == null) return NotFound();

        return Ok(eventItem);
    }

    [HttpGet("byName/{name}")]
    public async Task<ActionResult<Event>> GetAllEventsByName(string name)
    {
        Console.WriteLine(name);
        var events = _service.GetEventByName(name);

        if (events == null) return NotFound();

        return Ok(events);
    }

    [HttpGet("byLocation/{location}")]
    public async Task<ActionResult<Event>> GetAllEventsByLocation(string location)
    {
        Console.WriteLine("location for event will be :", location); 
        var events = _service.GetEventByLocation(location);

        if (events == null) return NotFound();

        return Ok(events);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
    {
        if (eventItem == null) return BadRequest("Event data is required");

        var createdEvent = await _service.CreateEventAsync(eventItem);
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
    {
        var result = await _service.UpdateEventAsync(id, updatedEvent);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var result = await _service.DeleteEventAsync(id);

        if (!result) return NotFound();

        return NoContent();
    }
}
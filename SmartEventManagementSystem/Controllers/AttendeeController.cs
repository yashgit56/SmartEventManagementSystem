using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendeeController : ControllerBase
{
    private readonly IAttendeeService _service;

    public AttendeeController(IAttendeeService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Attendee>>> GetAllAttendees()
    {
        try
        {
            var attendees = await _service.GetAllAttendeesAsync();

            return Ok(attendees);
        }
        catch (NotFoundException e)
        {
            return NotFound(new ErrorResponse()
            {
                Message = e.Message
            });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttendee(int id)
    {
        try
        {
            var attendee = await _service.GetAttendeeByIdAsync(id);
            return Ok(attendee);
        }
        catch (InvalidIDException ex)
        {
            return NotFound(new ErrorResponse()
            {
                Message = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse()
            {
                Message = ex.Message
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<Attendee>> CreateAttendee([FromBody] Attendee attendee)
    {
        try
        {
            var createdAttendee = await _service.CreateAttendeeAsync(attendee);
            
            Console.WriteLine(createdAttendee?.ToString());
        
            return CreatedAtAction(nameof(GetAttendee), new { id = createdAttendee?.Id }, createdAttendee);
        }
        catch (UserAlreadyExistException e)
        {
            return NotFound(new ErrorResponse()
            {
                Message = e.Message 
            });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAttendee(int id, [FromBody] Attendee updatedAttendee)
    {
        var result = await _service.UpdateAttendeeAsync(id, updatedAttendee);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttendee(int id)
    {
        try
        {
            await _service.DeleteAttendeeAsync(id);
            return NoContent();
        }
        catch (Exception ex) when (ex is InvalidIDException or NotFoundException)
        {
            return NotFound(new ErrorResponse()
            {
                Message = ex.Message
            });
        }
    }
}
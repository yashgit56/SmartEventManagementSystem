using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendeeController(IAttendeeService service) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Attendee>>> GetAllAttendees()
    {
        var attendees = await service.GetAllAttendeesAsync();

        if (!attendees.Any())
            return NotFound(new NotFoundException("No Attendees Found"));

        return Ok(attendees);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttendee(int id)
    {
        try
        {
            var attendee = await service.GetAttendeeByIdAsync(id);
            return Ok(attendee);
        }
        catch (Exception ex) when (ex is InvalidIDException or NotFoundException)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<Attendee>> CreateAttendee([FromBody] Attendee attendee)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(new DataNotValidException("attendee data is not in proper format"));

            var createdAttendee = await service.CreateAttendeeAsync(attendee);
        
            return CreatedAtAction(nameof(GetAttendee), new { id = createdAttendee.Id }, createdAttendee);
        }
        catch (Exception e) when (e is UserAlreadyExistException)
        {
            return NotFound(new { e.Message });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAttendee(int id, [FromBody] Attendee updatedAttendee)
    {
        if (!ModelState.IsValid)
            return BadRequest(new DataNotValidException("updated attendee data is not in proper format"));

        var result = await service.UpdateAttendeeAsync(id, updatedAttendee);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttendee(int id)
    {
        try
        {
            await service.DeleteAttendeeAsync(id);
            return NoContent();
        }
        catch (Exception ex) when (ex is InvalidIDException or NotFoundException)
        {
            return NotFound(new { ex.Message });
        }
    }
}
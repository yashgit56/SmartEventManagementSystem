using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Smart_Event_Management_System.Validators;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendeeController : ControllerBase
{
    private readonly IAttendeeService _service;
    private readonly IValidator<Attendee> _attendeeValidator;

    public AttendeeController(IAttendeeService service, IValidator<Attendee> validator)
    {
        _service = service;
        _attendeeValidator = validator;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Attendee>>> GetAllAttendees()
    {
        var attendees = await _service.GetAllAttendeesAsync();
        return Ok(attendees);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttendee(int id)
    {
        var attendee = await _service.GetAttendeeByIdAsync(id);
        return Ok(attendee);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Attendee>> CreateAttendee([FromBody] Attendee attendee)
    {
        var validationResult = await _attendeeValidator.ValidateAsync(attendee);

        if (!validationResult.IsValid) {
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
        
        var createdAttendee = await _service.CreateAttendeeAsync(attendee);
        return CreatedAtAction(nameof(GetAttendee), new { id = createdAttendee?.Id }, createdAttendee);
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
        await _service.DeleteAttendeeAsync(id);
        return NoContent();
    }
}
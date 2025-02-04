using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
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
        if (id <= 0)
        {
            throw new InvalidIDException("Attendee Id must be greater than zero.");
        }
        
        var attendee = await _service.GetAttendeeByIdAsync(id);
        
        return Ok(new AttendeeDetailsDto()
        {
            Username = attendee.Username,
            Email = attendee.Email,
            PhoneNumber = attendee.PhoneNumber
        });
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

        return Ok(new
        {
            SuccessMessage = "You registered successfully on system",
            createdAttendee
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAttendee(int id, [FromBody] Attendee updatedAttendee)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Attendee Id must be greater than zero.");
        }
        
        var validationResult = await _attendeeValidator.ValidateAsync(updatedAttendee);

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
        
        var authenticatedUsername = User.FindFirst(ClaimTypes.Name)?.Value;
        var attendee = await _service.GetAttendeeByUsername(authenticatedUsername);

        if (id != attendee.Id)
        {
            throw new UnauthorizedAccessException("You are Unauthorized to update attendee");
        }
        
        var result = await _service.UpdateAttendeeAsync(id, updatedAttendee);

        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttendee(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Attendee Id must be greater than zero.");
        }
        
        var authenticatedUsername = User.FindFirst(ClaimTypes.Name)?.Value;
        var attendee = await _service.GetAttendeeByUsername(authenticatedUsername);

        if (id != attendee.Id)
        {
            throw new UnauthorizedAccessException("You are Unauthorized to delete attendee");
        }
        
        await _service.DeleteAttendeeAsync(id);
        return NoContent();
    }
}
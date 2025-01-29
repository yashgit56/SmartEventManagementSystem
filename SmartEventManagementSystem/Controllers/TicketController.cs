using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly ITicketService _service;
    private readonly IValidator<Ticket> _ticketValidator;
    
    public TicketController(ITicketService service, IValidator<Ticket> validator)
    {
        _service = service;
        _ticketValidator = validator;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
    {
        var tickets = await _service.GetAllTicketsAsync();
        return Ok(tickets);
    }

    [HttpPut("attendee/{id}")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTicketsByAttendeeId(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Attendee id must be greater than zero.");
        }
        
        var tickets = await _service.GetAllTicketsByAttendeeId(id);

        if (!tickets.Any()) Console.WriteLine("No tickets found for that attendee");
        return Ok(tickets);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicketById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Ticket id must be greater than zero.");
        }
        
        var ticket = await _service.GetTicketByIdAsync(id);

        if (ticket == null) return NotFound();

        return Ok(ticket);
    }


    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket([FromBody] TicketDto ticketDto)
    {
        if (ticketDto.AttendeeId <= 0)
        {
            throw new InvalidIDException("Attendee Id must be greater than zero.");
        }

        if (ticketDto.EventId <= 0)
        {
            throw new InvalidIDException("Event Id must be greater than zero.");
        }
        
        var createdTicket = await _service.CreateTicketAsync(ticketDto);

        return CreatedAtAction(
            nameof(GetTicketById),
            new { id = createdTicket.Id },
            createdTicket
            );
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTicket(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIDException("Id must be greater than zero.");
        }

        var authenticatedUsername = User.FindFirst(ClaimTypes.Name)?.Value;

        var result = await _service.DeleteTicketAsync(id, authenticatedUsername);
        
        if (!result) return NotFound();
        
        return NoContent();
    }
}
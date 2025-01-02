﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketController(ITicketService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
    {
        var tickets = await _service.GetAllTicketsAsync();
        return Ok(tickets);
    }

    [HttpPut("attendee/{id}")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTicketsByAttendeeId(int id)
    {
        var tickets = await _service.GetAllTicketsByAttendeeId(id);

        if (!tickets.Any()) Console.WriteLine("No tickets found for that attendee");
        return Ok(tickets);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicketById(int id)
    {
        var ticket = await _service.GetTicketByIdAsync(id);

        if (ticket == null) return NotFound();

        return Ok(ticket);
    }


    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket([FromBody] Ticket ticket)
    {
        if (ticket == null) return BadRequest("Ticket data is required");

        var createdTicket = await _service.CreateTicketAsync(ticket);

        return CreatedAtAction(
            nameof(GetTicketById),
            new { id = createdTicket.Id },
            createdTicket
        );
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
    {
        var result = await _service.UpdateTicketAsync(id, ticket);
        if (!result) return NotFound();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTicket(int id)
    {
        var result = await _service.DeleteTicketAsync(id);

        if (!result) return NotFound();

        return NoContent();
    }
}
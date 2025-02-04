using FluentValidation;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using System.Security.Claims;

namespace Smart_Event_Management_System.Service;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAttendeeService _attendeeService;

    public TicketService(ITicketRepository ticketRepository, IAttendeeService attendeeService)
    {
        _ticketRepository = ticketRepository;
        _attendeeService = attendeeService;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        var allTicketsAsync = tickets.ToList();
        
        if (tickets == null || allTicketsAsync.Count == 0)
        {
            throw new NoTicketFoundException("No Tickets found");
        }

        return allTicketsAsync;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByAttendeeId(int id)
    {
        var tickets = await _ticketRepository.GetAllTicketsByAttendeeId(id);

        var allTicketsByAttendeeId = tickets.ToList();
        if (tickets == null || !allTicketsByAttendeeId.Any())
        {
            throw new NoTicketFoundException("No Tickets found for that attendee");
        }

        return allTicketsByAttendeeId;
    }

    public async Task<Ticket> GetTicketByIdAsync(int id)
    {
        var ticket = await _ticketRepository.GetTicketByIdAsync(id);
        
        if (ticket == null)
        {
            throw new NoTicketFoundException("No Ticket found");
        }

        return ticket;
    }

    public async Task<Ticket> CreateTicketAsync(TicketDto ticketDto)
    {
        return await _ticketRepository.CreateTicketAsync(ticketDto);
    }

    public async Task<bool> DeleteTicketAsync(int id)
    {
        return await _ticketRepository.DeleteTicketAsync(id);
    }

    public async Task<List<TicketSalesDto>> GetTicketSalesForEventAsync(int eventId)
    {
        return await _ticketRepository.GetTicketSalesForEventAsync(eventId);
    }
}
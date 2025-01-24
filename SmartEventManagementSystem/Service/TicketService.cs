using FluentValidation;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        if (tickets == null || !tickets.Any())
        {
            throw new NoTicketFoundException("No Tickets found");
        }

        return tickets;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByAttendeeId(int id)
    {
        var tickets = await _ticketRepository.GetAllTicketsByAttendeeId(id);
        
        if (tickets == null || !tickets.Any())
        {
            throw new NoTicketFoundException("No Tickets found for that attendee");
        }

        return tickets;
    }

    public async Task<Ticket> GetTicketByIdAsync(int id)
    {
        var tickets = await _ticketRepository.GetTicketByIdAsync(id);
        
        if (tickets == null)
        {
            throw new NoTicketFoundException("No Ticket found");
        }

        return tickets;
    }

    public async Task<Ticket> CreateTicketAsync(TicketDto ticketDto)
    {
        return await _ticketRepository.CreateTicketAsync(ticketDto);
    }

    public async Task<bool> UpdateTicketAsync(int id, Ticket updatedTicket)
    {
        return await _ticketRepository.UpdateTicketAsync(id, updatedTicket);
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
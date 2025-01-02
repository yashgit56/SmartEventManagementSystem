using FluentValidation;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IValidator<Ticket> _ticketValidator;

    public TicketService(IValidator<Ticket> ticketValidator, ITicketRepository _ticketRepository)
    {
        _ticketValidator = ticketValidator;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _ticketRepository.GetAllTicketsAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByAttendeeId(int id)
    {
        return await _ticketRepository.GetAllTicketsByAttendeeId(id);
    }

    public async Task<Ticket> GetTicketByIdAsync(int id)
    {
        return await _ticketRepository.GetTicketByIdAsync(id);
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        var validationResult = await _ticketValidator.ValidateAsync(ticket);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _ticketRepository.CreateTicketAsync(ticket);
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
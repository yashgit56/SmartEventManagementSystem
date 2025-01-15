using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<IEnumerable<Ticket>> GetAllTicketsByAttendeeId(int id);
    Task<Ticket> GetTicketByIdAsync(int id);
    Task<Ticket> CreateTicketAsync(TicketDto ticketDto);
    Task<bool> UpdateTicketAsync(int id, Ticket updatedTicket);
    Task<bool> DeleteTicketAsync(int id);
    Task<List<TicketSalesDto>> GetTicketSalesForEventAsync(int eventId);
}
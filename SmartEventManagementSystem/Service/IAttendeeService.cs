using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Service;

public interface IAttendeeService
{
    Task<IEnumerable<Attendee?>> GetAllAttendeesAsync();

    Task<Attendee> GetAttendeeByIdAsync(int id);

    Task<Attendee> GetAttendeeByUsername(string username);

    Task<Attendee?> CreateAttendeeAsync(Attendee attendee);

    Task<bool> UpdateAttendeeAsync(int id, Attendee attendee);

    Task<bool> DeleteAttendeeAsync(int id);

    Attendee GetAttendeeByUsernameAndPassword(string username, string password);

    Task<List<AttendeeWithTicketsDto>> GetAttendeesWithTicketPurchaseHistory();
}
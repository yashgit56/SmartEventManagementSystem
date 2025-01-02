using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public interface IAttendeeRepository
{
    Task<IEnumerable<Attendee>> GetAllAttendeesAsync();
    Task<Attendee> GetAttendeeByIdAsync(int id);
    Task<Attendee> CreateAttendeeAsync(Attendee attendee);
    Task<bool> UpdateAttendeeAsync(int id, Attendee updatedAttendee);
    Task<bool> DeleteAttendeeAsync(int id);
    Attendee? GetAttendeeByUsernameAndPassword(string username, string password);
    Task<List<AttendeeWithTicketsDto>> GetAttendeesWithTicketPurchaseHistory();
}
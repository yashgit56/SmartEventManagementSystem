using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public interface ICheckInLogRepository
{
    Task<IEnumerable<CheckInLog>> GetAllCheckInLogsAsync();
    Task<CheckInLog> GetCheckInLogByIdAsync(int id);
    Task<CheckInLog> CreateCheckInLogAsync(CheckInLog checkInLog);
    Task<bool> DeleteCheckInLogAsync(int id);
}
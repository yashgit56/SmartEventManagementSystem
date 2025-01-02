using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public class CheckInLogRepository : ICheckInLogRepository
{
    private readonly ApplicationDbContext _context;

    public CheckInLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CheckInLog>> GetAllCheckInLogsAsync()
    {
        return await _context.CheckInLogs.ToListAsync();
    }

    public async Task<CheckInLog> GetCheckInLogByIdAsync(int id)
    {
        return (await _context.CheckInLogs.FindAsync(id))!;
    }

    public async Task<CheckInLog> CreateCheckInLogAsync(CheckInLog checkInLog)
    {
        checkInLog.CheckInTime = DateTime.UtcNow;
        _context.CheckInLogs.Add(checkInLog);

        var ticket = await _context.Tickets.FindAsync(checkInLog.AttendeeId);
        if (ticket == null || ticket.AttendeeId != checkInLog.AttendeeId)
            throw new InvalidOperationException("Invalid ticket or attendee id");

        if (ticket.IsCheckedIn) throw new InvalidOperationException("Ticket already checked in");

        ticket.IsCheckedIn = true;
        await _context.SaveChangesAsync();
        return checkInLog;
    }

    public async Task<bool> DeleteCheckInLogAsync(int id)
    {
        var checkInLog = await _context.CheckInLogs.FindAsync(id);
        if (checkInLog != null)
        {
            _context.CheckInLogs.Remove(checkInLog);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
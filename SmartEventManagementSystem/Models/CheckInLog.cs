using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_Event_Management_System.Models;

public class CheckInLog
{
    public CheckInLog(int eventId, int attendeeId)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        CheckInTime = DateTime.UtcNow;
    }

    [Key] public int Id { get; set; }

    public int EventId { get; set; }

    [ForeignKey(nameof(EventId))] public Event Event { get; } = null!;

    public int AttendeeId { get; set; }

    [ForeignKey(nameof(AttendeeId))] public Attendee Attendee { get; } = null!;

    public DateTime CheckInTime { get; set; }
}
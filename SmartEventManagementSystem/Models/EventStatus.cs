using System.ComponentModel;

namespace Smart_Event_Management_System.Models;

public enum EventStatus
{
    [Description("Upcoming")] Upcoming = 1,

    [Description("Ongoing")] Ongoing = 2,

    [Description("Completed")] Completed = 3,

    [Description("Cancelled")] Cancelled = 4,

    [Description("Postponed")] Postponed = 5
}
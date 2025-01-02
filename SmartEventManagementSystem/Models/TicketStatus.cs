using System.ComponentModel;

namespace Smart_Event_Management_System.Models;

public enum TicketStatus
{
    [Description("Available")] Available = 1,
    [Description("Pending")] Pending = 2,
    [Description("Confirmed")] Confirmed = 3,
    [Description("CheckedIn")] CheckedIn = 4,
    [Description("Cancelled")] Cancelled = 5
}
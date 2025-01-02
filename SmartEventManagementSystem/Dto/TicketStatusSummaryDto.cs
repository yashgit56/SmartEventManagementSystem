using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Dto;

public class TicketStatusSummaryDto
{
    public TicketStatus Status { get; set; }

    public int Count { get; set; }
}
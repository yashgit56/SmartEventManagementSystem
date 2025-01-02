using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Dto;

public class TicketDetailsDto
{
    public decimal Price { get; set; }

    public bool IsCheckedIn { get; set; }

    public TicketStatus ticketStatus { get; set; }
}
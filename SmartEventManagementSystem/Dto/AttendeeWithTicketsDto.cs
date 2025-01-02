namespace Smart_Event_Management_System.Dto;

public class AttendeeWithTicketsDto
{
    public string Username { get; set; }

    public string Email { get; set; }

    public List<TicketDetailsDto> Tickets { get; set; }
}
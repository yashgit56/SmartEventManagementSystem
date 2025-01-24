namespace Smart_Event_Management_System.Dto;

public class AttendeeWithTicketsDto
{
    public string Username { get; set; } = null!;

    public string Email { get; set; }= null!;

    public List<TicketDetailsDto> Tickets { get; set; } = new List<TicketDetailsDto>();
}
namespace Smart_Event_Management_System.Dto;

public class EventDetailsDto
{
    public string EventName { get; set; } = null!;

    public DateTime date { get; set; } 

    public string Location { get; set; } = null!;

    public List<TicketDetailsDto> Tickets { get; set; } = new List<TicketDetailsDto>();

    public List<AttendeeDetailsDto> Attendees { get; set; } = new List<AttendeeDetailsDto>();
}
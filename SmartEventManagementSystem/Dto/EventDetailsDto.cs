namespace Smart_Event_Management_System.Dto;

public class EventDetailsDto
{
    public string EventName { get; set; }

    public DateTime date { get; set; }

    public string Location { get; set; }

    public List<TicketDetailsDto> Tickets { get; set; }

    public List<AttendeeDetailsDto> Attendees { get; set; }
}
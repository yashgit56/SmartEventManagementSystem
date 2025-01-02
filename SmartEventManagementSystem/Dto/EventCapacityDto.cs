namespace Smart_Event_Management_System.Dto;

public class EventCapacityDto
{
    public string EventName { get; set; }

    public DateTime Date { get; set; }

    public string Location { get; set; }

    public int Capacity { get; set; }

    public List<TicketStatusSummaryDto> TicketStatusSummary { get; set; }
}
namespace Smart_Event_Management_System.Dto;

public class EventCapacityDto
{
    public string EventName { get; set; } = null!;

    public DateTime Date { get; set; }

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public List<TicketStatusSummaryDto> TicketStatusSummary { get; set; } = new List<TicketStatusSummaryDto>();
}
namespace Smart_Event_Management_System.Dto;

public class EventSalesDto
{
    public string EventName { get; set; }= null!;
    
    public int TicketsSold { get; set; }
    
    public decimal TotalRevenue { get; set; }
}
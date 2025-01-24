namespace Smart_Event_Management_System.Dto;

public class TicketSalesDto
{
    public int TicketId { get; set; }

    public decimal Price { get; set; }

    public DateTime PurchaseDate { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}
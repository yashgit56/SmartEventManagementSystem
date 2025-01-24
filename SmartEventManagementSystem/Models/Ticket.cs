using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Smart_Event_Management_System.Models;

public class Ticket
{
    public Ticket(int eventId, int? attendeeId, decimal price, DateTime purchaseDate)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        Price = price;
        PurchaseDate = purchaseDate;
    }

    [Key] public int Id { get; set; }

    public int EventId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(EventId))] public Event? Event { get; }

    public int? AttendeeId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(AttendeeId))] public Attendee? Attendee { get; }

    public decimal Price { get; set; }

    public DateTime PurchaseDate { get; set; }

    public bool IsCheckedIn { get; set; } = false;

    public TicketStatus TicketStatus { get; set; } = TicketStatus.Available;

    public Ticket()
    {
        
    }
    
    public Ticket(int eventId, int? attendeeId, decimal price, DateTime purchaseDate, bool isCheckedIn)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        Price = price;
        PurchaseDate = purchaseDate;
        IsCheckedIn = isCheckedIn;
    }
}
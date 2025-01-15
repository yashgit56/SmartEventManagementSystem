using System.ComponentModel.DataAnnotations;

namespace Smart_Event_Management_System.Models;

public class Event
{
    public Event(string name, DateTime date, string location, int capacity, decimal basePrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name can not be null or contain whitespace", nameof(name));

        if (date < DateTime.Now) throw new ArgumentException("Event date must be in future", nameof(date));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location can not be empty", nameof(location));

        if (capacity <= 0) throw new ArgumentException("Capacity must be greater than zero");

        if (basePrice <= 0) throw new ArgumentException("Base Price must be greater than zero.");

        Name = name;
        Date = date;
        Location = location;
        Capacity = capacity;
        BasePrice = basePrice;
    }

    [Key] public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }

    public string Location { get; set; }

    public int Capacity { get; set; }

    public decimal BasePrice { get; set; }

    public EventStatus eventStatus { get; set; } = EventStatus.Upcoming;

    public ICollection<Ticket> Tickets { get; set; } 
}
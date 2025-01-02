using System.ComponentModel.DataAnnotations;

namespace Smart_Event_Management_System.Dto;

public class CheckInLogDto
{
    [Required] public int EventId { get; set; }

    [Required] public int AttendeeId { get; set; }
}
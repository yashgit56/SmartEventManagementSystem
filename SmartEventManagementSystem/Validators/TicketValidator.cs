using FluentValidation;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Validators;

public class TicketValidator : AbstractValidator<Ticket>
{
    public TicketValidator()
    {
        RuleFor(t => t.EventId)
            .GreaterThan(0).WithMessage("EventId must be greater than zero.");

        RuleFor(t => t.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(t => t.PurchaseDate)
            .NotEmpty().WithMessage("Purchase date is required.");

        RuleFor(t => t.AttendeeId)
            .Must(attendeeId => attendeeId.HasValue).WithMessage("AttendeeId cannot be null.");
    }
}
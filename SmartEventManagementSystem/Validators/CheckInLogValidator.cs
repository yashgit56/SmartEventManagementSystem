using FluentValidation;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Validators;

public class CheckInLogValidator : AbstractValidator<CheckInLog>
{
    public CheckInLogValidator()
    {
        RuleFor(log => log.EventId)
            .GreaterThan(0).WithMessage("EventId must be greater than zero.");

        RuleFor(log => log.AttendeeId)
            .GreaterThan(0).WithMessage("AttendeeId must be greater than zero.");

        RuleFor(log => log.CheckInTime)
            .NotEmpty().WithMessage("Check-in time is required.")
            .Must(BeValidDateTime).WithMessage("Invalid check-in time.");

        RuleFor(log => log.Event)
            .NotNull().WithMessage("Event is required.");

        RuleFor(log => log.Attendee)
            .NotNull().WithMessage("Attendee is required.");
    }

    private bool BeValidDateTime(DateTime checkInTime)
    {
        return checkInTime != default && checkInTime <= DateTime.UtcNow;
    }
}
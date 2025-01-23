using FluentValidation;
using FluentValidation.Results;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Event name is required.")
            .MaximumLength(100).WithMessage("Event name cannot exceed 100 characters.");

        RuleFor(e => e.Date)
            .NotEmpty().WithMessage("Event date is required.")
            .Must(date => date >= DateTime.Now).WithMessage("Event date must be in the future.");

        RuleFor(e => e.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(e => e.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(e => e.BasePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than zero.");
    }
    
    public Task<ValidationResult> ValidateAsync(Event eventInfo)
    {
        return base.ValidateAsync(eventInfo);
    }
}
using FluentValidation;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Validators;

public class AttendeeValidator : AbstractValidator<Attendee>
{
    public AttendeeValidator()
    {
        RuleFor(a => a.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

        RuleFor(a => a.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

        RuleFor(a => a.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
            .Matches(@"^\d{10,15}$").WithMessage("Invalid phone number format.");

        RuleFor(a => a.HashPassword)
            .NotEmpty().WithMessage("Password is required.");
    }
}
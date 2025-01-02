using FluentValidation;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class AttendeeService : IAttendeeService
{
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly IValidator<Attendee> _attendeeValidator;
    private readonly CustomLogicService _customLogicService;

    public AttendeeService(ApplicationDbContext context, CustomLogicService customLogicService,
        IValidator<Attendee> attendeeValidator, IAttendeeRepository attendeeRepository)
    {
        _attendeeRepository = attendeeRepository;
        _customLogicService = customLogicService;
        _attendeeValidator = attendeeValidator;
    }

    public async Task<IEnumerable<Attendee>> GetAllAttendeesAsync()
    {
        var attendees = await _attendeeRepository.GetAllAttendeesAsync();

        return attendees;
    }

    public async Task<Attendee> GetAttendeeByIdAsync(int id)
    {
        if (id <= 0) throw new InvalidIDException("Invalid Id. Must be greater than zero");

        var attendee = await _attendeeRepository.GetAttendeeByIdAsync(id);

        if (attendee == null) throw new NotFoundException($"Attendee with id {id} not found.");

        return attendee;
    }

    public async Task<Attendee> CreateAttendeeAsync(Attendee attendee)
    {
        var validationResult = await _attendeeValidator.ValidateAsync(attendee);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var hashPassword = _customLogicService.HashPassword(attendee.HashPassword);
        var tempAttendee = new Attendee(attendee.Username, attendee.Email, attendee.PhoneNumber, hashPassword);

        attendee = await _attendeeRepository.CreateAttendeeAsync(tempAttendee);

        return attendee;
    }

    public async Task<bool> UpdateAttendeeAsync(int id, Attendee updatedAttendee)
    {
        var validationResult = await _attendeeValidator.ValidateAsync(updatedAttendee);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        await _attendeeRepository.UpdateAttendeeAsync(id, updatedAttendee);

        return true;
    }

    public async Task<bool> DeleteAttendeeAsync(int id)
    {
        if (id <= 0) throw new InvalidIDException("Invalid Id. Must be greater than zero");

        await _attendeeRepository.DeleteAttendeeAsync(id);

        return true;
    }

    public Attendee? ValidateAttendee(string username, string password)
    {
        var hashPassword = _customLogicService.HashPassword(password);

        var attendee = _attendeeRepository.GetAttendeeByUsernameAndPassword(username, hashPassword);

        return attendee;
    }

    public async Task<List<AttendeeWithTicketsDto>> GetAttendeesWithTicketPurchaseHistory()
    {
        var attendees = await _attendeeRepository.GetAttendeesWithTicketPurchaseHistory();

        return attendees;
    }
}
using System.Text;
using System.Text.Json;
using FluentValidation;
using MassTransit;
using MessageContracts;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public AttendeeService(ApplicationDbContext context, CustomLogicService customLogicService,
        IValidator<Attendee> attendeeValidator, IAttendeeRepository attendeeRepository,
        IPublishEndpoint publishEndpoint
            )
    {
        _attendeeRepository = attendeeRepository;
        _customLogicService = customLogicService;
        _attendeeValidator = attendeeValidator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<IEnumerable<Attendee?>> GetAllAttendeesAsync()
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

    public async Task<Attendee?> CreateAttendeeAsync(Attendee attendee)
    {
        var validationResult = await _attendeeValidator.ValidateAsync(attendee);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var password = attendee.HashPassword;
        var hashPassword = _customLogicService.HashPassword(attendee.HashPassword);
        var tempAttendee = new Attendee(attendee.Username, attendee.Email, attendee.PhoneNumber, hashPassword);

        var createdAttendee = await _attendeeRepository.CreateAttendeeAsync(tempAttendee);

        if (createdAttendee == null)
        {
            throw new UserAlreadyExistException("User with that username or email already exists.");
        }
        
        try
        {
            var message = JsonSerializer.Serialize(tempAttendee);
            var body = Encoding.UTF8.GetBytes(message);

            var passAttendeeObj = new AttendeeEmailMessage()
            {
                Message = body
            };
            
            await _publishEndpoint.Publish<AttendeeEmailMessage>(passAttendeeObj, context =>
            {
                context.SetRoutingKey("attendee_email");
            });

            return createdAttendee;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while creating attendee" + e.Message) ;
            return createdAttendee;
        }
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
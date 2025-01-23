using FluentValidation;
using MassTransit;
using MessageContracts;
using Moq;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Smart_Event_Management_System.Service;
using FluentValidation.Results;

namespace SmartEventManagementSystem_Tests.ServiceTests;

public class AttendeeServiceTests
{
    private readonly Mock<IAttendeeRepository> _mockAttendeeRepository;
    private readonly Mock<IValidator<Attendee>> _mockAttendeeValidator;
    private readonly Mock<CustomLogicService> _mockCustomLogicService;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly AttendeeService _attendeeService;

    public AttendeeServiceTests()
    {
        _mockAttendeeRepository = new Mock<IAttendeeRepository>();
        _mockAttendeeValidator = new Mock<IValidator<Attendee>>();
        _mockCustomLogicService = new Mock<CustomLogicService>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();

        _attendeeService = new AttendeeService(
            _mockCustomLogicService.Object, 
            _mockAttendeeValidator.Object, 
            _mockAttendeeRepository.Object, 
            _mockPublishEndpoint.Object
        );
    }
    
    [Fact]
    public async Task GetAllAttendeesAsync_ReturnsAttendees_WhenAttendeesExist()
    {
        var attendees = new List<Attendee>
        {
            new Attendee("username1", "email1@test.com", "123456", "hashedPassword"),
            new Attendee("username2", "email2@test.com", "654321", "hashedPassword")
        };

        _mockAttendeeRepository.Setup(repo => repo.GetAllAttendeesAsync())
            .ReturnsAsync(attendees);
        
        var result = await _attendeeService.GetAllAttendeesAsync();
        
        Assert.NotNull(result);
        var enumerable = result as Attendee?[] ?? result.ToArray();
        Assert.Equal(2, enumerable.Count());
        Assert.Contains(enumerable, a => a!.Username == "username1");
        Assert.Contains(enumerable, a => a!.Username == "username2");
    }
    
    [Fact]
    public async Task GetAttendeeByIdAsync_ThrowsInvalidIDException_WhenIdIsInvalid()
    {
        int invalidId = 0;
        
        var exception = await Assert.ThrowsAsync<InvalidIDException>(() => _attendeeService.GetAttendeeByIdAsync(invalidId));
        Assert.Equal("Invalid Id. Must be greater than zero", exception.Message);
    }
    
    [Fact]
    public async Task GetAttendeeByIdAsync_ReturnsAttendee_WhenAttendeeExists()
    {
        int validId = 1;
        var attendee = new Attendee("username", "email@test.com", "123456", "hashedPassword");

        _mockAttendeeRepository.Setup(repo => repo.GetAttendeeByIdAsync(validId))
            .ReturnsAsync(attendee);
        
        var result = await _attendeeService.GetAttendeeByIdAsync(validId);
        
        Assert.NotNull(result);
        Assert.Equal("username", result.Username);
    }
    
    // [Fact]
    // public async Task CreateAttendeeAsync_ReturnsAttendee_WhenValid()
    // {
    //     var attendee = new Attendee("username", "email@test.com", "123456", "hashedPassword");
    //     var createdAttendee = new Attendee("username", "email@test.com", "123456", "hashedPassword");
    //
    //     // Mock validation result to simulate valid validation
    //     var validationResult = new FluentValidation.Results.ValidationResult();
    //     _mockAttendeeValidator.Setup(v => v.ValidateAsync(It.IsAny<Attendee>()))
    //         .ReturnsAsync(validationResult);
    //     
    //     // Mock other services
    //     _mockCustomLogicService.Setup(c => c.HashPassword(It.IsAny<string>())).Returns("hashedPassword");
    //     _mockAttendeeRepository.Setup(repo => repo.CreateAttendeeAsync(It.IsAny<Attendee>()))
    //         .ReturnsAsync(createdAttendee);
    //
    //     // Act
    //     var result = await _attendeeService.CreateAttendeeAsync(attendee);
    //
    //     // Assert
    //     Assert.NotNull(result);
    // }
    //
    //
    //
    // [Fact]
    // public async Task UpdateAttendeeAsync_ReturnsTrue_WhenAttendeeUpdated()
    // {
    //     const int attendeeId = 1;
    //     var updatedAttendee = new Attendee("updatedUsername", "updatedEmail@test.com", "987654", "hashedPassword");
    //     
    //     _mockAttendeeValidator.Setup(v => v.ValidateAsync(It.IsAny<Attendee>()))
    //         .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    //     _mockAttendeeRepository.Setup(repo => repo.UpdateAttendeeAsync(attendeeId, updatedAttendee))
    //         .ReturnsAsync(true);
    //     
    //     var result = await _attendeeService.UpdateAttendeeAsync(attendeeId, updatedAttendee);
    //     
    //     Assert.True(result);
    // }
    //
    // [Fact]
    // public async Task DeleteAttendeeAsync_ReturnsTrue_WhenAttendeeDeleted()
    // {
    //     int attendeeId = 1;
    //     
    //     _mockAttendeeRepository.Setup(repo => repo.DeleteAttendeeAsync(attendeeId))
    //         .ReturnsAsync(true);
    //     
    //     var result = await _attendeeService.DeleteAttendeeAsync(attendeeId);
    //     
    //     Assert.True(result);
    // }
    
    [Fact]
    public void ValidateAttendee_ReturnsAttendee_WhenValidCredentials()
    {
        string username = "validUsername";
        string password = "validPassword";
        var attendee = new Attendee("validUsername", "validEmail@test.com", "123456", "hashedPassword");

        _mockCustomLogicService.Setup(c => c.HashPassword(It.IsAny<string>())).Returns("hashedPassword");
        _mockAttendeeRepository.Setup(repo => repo.GetAttendeeByUsernameAndPassword(username, "hashedPassword"))
            .Returns(attendee);
        
        var result = _attendeeService.ValidateAttendee(username, password);
        
        Assert.NotNull(result);
        Assert.Equal("validUsername", result.Username);
    }
}
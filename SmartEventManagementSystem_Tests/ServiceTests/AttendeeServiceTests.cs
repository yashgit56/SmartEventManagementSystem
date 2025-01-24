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
    private readonly Mock<CustomLogicService> _mockCustomLogicService;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly AttendeeService _attendeeService;

    public AttendeeServiceTests()
    {
        _mockAttendeeRepository = new Mock<IAttendeeRepository>();
        _mockCustomLogicService = new Mock<CustomLogicService>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();

        _attendeeService = new AttendeeService(
            _mockCustomLogicService.Object,
            new Mock<IValidator<Attendee>>().Object,
            _mockAttendeeRepository.Object,
            _mockPublishEndpoint.Object
        );
    }

    [Fact]
    public async Task CreateAttendeeAsync_ValidAttendee_CreatesSuccessfully()
    {
        var attendee = new Attendee { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "password123", PhoneNumber = "1234567890" };
        var hashPassword = "password123";

        _mockCustomLogicService
            .Setup(c => c.HashPassword(It.IsAny<string>()))
            .Returns(hashPassword);

        _mockAttendeeRepository
            .Setup(r => r.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .ReturnsAsync(attendee);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<AttendeeEmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _attendeeService.CreateAttendeeAsync(attendee);

        Assert.NotNull(result);
        Assert.Equal(hashPassword, result.HashPassword);
        _mockAttendeeRepository.Verify(r => r.CreateAttendeeAsync(It.IsAny<Attendee>()), Times.Once);
    }

    [Fact]
    public async Task CreateAttendeeAsync_DuplicateAttendee_ThrowsUserAlreadyExistException()
    {
        var attendee = new Attendee { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "password123", PhoneNumber = "1234567890" };

        _mockAttendeeRepository
            .Setup(r => r.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .ReturnsAsync((Attendee)null);

        var exception = await Assert.ThrowsAsync<UserAlreadyExistException>(() => _attendeeService.CreateAttendeeAsync(attendee));
        Assert.Equal("User with that username or email already exists.", exception.Message);
        _mockAttendeeRepository.Verify(r => r.CreateAttendeeAsync(It.IsAny<Attendee>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAttendeesAsync_ValidAttendees_ReturnsAttendees()
    {
        var attendees = new List<Attendee>
        {
            new Attendee { Username = "user1", Email = "user1@gmail.com", HashPassword = "hashedpassword", PhoneNumber = "1234567890" },
            new Attendee { Username = "user2", Email = "user2@gmail.com", HashPassword = "hashedpassword", PhoneNumber = "0987654321" }
        };

        _mockAttendeeRepository
            .Setup(r => r.GetAllAttendeesAsync())
            .ReturnsAsync(attendees);

        var result = await _attendeeService.GetAllAttendeesAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAttendeeByIdAsync_ValidId_ReturnsAttendee()
    {
        var attendee = new Attendee { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "hashedpassword", PhoneNumber = "1234567890" };

        _mockAttendeeRepository
            .Setup(r => r.GetAttendeeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(attendee);

        var result = await _attendeeService.GetAttendeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetAttendeeByIdAsync_InvalidId_ThrowsInvalidIDException()
    {
        var exception = await Assert.ThrowsAsync<InvalidIDException>(() => _attendeeService.GetAttendeeByIdAsync(0));
        Assert.Equal("Invalid Id. Must be greater than zero", exception.Message);
    }

    [Fact]
    public async Task DeleteAttendeeAsync_ValidId_DeletesAttendee()
    {
        var attendee = new Attendee { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "hashedpassword", PhoneNumber = "1234567890" };

        _mockAttendeeRepository
            .Setup(r => r.DeleteAttendeeAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        var result = await _attendeeService.DeleteAttendeeAsync(1);

        Assert.True(result);
        _mockAttendeeRepository.Verify(r => r.DeleteAttendeeAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAttendeeAsync_InvalidId_ThrowsInvalidIDException()
    {
        var exception = await Assert.ThrowsAsync<InvalidIDException>(() => _attendeeService.DeleteAttendeeAsync(0));
        Assert.Equal("Invalid Id. Must be greater than zero", exception.Message);
    }

    [Fact]
    public void ValidateAttendee_ValidCredentials_ReturnsAttendee()
    {
        var username = "testuser";
        var password = "hashedpassword";
        var attendee = new Attendee { Username = username, HashPassword = password };

        _mockAttendeeRepository
            .Setup(r => r.GetAttendeeByUsernameAndPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(attendee);

        var result = _attendeeService.ValidateAttendee(username, password);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(password, result.HashPassword);
    }
}



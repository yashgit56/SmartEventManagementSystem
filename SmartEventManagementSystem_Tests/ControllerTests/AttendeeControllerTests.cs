using Microsoft.AspNetCore.Mvc;
using Moq;
using Smart_Event_Management_System.Controllers;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace SmartEventManagementSystem_Tests.ControllerTests;

public class AttendeeControllerTests
{
    private readonly Mock<IAttendeeService> _mockAttendeeService;
    private readonly AttendeeController _attendeeController;

    public AttendeeControllerTests()
    {
        _mockAttendeeService = new Mock<IAttendeeService>();
        _attendeeController = new AttendeeController(_mockAttendeeService.Object);
    }

    [Fact]
    public async Task GetAllAttendees_WhenNoAttendeesFound_ReturnsNotFound()
    {
        _mockAttendeeService.Setup(service => service.GetAllAttendeesAsync())
            .ThrowsAsync(new NotFoundException("No Attendees Found")); // Simulate exception
        
        var result = await _attendeeController.GetAllAttendees();
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Verify NotFound is returned
        var response = Assert.IsType<ErrorResponse>(notFoundResult.Value); // Verify the response is of type ErrorResponse
        Assert.Equal("No Attendees Found", response.Message); // Check the error message
    }


    [Fact]
    public async Task GetAllAttendees_WhenAttendeesFound_ReturnsOk()
    {
        var attendees = new List<Attendee>
        {
            new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" },
            new Attendee { Id = 2, Username = "JaneDoe", Email = "jane@example.com", PhoneNumber = "9876543210", HashPassword = "hashedpwd" }
        };
        _mockAttendeeService.Setup(service => service.GetAllAttendeesAsync())
            .ReturnsAsync(attendees);
        
        var result = await _attendeeController.GetAllAttendees();
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<List<Attendee>>(okResult.Value);
        Assert.Equal(2, returnValue.Count); // Ensure two attendees are returned
        Assert.Equal("JohnDoe", returnValue[0].Username); // Validate the first attendee
    }
    
    [Fact]
    public async Task GetAttendee_WhenAttendeeExists_ReturnsOk()
    {
        // Arrange
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.GetAttendeeByIdAsync(1))
            .ReturnsAsync(attendee);

        // Act
        var result = await _attendeeController.GetAttendee(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<Attendee>(okResult.Value);
        Assert.Equal("JohnDoe", returnValue.Username);
        Assert.Equal("john@example.com", returnValue.Email);
    }

    [Fact]
    public async Task GetAttendee_WhenAttendeeNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockAttendeeService.Setup(service => service.GetAttendeeByIdAsync(999))
            .Throws(new NotFoundException("Attendee not found"));

        // Act
        var result = await _attendeeController.GetAttendee(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Attendee not found", response.Message);
    }
    
    [Fact]
    public async Task CreateAttendee_WhenSuccessful_ReturnsCreatedAtAction()
    {
        // Arrange
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .ReturnsAsync(attendee);

        // Act
        var result = await _attendeeController.CreateAttendee(attendee);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsAssignableFrom<Attendee>(createdResult.Value);
        Assert.Equal("JohnDoe", returnValue.Username);
        Assert.Equal("john@example.com", returnValue.Email);
    }

    [Fact]
    public async Task CreateAttendee_WhenUserAlreadyExists_ReturnsNotFound()
    {
        // Arrange
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .Throws(new UserAlreadyExistException("User already exists"));

        // Act
        var result = await _attendeeController.CreateAttendee(attendee);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(notFoundResult.Value); // Assert as ErrorResponse
        Assert.Equal("User already exists", response.Message); // Check the error message
    }


    
    [Fact]
    public async Task UpdateAttendee_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.UpdateAttendeeAsync(1, attendee))
            .ReturnsAsync(true);

        // Act
        var result = await _attendeeController.UpdateAttendee(1, attendee);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAttendee_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.UpdateAttendeeAsync(1, attendee))
            .ReturnsAsync(false);

        // Act
        var result = await _attendeeController.UpdateAttendee(1, attendee);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    
    [Fact]
    public async Task DeleteAttendee_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _mockAttendeeService.Setup(service => service.DeleteAttendeeAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _attendeeController.DeleteAttendee(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAttendee_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockAttendeeService.Setup(service => service.DeleteAttendeeAsync(1))
            .Throws(new NotFoundException("Attendee not found"));

        // Act
        var result = await _attendeeController.DeleteAttendee(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Attendee not found", response.Message);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Smart_Event_Management_System.Controllers;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Xunit;

namespace SmartEventManagementSystem_Tests.ControllerTests;

public class AttendeeControllerTests
{
    private readonly Mock<IAttendeeService> _mockAttendeeService;
    private readonly Mock<IValidator<Attendee>> _mockAttendeeValidator;
    private readonly AttendeeController _attendeeController;

    public AttendeeControllerTests()
    {
        _mockAttendeeService = new Mock<IAttendeeService>();
        _mockAttendeeValidator = new Mock<IValidator<Attendee>>();
        _attendeeController = new AttendeeController(_mockAttendeeService.Object, _mockAttendeeValidator.Object);
    }

    [Fact]
    public async Task GetAllAttendees_WhenNoAttendeesFound_ReturnsNotFound()
    {
        _mockAttendeeService.Setup(service => service.GetAllAttendeesAsync())
            .ThrowsAsync(new NotFoundException("No Attendees Found"));
        
        var result = await _attendeeController.GetAllAttendees();
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("No Attendees Found", response.Message);
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
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("JohnDoe", returnValue[0].Username);
    }
    
    [Fact]
    public async Task GetAttendee_WhenAttendeeExists_ReturnsOk()
    {
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.GetAttendeeByIdAsync(1))
            .ReturnsAsync(attendee);

        var result = await _attendeeController.GetAttendee(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<Attendee>(okResult.Value);
        Assert.Equal("JohnDoe", returnValue.Username);
        Assert.Equal("john@example.com", returnValue.Email);
    }

    [Fact]
    public async Task GetAttendee_WhenAttendeeNotFound_ReturnsNotFound()
    {
        _mockAttendeeService.Setup(service => service.GetAttendeeByIdAsync(999))
            .Throws(new NotFoundException("Attendee not found"));

        var result = await _attendeeController.GetAttendee(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Attendee not found", response.Message);
    }
    
    [Fact]
    public async Task CreateAttendee_WhenSuccessful_ReturnsCreatedAtAction()
    {
        var attendee = new Attendee { Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .ReturnsAsync(attendee);

        var result = await _attendeeController.CreateAttendee(attendee);

        var actionResult = Assert.IsType<ActionResult<Attendee>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<Attendee>(createdResult.Value);
        Assert.Equal("JohnDoe", returnValue.Username);
        Assert.Equal("john@example.com", returnValue.Email);
    }

    [Fact]
    public async Task CreateAttendee_WhenUserAlreadyExists_ReturnsNotFound()
    {
        var attendee = new Attendee { Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.CreateAttendeeAsync(It.IsAny<Attendee>()))
            .Throws(new UserAlreadyExistException("User already exists"));

        var result = await _attendeeController.CreateAttendee(attendee);

        var actionResult = Assert.IsType<ActionResult<Attendee>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("User already exists", response.Message);
    }
    
    [Fact]
    public async Task UpdateAttendee_WhenSuccessful_ReturnsNoContent()
    {
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.UpdateAttendeeAsync(1, attendee))
            .ReturnsAsync(true);

        var result = await _attendeeController.UpdateAttendee(1, attendee);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAttendee_WhenNotFound_ReturnsNotFound()
    {
        var attendee = new Attendee { Id = 1, Username = "JohnDoe", Email = "john@example.com", PhoneNumber = "1234567890", HashPassword = "hashedpwd" };
        _mockAttendeeService.Setup(service => service.UpdateAttendeeAsync(1, attendee))
            .ReturnsAsync(false);

        var result = await _attendeeController.UpdateAttendee(1, attendee);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAttendee_WhenSuccessful_ReturnsNoContent()
    {
        _mockAttendeeService.Setup(service => service.DeleteAttendeeAsync(1))
            .ReturnsAsync(true);

        var result = await _attendeeController.DeleteAttendee(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAttendee_WhenNotFound_ReturnsNotFound()
    {
        _mockAttendeeService.Setup(service => service.DeleteAttendeeAsync(1))
            .Throws(new NotFoundException("Attendee not found"));

        var result = await _attendeeController.DeleteAttendee(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Attendee not found", response.Message);
    }
}

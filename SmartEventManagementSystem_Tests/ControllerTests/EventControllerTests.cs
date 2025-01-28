using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace SmartEventManagementSystem_Tests.ControllerTests;

using Microsoft.AspNetCore.Mvc;
using Moq;
using Smart_Event_Management_System.Controllers;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Xunit;

public class EventControllerTests
{
    private readonly Mock<IEventService> _mockEventService;
    private readonly Mock<IValidator<Event>> _mockEventValidator;
    private readonly EventController _eventController;

    public EventControllerTests()
    {
        _mockEventService = new Mock<IEventService>();
        _mockEventValidator = new Mock<IValidator<Event>>();
        _eventController = new EventController(_mockEventService.Object, _mockEventValidator.Object);
    }

    [Fact]
    public async Task GetAllEvents_WhenEventsExist_ReturnsOkResultWithEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event("Music Fest", DateTime.Now.AddDays(10), "New York", 100, 50),
            new Event("Tech Conference", DateTime.Now.AddDays(20), "San Francisco", 200, 100)
        };
        _mockEventService.Setup(service => service.GetAllEventsAsync()).ReturnsAsync(events);

        // Act
        var result = await _eventController.GetAllEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllEvents_WhenNoEventsExist_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        _mockEventService.Setup(service => service.GetAllEventsAsync()).ReturnsAsync(new List<Event>());

        // Act
        var result = await _eventController.GetAllEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetEvent_WhenEventExists_ReturnsOkResult()
    {
        // Arrange
        var eventItem = new Event("Music Fest", DateTime.Now.AddDays(10), "New York", 100, 50) { Id = 1 };
        _mockEventService.Setup(service => service.GetEventByIdAsync(1)).ReturnsAsync(eventItem);

        // Act
        var result = await _eventController.GetEvent(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Event>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockEventService.Setup(service => service.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync((Event)null);

        // Act
        var result = await _eventController.GetEvent(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEventsByName_WhenEventsExist_ReturnsOkResult()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event("Music Fest", DateTime.Now.AddDays(10), "New York", 100, 50)
        };
        _mockEventService.Setup(service => service.GetEventByName("Music Fest")).ReturnsAsync(events);

        // Act
        var result = await _eventController.GetAllEventsByName("Music Fest");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAllEventsByName_WhenEventsDoNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockEventService.Setup(service => service.GetEventByName(It.IsAny<string>())).ReturnsAsync((List<Event>)null);

        // Act
        var result = await _eventController.GetAllEventsByName("Nonexistent Event");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateEvent_WhenSuccessful_ReturnsCreatedAtAction()
    {
        // Arrange
        var newEvent = new Event("Music Fest", DateTime.Now.AddDays(10), "New York", 100, 50);
        _mockEventService.Setup(service => service.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(new Event("Music Fest", DateTime.Now.AddDays(10), "New York", 100, 50) { Id = 1 });

        // Act
        var result = await _eventController.CreateEvent(newEvent);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Event>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task UpdateEvent_WhenEventExists_ReturnsNoContent()
    {
        // Arrange
        var updatedEvent = new Event("Updated Event", DateTime.Now.AddDays(15), "Chicago", 150, 75) { Id = 1 };
        _mockEventService.Setup(service => service.UpdateEventAsync(1, updatedEvent)).ReturnsAsync(true);

        // Act
        var result = await _eventController.UpdateEvent(1, updatedEvent);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var updatedEvent = new Event("Updated Event", DateTime.Now.AddDays(15), "Chicago", 150, 75);
        _mockEventService.Setup(service => service.UpdateEventAsync(It.IsAny<int>(), It.IsAny<Event>())).ReturnsAsync(false);

        // Act
        var result = await _eventController.UpdateEvent(1, updatedEvent);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_WhenEventExists_ReturnsNoContent()
    {
        // Arrange
        _mockEventService.Setup(service => service.DeleteEventAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _eventController.DeleteEvent(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockEventService.Setup(service => service.DeleteEventAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var result = await _eventController.DeleteEvent(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

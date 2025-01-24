using Moq;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Smart_Event_Management_System.Service;

namespace SmartEventManagementSystem_Tests.ServiceTests;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _mockEventRepository;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _mockEventRepository = new Mock<IEventRepository>();

        _eventService = new EventService(
            _mockEventRepository.Object
        );
    }

    [Fact]
    public async Task GetAllEventsAsync_ValidRequest_ReturnsEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Location = "Location 1", Date = DateTime.Now, Capacity = 100, BasePrice = 50 },
            new Event { Id = 2, Name = "Event 2", Location = "Location 2", Date = DateTime.Now.AddDays(1), Capacity = 200, BasePrice = 75 }
        };

        _mockEventRepository
            .Setup(repo => repo.GetAllEventsAsync())
            .ReturnsAsync(events);

        // Act
        var result = await _eventService.GetAllEventsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Event 1", result.First().Name);
    }

    [Fact]
    public async Task GetAllEventsAsync_NoEventsFound_ThrowsNoEventFoundException()
    {
        // Arrange
        _mockEventRepository
            .Setup(repo => repo.GetAllEventsAsync())
            .ReturnsAsync(new List<Event>()); // Returns an empty list of events

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NoEventFoundException>(() => _eventService.GetAllEventsAsync());
        Assert.Equal("No events found", exception.Message); // Verify that the exception message matches
    }


    [Fact]
    public async Task GetEventByName_ValidName_ReturnsEvents()
    {
        // Arrange
        var name = "Event 1";
        var events = new List<Event>
        {
            new Event { Id = 1, Name = name, Location = "Location 1", Date = DateTime.Now, Capacity = 100, BasePrice = 50 }
        };

        _mockEventRepository
            .Setup(repo => repo.GetAllEventsByName(name))
            .ReturnsAsync(events);

        // Act
        var result = await _eventService.GetEventByName(name);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(name, result.First().Name);
    }

    [Fact]
    public async Task GetEventByName_NoEventFound_ThrowsNoEventFoundException()
    {
        var name = "Non-existent Event";
        _mockEventRepository
            .Setup(repo => repo.GetAllEventsByName(name))
            .ReturnsAsync(new List<Event>());

        
        await Assert.ThrowsAsync<NoEventFoundException>(() => _eventService.GetEventByName(name));
    }

    [Fact]
    public async Task GetEventByIdAsync_ValidId_ReturnsEvent()
    {
        const int eventId = 1;
        var eventItem = new Event { Id = eventId, Name = "Event 1", Location = "Location 1", Date = DateTime.Now, Capacity = 100, BasePrice = 50 };

        _mockEventRepository
            .Setup(repo => repo.GetEventByIdAsync(eventId))
            .ReturnsAsync(eventItem);
  
        var result = await _eventService.GetEventByIdAsync(eventId);
        
        Assert.NotNull(result);
        Assert.Equal(eventId, result.Id);
        Assert.Equal("Event 1", result.Name);
    }

    [Fact]
    public async Task GetEventByIdAsync_NoEventFound_ThrowsNoEventFoundException()
    {
        const int eventId = 999;
        _mockEventRepository
            .Setup(repo => repo.GetEventByIdAsync(eventId))
            .ReturnsAsync((Event)null);
        
        await Assert.ThrowsAsync<NoEventFoundException>(() => _eventService.GetEventByIdAsync(eventId));
    }

    [Fact]
    public async Task CreateEventAsync_ValidEvent_CreatesEvent()
    {
        // Arrange
        var eventItem = new Event { Name = "New Event", Location = "New Location", Date = DateTime.Now, Capacity = 100, BasePrice = 50 };

        _mockEventRepository
            .Setup(repo => repo.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(eventItem);

        // Act
        var result = await _eventService.CreateEventAsync(eventItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Event", result.Name);
    }

    [Fact]
    public async Task UpdateEventAsync_ValidId_UpdatesEvent()
    {
        // Arrange
        var eventId = 1;
        var updatedEvent = new Event { Id = eventId, Name = "Updated Event", Location = "Updated Location", Date = DateTime.Now, Capacity = 150, BasePrice = 60 };

        _mockEventRepository
            .Setup(repo => repo.UpdateEventAsync(eventId, updatedEvent))
            .ReturnsAsync(true);

        // Act
        var result = await _eventService.UpdateEventAsync(eventId, updatedEvent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateEventAsync_NoEventFound_ThrowsNoEventFoundException()
    {
        // Arrange
        var eventId = 999;
        var updatedEvent = new Event { Id = eventId, Name = "Updated Event", Location = "Updated Location", Date = DateTime.Now, Capacity = 150, BasePrice = 60 };

        _mockEventRepository
            .Setup(repo => repo.UpdateEventAsync(eventId, updatedEvent))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NoEventFoundException>(() => _eventService.UpdateEventAsync(eventId, updatedEvent));
    }

    [Fact]
    public async Task DeleteEventAsync_ValidId_DeletesEvent()
    {
        // Arrange
        var eventId = 1;
        _mockEventRepository
            .Setup(repo => repo.DeleteEventAsync(eventId))
            .ReturnsAsync(true);

        // Act
        var result = await _eventService.DeleteEventAsync(eventId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteEventAsync_NoEventFound_ThrowsNoEventFoundException()
    {
        // Arrange
        var eventId = 999;
        _mockEventRepository
            .Setup(repo => repo.DeleteEventAsync(eventId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NoEventFoundException>(() => _eventService.DeleteEventAsync(eventId));
    }
}

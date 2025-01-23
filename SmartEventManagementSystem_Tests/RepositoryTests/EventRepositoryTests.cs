using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Repository;
using Xunit.Abstractions;
using Smart_Event_Management_System.Models;

namespace SmartEventManagementSystem_Tests.RepositoryTests;

public class EventRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly IEventRepository _eventRepository;
    private readonly ApplicationDbContext _appDbContext;

    public EventRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _appDbContext = new ApplicationDbContext(options);
        _eventRepository = new EventRepository(_appDbContext);
    }
    
    [Fact]
    public async Task GetAllEventsAsync_ReturnsAllEventsSortedByDate()
    {
        _appDbContext.Events.RemoveRange(_appDbContext.Events);
        await _appDbContext.SaveChangesAsync();
        
        var event1 = new Event("Event A", DateTime.UtcNow.AddDays(3), "Location A", 100, 50m);
        var event2 = new Event("Event B", DateTime.UtcNow.AddDays(1), "Location B", 200, 60m);
        var event3 = new Event("Event C", DateTime.UtcNow.AddDays(2), "Location C", 150, 70m);

        _appDbContext.Events.AddRange(event1, event2, event3);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _eventRepository.GetAllEventsAsync();
        
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Collection(result,
            e => Assert.Equal("Event B", e.Name),
            e => Assert.Equal("Event C", e.Name),
            e => Assert.Equal("Event A", e.Name));
    }
    
    [Fact]
    public async Task GetEventByIdAsync_ReturnsEvent_WhenEventExists()
    {
        var eventItem = new Event("Event A", DateTime.UtcNow.AddDays(3), "Location A", 100, 50m);
        _appDbContext.Events.Add(eventItem);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _eventRepository.GetEventByIdAsync(eventItem.Id);
        
        Assert.NotNull(result);
        Assert.Equal(eventItem.Id, result.Id);
        Assert.Equal("Event A", result.Name);
    }

    [Fact]
    public async Task GetEventByIdAsync_ThrowsException_WhenEventDoesNotExist()
    {
        var eventInfo = await _eventRepository.GetEventByIdAsync(999);
        
        Assert.Null(eventInfo);
    }

    [Fact]
    public async Task GetAllEventsByName_ReturnsMatchingEvents_WhenEventsExist()
    {
        var event1 = new Event("Test Event", DateTime.UtcNow.AddDays(1), "Location A", 100, 50m);
        var event2 = new Event("Test Event", DateTime.UtcNow.AddDays(2), "Location B", 200, 60m);
        var event3 = new Event("Other Event", DateTime.UtcNow.AddDays(3), "Location C", 150, 70m);

        _appDbContext.Events.AddRange(event1, event2, event3);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _eventRepository.GetAllEventsByName("Test Event");
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("Test Event", e.Name));
    }

    [Fact]
    public async Task GetAllEventsByLocation_ReturnsMatchingEvents_WhenEventsExist()
    {
        var event1 = new Event("Event A", DateTime.UtcNow.AddDays(1), "Test Location", 100, 50m);
        var event2 = new Event("Event B", DateTime.UtcNow.AddDays(2), "Test Location", 200, 60m);
        var event3 = new Event("Event C", DateTime.UtcNow.AddDays(3), "Other Location", 150, 70m);

        _appDbContext.Events.AddRange(event1, event2, event3);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _eventRepository.GetAllEventsByLocation("Test Location");
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("Test Location", e.Location));
    }

    [Fact]
    public async Task CreateEventAsync_CreatesNewEventSuccessfully()
    {
        var eventItem = new Event("New Event", DateTime.UtcNow.AddDays(1), "New Location", 150, 75m);
        
        var result = await _eventRepository.CreateEventAsync(eventItem);
        
        Assert.NotNull(result);
        Assert.Equal("New Event", result.Name);
        Assert.Equal("New Location", result.Location);
    }

    [Fact]
    public async Task UpdateEventAsync_UpdatesEvent_WhenEventExists()
    {
        var eventItem = new Event("Event A", DateTime.UtcNow.AddDays(1), "Old Location", 100, 50m);
        _appDbContext.Events.Add(eventItem);
        await _appDbContext.SaveChangesAsync();

        var updatedEvent = new Event("Updated Event", DateTime.UtcNow.AddDays(2), "New Location", 200, 75m);
        
        var result = await _eventRepository.UpdateEventAsync(eventItem.Id, updatedEvent);
        
        Assert.True(result);
        var updatedItem = await _eventRepository.GetEventByIdAsync(eventItem.Id);
        Assert.Equal("Updated Event", updatedItem.Name);
        Assert.Equal("New Location", updatedItem.Location);
    }

    [Fact]
    public async Task UpdateEventAsync_ReturnsFalse_WhenEventDoesNotExist()
    {
        var updatedEvent = new Event("Updated Event", DateTime.UtcNow.AddDays(2), "New Location", 200, 75m);
        
        var result = await _eventRepository.UpdateEventAsync(999, updatedEvent);
        
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteEventAsync_DeletesEvent_WhenEventExists()
    {
        var eventItem = new Event("Event A", DateTime.UtcNow.AddDays(1), "Location A", 100, 50m);
        _appDbContext.Events.Add(eventItem);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _eventRepository.DeleteEventAsync(eventItem.Id);
        
        Assert.True(result);
        var deletedEvent = await _appDbContext.Events.FindAsync(eventItem.Id);
        Assert.Null(deletedEvent);
    }

    [Fact]
    public async Task DeleteEventAsync_ReturnsFalse_WhenEventDoesNotExist()
    {
        var result = await _eventRepository.DeleteEventAsync(999);
        
        Assert.False(result);
    }
    
}
using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Xunit.Abstractions;

namespace SmartEventManagementSystem_Tests.RepositoryTests;

public class TicketRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly ITicketRepository _ticketRepository;
    private readonly ApplicationDbContext _appDbContext;

    public TicketRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _appDbContext = new ApplicationDbContext(options);
        _ticketRepository = new TicketRepository(_appDbContext);
    }
    
    [Fact]
    public async Task GetAllTicketsAsync_ReturnsAllTickets_WhenTicketsExist()
    {
        _appDbContext.Tickets.RemoveRange(_appDbContext.Tickets);
        await _appDbContext.SaveChangesAsync();
        
        var ticket1 = new Ticket(1, 1, 50, DateTime.Now.AddDays(-1));
        var ticket2 = new Ticket(2, 2, 100, DateTime.Now.AddDays(-2));
        _appDbContext.Tickets.Add(ticket1);
        _appDbContext.Tickets.Add(ticket2);
        await _appDbContext.SaveChangesAsync();

        // Act
        var result = await _ticketRepository.GetAllTicketsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.EventId == 1);
        Assert.Contains(result, t => t.EventId == 2);
    }
    
    [Fact]
    public async Task GetAllTicketsAsync_ReturnsEmptyList_WhenNoTicketsExist()
    {
        _appDbContext.Tickets.RemoveRange(_appDbContext.Tickets);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _ticketRepository.GetAllTicketsAsync();
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTicketByIdAsync_ReturnsTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 1, 50, DateTime.Now);
        _appDbContext.Tickets.Add(ticket);
        await _appDbContext.SaveChangesAsync();

        // Act
        var result = await _ticketRepository.GetTicketByIdAsync(ticket.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticket.Id, result.Id);
        Assert.Equal(ticket.EventId, result.EventId);
    }

    [Fact]
    public async Task GetTicketByIdAsync_ReturnsNull_WhenTicketDoesNotExist()
    {
        // Act
        var result = await _ticketRepository.GetTicketByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTicketAsync_CreatesTicketSuccessfully()
    {
        // Arrange
        var eventItem = new Event("Test Event", DateTime.Now.AddDays(1), "Location", 100, 50);
        var attendee = new Attendee("testUser", "test@example.com", "1234567890", "hashedPassword");
        _appDbContext.Events.Add(eventItem);
        _appDbContext.Attendees.Add(attendee);
        await _appDbContext.SaveChangesAsync();

        var ticketDto = new TicketDto { EventId = eventItem.Id, AttendeeId = attendee.Id };

        // Act
        var result = await _ticketRepository.CreateTicketAsync(ticketDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventItem.Id, result.EventId);
        Assert.Equal(attendee.Id, result.AttendeeId);
        Assert.Equal(eventItem.BasePrice, result.Price);
    }

    [Fact]
    public async Task CreateTicketAsync_ThrowsException_WhenEventIdIsInvalid()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 999, AttendeeId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ticketRepository.CreateTicketAsync(ticketDto));
    }

    [Fact]
    public async Task CreateTicketAsync_ThrowsException_WhenAttendeeIdIsInvalid()
    {
        // Arrange
        var eventItem = new Event("Test Event", DateTime.Now.AddDays(1), "Location", 100, 50);
        _appDbContext.Events.Add(eventItem);
        await _appDbContext.SaveChangesAsync();

        var ticketDto = new TicketDto { EventId = eventItem.Id, AttendeeId = 999 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ticketRepository.CreateTicketAsync(ticketDto));
    }

    [Fact]
    public async Task CreateTicketAsync_ThrowsException_WhenEventCapacityExceeded()
    {
        // Arrange
        var eventItem = new Event("Test Event", DateTime.Now.AddDays(1), "Location", 1, 50);
        var attendee1 = new Attendee("user1", "user1@example.com", "1234567890", "hashedPassword1");
        var attendee2 = new Attendee("user2", "user2@example.com", "0987654321", "hashedPassword2");
        _appDbContext.Events.Add(eventItem);
        _appDbContext.Attendees.AddRange(attendee1, attendee2);
        await _appDbContext.SaveChangesAsync();

        var ticketDto1 = new TicketDto { EventId = eventItem.Id, AttendeeId = attendee1.Id };
        var ticketDto2 = new TicketDto { EventId = eventItem.Id, AttendeeId = attendee2.Id };

        // Act
        await _ticketRepository.CreateTicketAsync(ticketDto1);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _ticketRepository.CreateTicketAsync(ticketDto2));
    }

    
    
    
    

}
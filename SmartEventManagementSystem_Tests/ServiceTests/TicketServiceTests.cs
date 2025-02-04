using FluentAssertions;
using Moq;
using Smart_Event_Management_System.Service;
using Smart_Event_Management_System.Repository;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Dto;

using Smart_Event_Management_System.CustomException;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _mockTicketRepository;
    private readonly Mock<IAttendeeService> _attendeeService;
    private readonly TicketService _ticketService;

    public TicketServiceTests()
    {
        _mockTicketRepository = new Mock<ITicketRepository>();
        _attendeeService = new Mock<IAttendeeService>();
        _ticketService = new TicketService(_mockTicketRepository.Object, _attendeeService.Object); 
    }

    [Fact]
    public async Task GetAllTicketsAsync_ShouldReturnAllTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, Price = 10 },
            new Ticket { Id = 2, Price = 20 }
        };

        _mockTicketRepository.Setup(repo => repo.GetAllTicketsAsync())
            .ReturnsAsync(tickets);

        // Act
        var result = await _ticketService.GetAllTicketsAsync();

        // Assert
        result.Should().BeEquivalentTo(tickets);
    }

    [Fact]
    public async Task GetAllTicketsByAttendeeId_ShouldReturnTicketsByAttendeeId()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, AttendeeId = 1, Price = 10 },
            new Ticket { Id = 2, AttendeeId = 1, Price = 20 }
        };

        _mockTicketRepository.Setup(repo => repo.GetAllTicketsByAttendeeId(1))
            .ReturnsAsync(tickets);

        // Act
        var result = await _ticketService.GetAllTicketsByAttendeeId(1);

        // Assert
        result.Should().BeEquivalentTo(tickets);
    }

    [Fact]
    public async Task GetTicketByIdAsync_ShouldReturnTicketById()
    {
        // Arrange
        var ticket = new Ticket { Id = 1, Price = 10 };

        _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(1))
            .ReturnsAsync(ticket);

        // Act
        var result = await _ticketService.GetTicketByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(ticket);
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldCreateTicket()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        var ticket = new Ticket { Id = 1, Price = 10, AttendeeId = 1, EventId = 1 };

        _mockTicketRepository.Setup(repo => repo.CreateTicketAsync(ticketDto))
            .ReturnsAsync(ticket);

        // Act
        var result = await _ticketService.CreateTicketAsync(ticketDto);

        // Assert
        result.Should().BeEquivalentTo(ticket);
    }

    // [Fact]
    // public async Task DeleteTicketAsync_ShouldReturnTrueIfDeleted()
    // {
    //     // Arrange
    //     _mockTicketRepository.Setup(repo => repo.DeleteTicketAsync(1))
    //         .ReturnsAsync(true);
    //
    //     // Act
    //     var result = await _ticketService.DeleteTicketAsync(1,"ayushpatel");
    //
    //     // Assert
    //     result.Should().BeTrue();
    // }

    [Fact]
    public async Task GetTicketSalesForEventAsync_ShouldReturnTicketSales()
    {
        // Arrange
        var ticketSales = new List<TicketSalesDto>
        {
            new TicketSalesDto { TicketId = 1, Price = 10, Username = "John", Email = "john@example.com" }
        };

        _mockTicketRepository.Setup(repo => repo.GetTicketSalesForEventAsync(1))
            .ReturnsAsync(ticketSales);

        // Act
        var result = await _ticketService.GetTicketSalesForEventAsync(1);

        // Assert
        result.Should().BeEquivalentTo(ticketSales);
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldThrowExceptionIfEventNotFound()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 999, AttendeeId = 1 };

        _mockTicketRepository.Setup(repo => repo.CreateTicketAsync(ticketDto))
            .ThrowsAsync(new ArgumentException("Invalid Event ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ticketService.CreateTicketAsync(ticketDto));
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldThrowExceptionIfAttendeeNotFound()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 999 };

        _mockTicketRepository.Setup(repo => repo.CreateTicketAsync(ticketDto))
            .ThrowsAsync(new ArgumentException("Invalid Attendee ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ticketService.CreateTicketAsync(ticketDto));
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldThrowExceptionIfEventIsFull()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        var eventItem = new Event { Id = 1, Capacity = 0, BasePrice = 10 };

        _mockTicketRepository.Setup(repo => repo.CreateTicketAsync(ticketDto))
            .ThrowsAsync(new InvalidOperationException("Event reached its maximum capacity"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _ticketService.CreateTicketAsync(ticketDto));
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldThrowExceptionIfEventIsCompleted()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        var eventItem = new Event { Id = 1, Date = DateTime.UtcNow.AddDays(-1), BasePrice = 10 };

        _mockTicketRepository.Setup(repo => repo.CreateTicketAsync(ticketDto))
            .ThrowsAsync(new EventCompleteException("Event is already completed"));

        // Act & Assert
        await Assert.ThrowsAsync<EventCompleteException>(() => _ticketService.CreateTicketAsync(ticketDto));
    }
}

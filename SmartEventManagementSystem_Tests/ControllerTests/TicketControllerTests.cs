namespace SmartEventManagementSystem_Tests.ControllerTests;

using Microsoft.AspNetCore.Mvc;
using Moq;
using Smart_Event_Management_System.Controllers;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Xunit;

public class TicketControllerTests
{
    private readonly Mock<ITicketService> _mockTicketService;
    private readonly TicketController _ticketController;

    public TicketControllerTests()
    {
        _mockTicketService = new Mock<ITicketService>();
        _ticketController = new TicketController(_mockTicketService.Object);
    }

    [Fact]
    public async Task GetAllTickets_WhenTicketsExist_ReturnsOkResultWithTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(1, 1, 50, DateTime.Now),
            new Ticket(2, 2, 75, DateTime.Now)
        };
        _mockTicketService.Setup(service => service.GetAllTicketsAsync()).ReturnsAsync(tickets);

        // Act
        var result = await _ticketController.GetAllTickets();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllTicketsByAttendeeId_WhenTicketsExist_ReturnsOkResultWithTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(1, 1, 50, DateTime.Now),
            new Ticket(2, 1, 75, DateTime.Now)
        };
        _mockTicketService.Setup(service => service.GetAllTicketsByAttendeeId(1)).ReturnsAsync(tickets);

        // Act
        var result = await _ticketController.GetAllTicketsByAttendeeId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllTicketsByAttendeeId_WhenNoTicketsExist_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        _mockTicketService.Setup(service => service.GetAllTicketsByAttendeeId(1)).ReturnsAsync(new List<Ticket>());

        // Act
        var result = await _ticketController.GetAllTicketsByAttendeeId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetTicketById_WhenTicketExists_ReturnsOkResult()
    {
        // Arrange
        var ticket = new Ticket(1, 1, 50, DateTime.Now) { Id = 1 };
        _mockTicketService.Setup(service => service.GetTicketByIdAsync(1)).ReturnsAsync(ticket);

        // Act
        var result = await _ticketController.GetTicketById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Ticket>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetTicketById_WhenTicketDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockTicketService.Setup(service => service.GetTicketByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket)null);

        // Act
        var result = await _ticketController.GetTicketById(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTicket_WhenSuccessful_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        var createdTicket = new Ticket(1, 1, 50, DateTime.Now) { Id = 1 };
        _mockTicketService.Setup(service => service.CreateTicketAsync(ticketDto)).ReturnsAsync(createdTicket);

        // Act
        var result = await _ticketController.CreateTicket(ticketDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Ticket>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateTicket_WhenExceptionThrown_ThrowsException()
    {
        // Arrange
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        _mockTicketService.Setup(service => service.CreateTicketAsync(ticketDto))
            .Throws(new ArgumentException("Invalid ticket data"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ticketController.CreateTicket(ticketDto));
    }

    [Fact]
    public async Task UpdateTicket_WhenTicketExists_ReturnsNoContent()
    {
        // Arrange
        var ticket = new Ticket(1, 1, 50, DateTime.Now);
        _mockTicketService.Setup(service => service.UpdateTicketAsync(1, ticket)).ReturnsAsync(true);

        // Act
        var result = await _ticketController.UpdateTicket(1, ticket);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTicket_WhenTicketDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var ticket = new Ticket(1, 1, 50, DateTime.Now);
        _mockTicketService.Setup(service => service.UpdateTicketAsync(It.IsAny<int>(), It.IsAny<Ticket>())).ReturnsAsync(false);

        // Act
        var result = await _ticketController.UpdateTicket(1, ticket);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTicket_WhenTicketExists_ReturnsNoContent()
    {
        // Arrange
        _mockTicketService.Setup(service => service.DeleteTicketAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _ticketController.DeleteTicket(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTicket_WhenTicketDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockTicketService.Setup(service => service.DeleteTicketAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var result = await _ticketController.DeleteTicket(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

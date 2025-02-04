using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

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
    private readonly Mock<IValidator<Ticket>> _mockTicketValidator;
    private readonly Mock<IAttendeeService> _mockAttendeeService; 
    private readonly TicketController _ticketController;

    public TicketControllerTests()
    {
        _mockTicketService = new Mock<ITicketService>();
        _mockTicketValidator = new Mock<IValidator<Ticket>>();
        _mockAttendeeService = new Mock<IAttendeeService>();
        _ticketController = new TicketController(_mockTicketService.Object, _mockTicketValidator.Object, _mockAttendeeService.Object);
    }

    [Fact]
    public async Task GetAllTickets_WhenTicketsExist_ReturnsOkResultWithTickets()
    {
        var tickets = new List<Ticket>
        {
            new Ticket(1, 1, 50, DateTime.Now),
            new Ticket(2, 2, 75, DateTime.Now)
        };
        _mockTicketService.Setup(service => service.GetAllTicketsAsync()).ReturnsAsync(tickets);

        var result = await _ticketController.GetAllTickets();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllTicketsByAttendeeId_WhenTicketsExist_ReturnsOkResultWithTickets()
    {
        var tickets = new List<Ticket>
        {
            new Ticket(1, 1, 50, DateTime.Now),
            new Ticket(2, 1, 75, DateTime.Now)
        };
        _mockTicketService.Setup(service => service.GetAllTicketsByAttendeeId(1)).ReturnsAsync(tickets);

        var result = await _ticketController.GetAllTicketsByAttendeeId(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllTicketsByAttendeeId_WhenNoTicketsExist_ReturnsOkResultWithEmptyList()
    {
        _mockTicketService.Setup(service => service.GetAllTicketsByAttendeeId(1)).ReturnsAsync(new List<Ticket>());

        var result = await _ticketController.GetAllTicketsByAttendeeId(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Ticket>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetTicketById_WhenTicketExists_ReturnsOkResult()
    {
        var ticket = new Ticket(1, 1, 50, DateTime.Now) { Id = 1 };
        _mockTicketService.Setup(service => service.GetTicketByIdAsync(1)).ReturnsAsync(ticket);

        var result = await _ticketController.GetTicketById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Ticket>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetTicketById_WhenTicketDoesNotExist_ReturnsNotFound()
    {
        _mockTicketService.Setup(service => service.GetTicketByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket)null);

        var result = await _ticketController.GetTicketById(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTicket_WhenSuccessful_ReturnsCreatedAtActionResult()
    {
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        var createdTicket = new Ticket(1, 1, 50, DateTime.Now) { Id = 1 };
        _mockTicketService.Setup(service => service.CreateTicketAsync(ticketDto)).ReturnsAsync(createdTicket);

        var result = await _ticketController.CreateTicket(ticketDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<Ticket>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateTicket_WhenExceptionThrown_ThrowsException()
    {
        var ticketDto = new TicketDto { EventId = 1, AttendeeId = 1 };
        _mockTicketService.Setup(service => service.CreateTicketAsync(ticketDto))
            .Throws(new ArgumentException("Invalid ticket data"));

        await Assert.ThrowsAsync<ArgumentException>(() => _ticketController.CreateTicket(ticketDto));
    }

    // [Fact]
    // public async Task DeleteTicket_WhenTicketExists_ReturnsNoContent()
    // {
    //     _mockTicketService.Setup(service => service.DeleteTicketAsync(1, "ayushpatel")).ReturnsAsync(true);
    //
    //     var result = await _ticketController.DeleteTicket(1);
    //
    //     Assert.IsType<NoContentResult>(result);
    // }
    //
    // [Fact]
    // public async Task DeleteTicket_WhenTicketDoesNotExist_ReturnsNotFound()
    // {
    //     _mockTicketService.Setup(service => service.DeleteTicketAsync(It.IsAny<int>(), "ayushpatel")).ReturnsAsync(false);
    //
    //     var result = await _ticketController.DeleteTicket(1);
    //
    //     Assert.IsType<NotFoundResult>(result);
    // }
}

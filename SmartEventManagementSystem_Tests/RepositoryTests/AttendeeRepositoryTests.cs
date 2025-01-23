using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Xunit.Abstractions;

namespace SmartEventManagementSystem_Tests.RepositoryTests;

public class AttendeeRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly ApplicationDbContext _appDbContext;

    public AttendeeRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _appDbContext = new ApplicationDbContext(options);
        _attendeeRepository = new AttendeeRepository(_appDbContext);
    }

    [Fact]
    public void GetAttendeeById_ReturnsCorrectAttendee()
    {
        var attendeeId = 1;

        var result = _attendeeRepository.GetAttendeeByIdAsync(attendeeId);
        
        Assert.NotNull(result);
        Assert.Equal(attendeeId, result.Id);
    }
    
    [Fact]
    public async Task GetAttendeeByIdAsync_ReturnsNull_WhenAttendeeNotFound()
    {
        const int attendeeId = 100;

        var attendee = await _attendeeRepository.GetAttendeeByIdAsync(attendeeId);
        
        Assert.Null(attendee);
    }
    
    [Fact]
    public async Task CreateAttendeeAsync_CreatesNewAttendee_WhenNoDuplicateExists()
    {
        var attendee = new Attendee("vishal", "vishal@example.com", "1234567890", "hashedPassword123");

        var result = await _attendeeRepository.CreateAttendeeAsync(attendee);
        
        Assert.NotNull(result);
        Assert.Equal("vishal", result.Username);
        Assert.Equal("vishal@example.com", result.Email);
    }
    
    [Fact]
    public async Task CreateAttendeeAsync_ReturnsNull_WhenDuplicateExists()
    {
        var attendee = new Attendee("yash", "yash@example.com", "1234567890", "hashedPassword123");

        var result = await _attendeeRepository.CreateAttendeeAsync(attendee);
        
        var duplicateAttendee = new Attendee("yash", "yash@example.com", "1234567890", "hashedPassword123");
        var result2 = await _attendeeRepository.CreateAttendeeAsync(duplicateAttendee);
        
        Assert.Null(result2);
    }

    [Fact]
    public async Task UpdateAttendeeAsync_UpdatesAttendee_WhenAttendeeExists()
    {
        var attendee = new Attendee("johnDoe", "john@example.com", "1234567890", "hashedPassword123");
        var updatedAttendee = new Attendee("johnDoeUpdated", "john_updated@example.com", "0987654321", "newHashedPassword123");

        _appDbContext.Add(attendee);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _attendeeRepository.UpdateAttendeeAsync(attendee.Id, updatedAttendee);
        
        Assert.True(result); 

        var updated = await _attendeeRepository.GetAttendeeByIdAsync(attendee.Id);
        Assert.NotNull(updated);
        Assert.Equal("johnDoeUpdated", updated.Username);
        Assert.Equal("john_updated@example.com", updated.Email);
        Assert.Equal("0987654321", updated.PhoneNumber);
    }

    [Fact]
    public async Task UpdateAttendeeAsync_ReturnsFalse_WhenAttendeeNotFound()
    {
        var updatedAttendee = new Attendee("johnDoeUpdated", "john_updated@example.com", "0987654321", "newHashedPassword123");

        var result = await _attendeeRepository.UpdateAttendeeAsync(updatedAttendee.Id, updatedAttendee);
        
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAttendeeAsync_DeletesAttendee_WhenAttendeeExists()
    {
        var attendee = new Attendee("vishal", "vishal@example.com", "1234567890", "hashedPassword123");
        _appDbContext.Attendees.Add(attendee);
        await _appDbContext.SaveChangesAsync();
        
        var addedAttendee = await _appDbContext.Attendees.FindAsync(attendee.Id);
        Assert.NotNull(addedAttendee);
        
        var result = await _attendeeRepository.DeleteAttendeeAsync(addedAttendee.Id);
        
        Assert.True(result);
    }
    
    [Fact]
    public async Task DeleteAttendeeAsync_ReturnsFalse_WhenAttendeeNotFound()
    {
        const int attendeeId = 1000;
        
        var result = await _attendeeRepository.DeleteAttendeeAsync(attendeeId);
        Assert.False(result);
    }

    [Fact]
    public async Task GetAdminByUsernameAndPassword_WhenUserExist()
    {
        var attendee = new Attendee("yashvataliya65", "yashvataliya65@gmail.com", "1234567890", "yashvataliya65");
        _appDbContext.Add(attendee);
        await _appDbContext.SaveChangesAsync();
        
        var username = "yashvataliya65";
        var password = "yashvataliya65";

        var user = _attendeeRepository.GetAttendeeByUsernameAndPassword(username, password);
        
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
        Assert.Equal(password, user.HashPassword);
    }
    
    [Fact]
    public async Task GetAdminByUsernameAndPassword_WhenUserNotExist()
    {
        var username = "yashvataliya65";
        var password = "yashvataliya65";

        var user = _attendeeRepository.GetAttendeeByUsernameAndPassword(username, password);
        
        Assert.Null(user);
    }
    
    [Fact]
    public async Task GetAllAttendeesAsync_ReturnsAllAttendees_WhenAttendeesExist()
    {
        _appDbContext.Attendees.RemoveRange(_appDbContext.Attendees);
        await _appDbContext.SaveChangesAsync();
        
        var attendee1 = new Attendee("user1", "user1@example.com", "1234567890", "hashedPassword1");
        var attendee2 = new Attendee("user2", "user2@example.com", "0987654321", "hashedPassword2");

        _appDbContext.Attendees.AddRange(attendee1,attendee2);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _attendeeRepository.GetAllAttendeesAsync();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a!.Username == "user1");
        Assert.Contains(result, a => a!.Username == "user2");
    }

    [Fact]
    public async Task GetAllAttendeesAsync_ReturnsEmptyCollection_WhenNoAttendeesExist()
    {
        var attendees = _appDbContext.Attendees.ToList();
        _appDbContext.Attendees.RemoveRange(attendees);
        await _appDbContext.SaveChangesAsync();
        
        var result = await _attendeeRepository.GetAllAttendeesAsync();
        
        Assert.NotNull(result);
        Assert.Empty(result); 
    }

}
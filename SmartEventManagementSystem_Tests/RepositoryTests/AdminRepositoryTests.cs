using Microsoft.EntityFrameworkCore;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Xunit.Abstractions;

namespace SmartEventManagementSystem_Tests.RepositoryTests;

public class AdminRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly IAdminRepository _adminRepository;
    private readonly ApplicationDbContext _appDbContext;

    public AdminRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

         _appDbContext = new ApplicationDbContext(options);
         _adminRepository = new AdminRepository(_appDbContext);
    }
    
    [Fact]
    public void GetAdminByUsernameAndPassword_ReturnsAdmin_WhenCredentialsAreCorrect()
    {
        var admin = new Admin("adminUser", "admin@example.com", "hashedPassword123");
        _appDbContext.Admins.Add(admin);
        _appDbContext.SaveChanges();
        
        var result = _adminRepository.GetAdminByUsernameAndPassword("adminUser", "hashedPassword123");
        
        Assert.NotNull(result);
        Assert.Equal("adminUser", result.Username);
        Assert.Equal("admin@example.com", result.Email);
        Assert.Equal("hashedPassword123", result.HashPassword);
    }
    
    [Fact]
    public void GetAdminByUsernameAndPassword_ReturnsNull_WhenUsernameIsIncorrect()
    {
        var admin = new Admin("adminUser", "admin@example.com", "hashedPassword123");
        _appDbContext.Admins.Add(admin);
        _appDbContext.SaveChanges();
        
        var result = _adminRepository.GetAdminByUsernameAndPassword("wrongUser", "hashedPassword123");
        
        Assert.Null(result);
    }
    
    [Fact]
    public void GetAdminByUsernameAndPassword_ReturnsNull_WhenPasswordIsIncorrect()
    {
        var admin = new Admin("adminUser", "admin@example.com", "hashedPassword123");
        _appDbContext.Admins.Add(admin);
        _appDbContext.SaveChanges();
        
        var result = _adminRepository.GetAdminByUsernameAndPassword("adminUser", "wrongPassword");
        
        Assert.Null(result);
    }

    [Fact]
    public void GetAdminByUsernameAndPassword_ReturnsNull_WhenAdminDoesNotExist()
    {
        var result = _adminRepository.GetAdminByUsernameAndPassword("nonExistentUser", "somePassword");
        
        Assert.Null(result);
    }

    [Fact]
    public void CreateAdmin_CreatesAdminSuccessfully()
    {
        var admin = new Admin("newAdmin", "admin@example.com", "hashedPassword123");
        
        var result = _adminRepository.CreateAdmin(admin);
        
        Assert.NotNull(result);
        Assert.Equal("newAdmin", result.Username);
        Assert.Equal("admin@example.com", result.Email);
        Assert.Equal("hashedPassword123", result.HashPassword);

        var savedAdmin = _appDbContext.Admins.FirstOrDefault(a => a.Username == "newAdmin");
        Assert.NotNull(savedAdmin);
        Assert.Equal("admin@example.com", savedAdmin.Email);
    }
}
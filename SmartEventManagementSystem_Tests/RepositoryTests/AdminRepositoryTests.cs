// using Microsoft.EntityFrameworkCore;
// using Smart_Event_Management_System.Context;
// using Smart_Event_Management_System.Models;
// using Smart_Event_Management_System.Repository;
// using Xunit.Abstractions;
//
// namespace SmartEventManagementSystem_Tests.RepositoryTests;
//
// public class AdminRepositoryTests
// {
//     private readonly ITestOutputHelper _output;
//     private readonly IAdminRepository _AdminRepository;
//     private readonly ApplicationDbContext _appDbContext;
//
//     public AdminRepositoryTests(ITestOutputHelper output)
//     {
//         _output = output;
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase("TestDatabase")
//             .Options;
//
//          _appDbContext = new ApplicationDbContext(options);
//          _AdminRepository = new AdminRepository(_appDbContext);
//     }
//     
//     [Fact]
//     public void GetAdminByUsernameAndPassword_ReturnsAdmin_WhenCredentialsAreCorrect()
//     {
//         var Admin = new Admin("AdminUser", "Admin@example.com", "hashedPassword123");
//         _appDbContext.Admins.Add(Admin);
//         _appDbContext.SaveChanges();
//         
//         var result = _AdminRepository.GetAdminByUsernameAndPassword("AdminUser", "hashedPassword123");
//         
//         Assert.NotNull(result);
//         Assert.Equal("AdminUser", result.Username);
//         Assert.Equal("Admin@example.com", result.Email);
//         Assert.Equal("hashedPassword123", result.Password);
//     }
//     
//     [Fact]
//     public void GetAdminByUsernameAndPassword_ReturnsNull_WhenUsernameIsIncorrect()
//     {
//         var Admin = new Admin("AdminUser", "Admin@example.com", "hashedPassword123");
//         _appDbContext.Admins.Add(Admin);
//         _appDbContext.SaveChanges();
//         
//         var result = _AdminRepository.GetAdminByUsernameAndPassword("wrongUser", "hashedPassword123");
//         
//         Assert.Null(result);
//     }
//     
//     [Fact]
//     public void GetAdminByUsernameAndPassword_ReturnsNull_WhenPasswordIsIncorrect()
//     {
//         var Admin = new Admin("AdminUser", "Admin@example.com", "hashedPassword123");
//         _appDbContext.Admins.Add(Admin);
//         _appDbContext.SaveChanges();
//         
//         var result = _AdminRepository.GetAdminByUsernameAndPassword("AdminUser", "wrongPassword");
//         
//         Assert.Null(result);
//     }
//
//     [Fact]
//     public void GetAdminByUsernameAndPassword_ReturnsNull_WhenAdminDoesNotExist()
//     {
//         var result = _AdminRepository.GetAdminByUsernameAndPassword("nonExistentUser", "somePassword");
//         
//         Assert.Null(result);
//     }
//
//     [Fact]
//     public async Task CreateAdmin_CreatesAdminSuccessfully()
//     {
//         _appDbContext.Admins.RemoveRange(_appDbContext.Admins);
//         await _appDbContext.SaveChangesAsync();
//         
//         var Admin = new Admin("newAdmin", "Admin@example.com", "hashedPassword123");
//         
//         var result = _AdminRepository.CreateAdmin(Admin);
//         
//         Assert.NotNull(result);
//         Assert.Equal("newAdmin", result.Username);
//         Assert.Equal("Admin@example.com", result.Email);
//         Assert.Equal("hashedPassword123", result.Password);
//         
//         var savedAdmin = _appDbContext.Admins.FirstOrDefault(a => a.Username == "newAdmin");
//         Assert.NotNull(savedAdmin);
//         Assert.Equal("Admin@example.com", savedAdmin.Email);
//     }
//
// }
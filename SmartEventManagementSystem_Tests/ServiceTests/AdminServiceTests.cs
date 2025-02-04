// using FluentValidation;
// using Moq;
// using Smart_Event_Management_System.CustomException;
// using Smart_Event_Management_System.CustomLogic;
// using Smart_Event_Management_System.Models;
// using Smart_Event_Management_System.Repository;
// using Smart_Event_Management_System.Service;
// using Xunit.Abstractions;
//
// namespace SmartEventManagementSystem_Tests.ServiceTests;
//
//     public class AdminServiceTests
//     {
//         private readonly ITestOutputHelper _testOutputHelper;
//         private readonly Mock<IAdminRepository> _mockAdminRepository;
//         private readonly Mock<CustomLogicService> _mockCustomLogicService;
//         private readonly AdminService _AdminService;
//
//         public AdminServiceTests(ITestOutputHelper testOutputHelper)
//         {
//             _testOutputHelper = testOutputHelper;
//             _mockAdminRepository = new Mock<IAdminRepository>();
//             _mockCustomLogicService = new Mock<CustomLogicService>();
//
//             _AdminService = new AdminService(
//                 _mockCustomLogicService.Object,
//                 _mockAdminRepository.Object
//             );
//         }
//         
//         [Fact]
//         public void CreateAdmin_ValidAdmin_CreatesSuccessfully()
//         {
//             var Admin = new Admin { Username = "testuser", Email = "testuser@gmail.com", Password = "password123" };
//             var hashedPassword = "password123";
//             
//             _mockCustomLogicService
//                 .Setup(c => c.HashPassword(Admin.Password))
//                 .Returns(hashedPassword);
//             
//             _mockAdminRepository
//                 .Setup(r => r.CreateAdmin(It.IsAny<Admin>()))
//                 .Returns(Admin);
//             
//             var result = _AdminService.CreateAdmin(Admin);
//             
//             Assert.NotNull(result);
//             _testOutputHelper.WriteLine(hashedPassword + " " + result.Password) ;
//             Assert.Equal(hashedPassword, result.Password);  
//             _mockAdminRepository.Verify(r => r.CreateAdmin(It.IsAny<Admin>()), Times.Once); 
//         }
//         
//         [Fact]
//         public void CreateAdmin_InvalidAdmin_ThrowsAdminAlreadyExistException()
//         {
//             var Admin = new Admin { Username = "testuser", Email = "testuser@gmail.com", Password = "password123" };
//             
//             _mockAdminRepository
//                 .Setup(r => r.CreateAdmin(It.IsAny<Admin>()))
//                 .Returns((Admin)null); 
//             
//             var exception = Assert.Throws<UserAlreadyExistException>(() => _AdminService.CreateAdmin(Admin));
//             
//             _mockAdminRepository.Verify(r => r.CreateAdmin(It.IsAny<Admin>()), Times.Once);
//             
//             Assert.Equal("Admin already exist with that email or username", exception.Message);
//         }
//
//         
//         [Fact]
//         public void GetAdminByUsernameAndPassword_ValidCredentials_ReturnsAdmin()
//         {
//             var username = "testuser";
//             var password = "hashedpassword";
//             var Admin = new Admin { Username = username, Password = password };
//             
//             _mockCustomLogicService
//                 .Setup(c => c.HashPassword(password))
//                 .Returns(password); 
//             
//             _mockAdminRepository
//                 .Setup(r => r.GetAdminByUsernameAndPassword(username, password))
//                 .Returns(Admin);
//             
//             var result = _AdminService.GetAdminByUsernameAndPassword(username, password);
//             
//             Assert.NotNull(result);
//             Assert.Equal(username, result.Username);
//             Assert.Equal(password, result.Password);
//             _mockAdminRepository.Verify(r => r.GetAdminByUsernameAndPassword(username, password), Times.Once); 
//         }
//
//         [Fact]
//         public void GetAdminByUsernameAndPassword_InvalidCredentials_ThrowsNotFoundException()
//         {
//             var username = "nonexistentuser";
//             var password = "wrongpassword";
//             
//             _mockAdminRepository
//                 .Setup(r => r.GetAdminByUsernameAndPassword(username, password))
//                 .Returns((Admin)null);  // No matching Admin found
//             
//             var exception = Assert.Throws<NotFoundException>(() => _AdminService.GetAdminByUsernameAndPassword(username, password));
//             Assert.Equal("Admin does not exist with that username", exception.Message);
//         }
//
//
//     }
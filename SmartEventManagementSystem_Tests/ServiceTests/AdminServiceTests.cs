using FluentValidation;
using Moq;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Smart_Event_Management_System.Service;
using Xunit.Abstractions;

namespace SmartEventManagementSystem_Tests.ServiceTests;

    public class AdminServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<IAdminRepository> _mockAdminRepository;
        private readonly Mock<CustomLogicService> _mockCustomLogicService;
        private readonly AdminService _adminService;

        public AdminServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mockAdminRepository = new Mock<IAdminRepository>();
            _mockCustomLogicService = new Mock<CustomLogicService>();

            _adminService = new AdminService(
                _mockCustomLogicService.Object,
                _mockAdminRepository.Object
            );
        }
        
        [Fact]
        public void CreateAdmin_ValidAdmin_CreatesSuccessfully()
        {
            var admin = new Admin { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "password123" };
            var hashedPassword = "password123";
            
            _mockCustomLogicService
                .Setup(c => c.HashPassword(admin.HashPassword))
                .Returns(hashedPassword);
            
            _mockAdminRepository
                .Setup(r => r.CreateAdmin(It.IsAny<Admin>()))
                .Returns(admin);
            
            var result = _adminService.CreateAdmin(admin);
            
            Assert.NotNull(result);
            _testOutputHelper.WriteLine(hashedPassword + " " + result.HashPassword) ;
            Assert.Equal(hashedPassword, result.HashPassword);  
            _mockAdminRepository.Verify(r => r.CreateAdmin(It.IsAny<Admin>()), Times.Once); 
        }
        
        [Fact]
        public void CreateAdmin_InvalidAdmin_ThrowsAdminAlreadyExistException()
        {
            var admin = new Admin { Username = "testuser", Email = "testuser@gmail.com", HashPassword = "password123" };
            
            _mockAdminRepository
                .Setup(r => r.CreateAdmin(It.IsAny<Admin>()))
                .Returns((Admin)null); 
            
            var exception = Assert.Throws<UserAlreadyExistException>(() => _adminService.CreateAdmin(admin));
            
            _mockAdminRepository.Verify(r => r.CreateAdmin(It.IsAny<Admin>()), Times.Once);
            
            Assert.Equal("Admin already exist with that email or username", exception.Message);
        }

        
        [Fact]
        public void GetAdminByUsernameAndPassword_ValidCredentials_ReturnsAdmin()
        {
            var username = "testuser";
            var password = "hashedpassword";
            var admin = new Admin { Username = username, HashPassword = password };
            
            _mockCustomLogicService
                .Setup(c => c.HashPassword(password))
                .Returns(password); 
            
            _mockAdminRepository
                .Setup(r => r.GetAdminByUsernameAndPassword(username, password))
                .Returns(admin);
            
            var result = _adminService.GetAdminByUsernameAndPassword(username, password);
            
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(password, result.HashPassword);
            _mockAdminRepository.Verify(r => r.GetAdminByUsernameAndPassword(username, password), Times.Once); 
        }

        [Fact]
        public void GetAdminByUsernameAndPassword_InvalidCredentials_ThrowsNotFoundException()
        {
            var username = "nonexistentuser";
            var password = "wrongpassword";
            
            _mockAdminRepository
                .Setup(r => r.GetAdminByUsernameAndPassword(username, password))
                .Returns((Admin)null);  // No matching admin found
            
            var exception = Assert.Throws<NotFoundException>(() => _adminService.GetAdminByUsernameAndPassword(username, password));
            Assert.Equal("Admin does not exist with that username", exception.Message);
        }


    }
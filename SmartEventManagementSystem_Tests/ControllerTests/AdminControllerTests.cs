using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Smart_Event_Management_System.Controllers;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace SmartEventManagementSystem_Tests.ControllerTests;

public class AdminControllerTests
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<IValidator<Admin>> _mockAdminValidator;
    private readonly AdminController _adminController;
    
    public AdminControllerTests()
    {
        _mockAdminService = new Mock<IAdminService>();
        _mockAdminValidator = new Mock<IValidator<Admin>>();

        _adminController = new AdminController(_mockAdminService.Object, _mockAdminValidator.Object);
    }
    
    [Fact]
    public async Task CreateAdmin_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var admin = new Admin { Username = "admin1", Email = "admin1@example.com", HashPassword = "password123" };
        var validationResult = new ValidationResult();
        _mockAdminValidator.Setup(v => v.ValidateAsync(admin, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
        _mockAdminService.Setup(s => s.CreateAdmin(admin)).Returns(admin);

        // Act
        var result = await _adminController.CreateAdmin(admin);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Admin>(okResult.Value);
        Assert.Equal(admin.Username, returnValue.Username);
    }
    
    [Fact]
    public async Task CreateAdmin_InvalidInput_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var admin = new Admin { Username = "", Email = "invalidemail", HashPassword = "" };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Username", "Username is required."),
            new ValidationFailure("Email", "Invalid email format."),
            new ValidationFailure("HashPassword", "Password is required.")
        });

        // Mock the validator to return the validation result
        _mockAdminValidator.Setup(v => v.ValidateAsync(admin, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act
        var result = await _adminController.CreateAdmin(admin);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        // Assert the response as ValidationErrorResponse
        var response = Assert.IsType<ValidationErrorResponse>(badRequestResult.Value);

        // Check the message
        Assert.Equal("Validation errors occurred.", response.Message);

        // Ensure 3 validation errors are returned
        Assert.Equal(3, response.Errors.Count);

        // Validate specific error fields and messages
        var usernameError = response.Errors.FirstOrDefault(e => e.PropertyName == "Username");
        Assert.NotNull(usernameError);
        Assert.Equal("Username is required.", usernameError.ErrorMessage);

        var emailError = response.Errors.FirstOrDefault(e => e.PropertyName == "Email");
        Assert.NotNull(emailError);
        Assert.Equal("Invalid email format.", emailError.ErrorMessage);

        var passwordError = response.Errors.FirstOrDefault(e => e.PropertyName == "HashPassword");
        Assert.NotNull(passwordError);
        Assert.Equal("Password is required.", passwordError.ErrorMessage);
    }




    
    [Fact]
    public void GetAdminByUsernameAndPassword_AdminFound_ReturnsOkResult()
    {
        // Arrange
        var username = "admin1";
        var password = "password123";
        var admin = new Admin { Username = username, Email = "admin1@example.com", HashPassword = password };

        _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Returns(admin);

        // Act
        var result = _adminController.GetAdminByUsernameAndPassword(username, password);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Admin>(okResult.Value);
        Assert.Equal(username, returnValue.Username);
    }

    [Fact]
    public void GetAdminByUsernameAndPassword_AdminNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "wrongpassword";

        _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Returns((Admin)null);

        // Act
        var result = _adminController.GetAdminByUsernameAndPassword(username, password);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("no admin found with username nonexistentuser", response.Message);
    }

    [Fact]
    public void GetAdminByUsernameAndPassword_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var username = "admin1";
        var password = "password123";
        var exceptionMessage = "Unexpected error occurred.";

        _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Throws(new Exception(exceptionMessage));

        // Act
        var result = _adminController.GetAdminByUsernameAndPassword(username, password);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        var response = Assert.IsAssignableFrom<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An unexpected error occurred.", response.Message);
    }




}
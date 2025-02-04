// using FluentValidation;
// using FluentValidation.Results;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Smart_Event_Management_System.Controllers;
// using Smart_Event_Management_System.Dto;
// using Smart_Event_Management_System.Models;
// using Smart_Event_Management_System.Service;
// using Smart_Event_Management_System.CustomException;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace SmartEventManagementSystem_Tests.ControllerTests;
//
// public class AdminControllerTests
// {
//     private readonly ITestOutputHelper _testOutputHelper;
//     private readonly Mock<IAdminService> _mockAdminService;
//     private readonly Mock<IValidator<Admin>> _mockAdminValidator;
//     private readonly AdminController _AdminController;
//     
//     
//     public AdminControllerTests(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//         _mockAdminService = new Mock<IAdminService>();
//         _mockAdminValidator = new Mock<IValidator<Admin>>();
//         _AdminController = new AdminController(_mockAdminService.Object, _mockAdminValidator.Object);
//     }
//     
//     [Fact]
//     public async Task CreateAdmin_ValidInput_ReturnsOkResult()
//     {
//         var Admin = new Admin { Username = "Admin1", Email = "Admin1@example.com", Password = "password123" };
//         var validationResult = new ValidationResult();
//         _mockAdminValidator.Setup(v => v.ValidateAsync(Admin, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
//         _mockAdminService.Setup(s => s.CreateAdmin(Admin)).Returns(Admin);
//
//         var result = await _AdminController.CreateAdmin(Admin);
//
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         var returnValue = Assert.IsType<Admin>(okResult.Value);
//         Assert.Equal(Admin.Username, returnValue.Username);
//     }
//     
//     [Fact]
//     public async Task CreateAdmin_InvalidInput_ReturnsBadRequestWithErrors()
//     {
//         var Admin = new Admin { Username = "", Email = "invalidemail", Password = "" };
//
//         var validationResult = new ValidationResult(new List<ValidationFailure>
//         {
//             new ValidationFailure("Username", "Username is required."),
//             new ValidationFailure("Email", "Invalid email format."),
//             new ValidationFailure("HashPassword", "Password is required.")
//         });
//
//         _mockAdminValidator.Setup(v => v.ValidateAsync(Admin, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
//
//         var result = await _AdminController.CreateAdmin(Admin);
//
//         var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//         var response = Assert.IsType<ValidationErrorResponse>(badRequestResult.Value);
//         Assert.Equal("Validation errors occurred.", response.Message);
//         Assert.Equal(3, response.Errors.Count);
//
//         var usernameError = response.Errors.FirstOrDefault(e => e.PropertyName == "Username");
//         Assert.NotNull(usernameError);
//         Assert.Equal("Username is required.", usernameError.ErrorMessage);
//
//         var emailError = response.Errors.FirstOrDefault(e => e.PropertyName == "Email");
//         Assert.NotNull(emailError);
//         Assert.Equal("Invalid email format.", emailError.ErrorMessage);
//
//         var passwordError = response.Errors.FirstOrDefault(e => e.PropertyName == "HashPassword");
//         Assert.NotNull(passwordError);
//         Assert.Equal("Password is required.", passwordError.ErrorMessage);
//     }
//     
//     [Fact]
//     public void GetAdminByUsernameAndPassword_AdminFound_ReturnsOkResult()
//     {
//         var username = "Admin1";
//         var password = "password123";
//         var Admin = new Admin { Username = username, Email = "Admin1@example.com", Password = password };
//
//         _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Returns(Admin);
//
//         var result = _AdminController.GetAdminByUsernameAndPassword(username, password);
//
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnValue = Assert.IsType<Admin>(okResult.Value);
//         Assert.Equal(username, returnValue.Username);
//     }
//
//     // [Fact]
//     // public void GetAdminByUsernameAndPassword_AdminNotFound_ReturnsNotFoundResult()
//     // {
//     //     var username = "nonexistentuser";
//     //     var password = "wrongpassword";
//     //
//     //     // Mock the service to return null for this Admin
//     //     _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Returns((Admin)null);
//     //
//     //     // Call the controller
//     //     var result = _AdminController.GetAdminByUsernameAndPassword(username, password);
//     //
//     //     // Assert the result is a NotFoundObjectResult
//     //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
//     //
//     //     // Assert that the error message returned is correct
//     //     var response = Assert.IsAssignableFrom<ErrorResponse>(notFoundResult.Value);
//     //     Assert.Equal("no Admin found with username nonexistentuser", response.Message);
//     // }
//     //
//     //
//     // [Fact]
//     // public void GetAdminByUsernameAndPassword_ExceptionThrown_ReturnsInternalServerError()
//     // {
//     //     var username = "Admin1";
//     //     var password = "password123";
//     //     var exceptionMessage = "Unexpected error occurred.";
//     //
//     //     _mockAdminService.Setup(s => s.GetAdminByUsernameAndPassword(username, password)).Throws(new Exception(exceptionMessage));
//     //
//     //     var result = _AdminController.GetAdminByUsernameAndPassword(username, password);
//     //
//     //     var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
//     //     Assert.Equal(500, statusCodeResult.StatusCode);
//     //
//     //     var response = Assert.IsAssignableFrom<ErrorResponse>(statusCodeResult.Value);
//     //     Assert.Equal("An unexpected error occurred.", response.Message);
//     // }
// }

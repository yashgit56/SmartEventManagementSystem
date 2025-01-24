using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Smart_Event_Management_System.Validators;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")] 
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IValidator<Admin> _adminValidator;
    
    public AdminController(IAdminService adminService, IValidator<Admin> adminValidator)
    {
        _adminService = adminService;
        _adminValidator = adminValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
    {
        
        var validationResult = await _adminValidator.ValidateAsync(admin);

        if (!validationResult.IsValid) {
            var errors = validationResult.Errors
                .Select(error => new ValidationFailure(error.PropertyName, error.ErrorMessage))
                .ToList();

            var validationErrorResponse = new ValidationErrorResponse()
            {
                Message = "Validation errors occurred.",
                Errors = errors
            };

            return BadRequest(validationErrorResponse);
        }

        var createdAdmin = _adminService.CreateAdmin(admin);
        return Ok(createdAdmin);
    }

    [HttpGet]
    public ActionResult<Admin>? GetAdminByUsernameAndPassword(string username, string password)
    {
        try
        {
            var admin = _adminService.GetAdminByUsernameAndPassword(username, password);

            if (admin == null)
            {
                throw new NotFoundException($"no admin found with username {username}");
            }

            return Ok(admin);
        }
        catch (NotFoundException e)
        {
            return NotFound(new ErrorResponse()
            {
                Message = e.Message
            });
        }
        catch (Exception ex)
        {
            return new ObjectResult(new ErrorResponse()
            {
                Message = "An unexpected error occurred."
            })
            {
                StatusCode = 500
            };
        }
    }
}
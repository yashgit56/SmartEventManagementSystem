using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;
using Smart_Event_Management_System.Validators;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")] 
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly AdminValidator _adminValidator;
    
    public AdminController(IAdminService adminService, AdminValidator adminValidator)
    {
        _adminService = adminService;
        _adminValidator = adminValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
    {
        try
        {
            var validationResult = await _adminValidator.ValidateAsync(admin);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToString());
            }

            var createdAdmin = _adminService.CreateAdmin(admin);
            return Ok(createdAdmin);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Messsage = ex.Message
            });
        }
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
            Console.WriteLine(e.Message);
            return null;
        }
    }
}
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

    [HttpGet]
    public ActionResult<Admin>? GetAdminByUsernameAndPassword(string username, string password)
    {
        var Admin = _adminService.GetAdminByUsernameAndPassword(username, password);

        if (Admin == null)
        { 
            throw new NotFoundException($"no Admin found with username {username}");
        }

        return Ok(Admin);
    }
}
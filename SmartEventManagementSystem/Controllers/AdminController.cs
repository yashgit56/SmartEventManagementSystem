using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[Route("api/[controller]")] 
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("register")]
    public IActionResult CreateAdmin([FromBody] Admin admin)
    {
        if (!ModelState.IsValid) return BadRequest(new DataNotValidException("admin data is not in proper format"));

        var createdAdmin = _adminService.CreateAdmin(admin);
        return Ok(createdAdmin);
    }
}
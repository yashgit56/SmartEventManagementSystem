using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.Dto.AuthenticationDto;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Service;

namespace Smart_Event_Management_System.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAttendeeService _attendeeService;
    private readonly IConfiguration _configuration;
    private readonly IJwtService _jwtService;

    public AuthController(IAttendeeService attendeeService, IConfiguration configuration, IAdminService AdminService,
        IJwtService jwtService)
    {
        _attendeeService = attendeeService;
        _configuration = configuration;
        _adminService = AdminService;
        _jwtService = jwtService;
    }

    [HttpPost("Admin/login")]
    public IActionResult LoginAdmin([FromBody] LoginRequest request)
    {
        var AdminUser = _adminService.GetAdminByUsernameAndPassword(request.Username!, request.Password!);

        if (AdminUser == null) return Unauthorized(new { message = "Invalid username or password" });
        
        var token = _jwtService.GenerateToken(AdminUser, request);
        
        return Ok(new LoginResponse
        {
            Token = token
        });

    }
    
    [HttpPost("Attendee/login")]
    public IActionResult LoginUser([FromBody] LoginRequest request)
    {
        var attendeeUser = _attendeeService.GetAttendeeByUsernameAndPassword(request.Username!, request.Password!);

        if (attendeeUser == null) return Unauthorized(new { message = "Invalid username or password" });
        
        var token = _jwtService.GenerateToken(attendeeUser, request);
        
        return Ok(new LoginResponse
        {
            Token = token
        });

    }
}
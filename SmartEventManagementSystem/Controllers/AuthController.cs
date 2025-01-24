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

    public AuthController(IAttendeeService attendeeService, IConfiguration configuration, IAdminService adminService,
        IJwtService jwtService)
    {
        _attendeeService = attendeeService;
        _configuration = configuration;
        _adminService = adminService;
        _jwtService = jwtService;
    }

    [HttpPost("Admin/login")]
    public IActionResult LoginAdmin([FromBody] LoginRequest request)
    {
        Admin adminUser = null!;
        
        adminUser = _adminService.GetAdminByUsernameAndPassword(request.Username!, request.Password!);

        if (adminUser != null)
        {
            var token = _jwtService.GenerateToken(adminUser, request);
            Console.WriteLine(token);
            return Ok(new LoginResponse
            {
                Token = token,
                Role = request.Role
            });
        }

        return Unauthorized(new { message = "Invalid username or password" });
    }
    
    [HttpPost("Attendee/login")]
    public IActionResult LoginUser([FromBody] LoginRequest request)
    {
        Attendee attendeeUser = null!;

        if (request.Role == "Attendee")
            attendeeUser = _attendeeService.ValidateAttendee(request.Username!, request.Password!);

        if (attendeeUser != null)
        {
            var token = _jwtService.GenerateToken(attendeeUser, request);
            Console.WriteLine(token);
            return Ok(new LoginResponse
            {
                Token = token,
                Role = request.Role
            });
        }

        return Unauthorized(new { message = "Invalid username or password" });
    }
}
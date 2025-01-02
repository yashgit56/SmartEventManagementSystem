using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statusCode)
    {
        var errorMessage = "An unexpected error occurred.";
        var details = "Please contact support if the issue persists.";

        if (statusCode == 401)
        {
            errorMessage = "Unauthorized access.";
            details = "You are not authorized to access this page.";
        }
        else if (statusCode == 403)
        {
            errorMessage = "Forbidden access.";
            details = "You do not have permission to access this page.";
        }

        return View(new ErrorViewModel
        {
            ErrorMessage = errorMessage,
            StatusCode = statusCode,
            Details = details
        });
    }
}
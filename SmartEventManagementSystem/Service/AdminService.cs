using FluentValidation;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IValidator<Admin> _adminValidator;
    private readonly CustomLogicService _customLogicService;

    public AdminService(CustomLogicService customLogicService,
        IValidator<Admin> adminValidator, IAdminRepository adminRepository)
    {
        _customLogicService = customLogicService;
        _adminValidator = adminValidator;
        _adminRepository = adminRepository;
    }

    public Admin? ValidateAdmin(string username, string password)
    {
        var hashPassword = _customLogicService.HashPassword(password);

        var admin = _adminRepository.GetAdminByUsernameAndPassword(username, password);

        return admin;
    }
}
using FluentValidation;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly CustomLogicService _customLogicService;

    public AdminService(CustomLogicService customLogicService,
        IAdminRepository adminRepository)
    {
        _customLogicService = customLogicService;
        _adminRepository = adminRepository;
    }

    public Admin? GetAdminByUsernameAndPassword(string username, string password)
    {
        var hashPassword = _customLogicService.HashPassword(password);

        var admin = _adminRepository.GetAdminByUsernameAndPassword(username, hashPassword);

        if (admin == null)
        {
            throw new NotFoundException("Admin does not exist with that username");
        }

        return admin;
    }

    public Admin? CreateAdmin(Admin admin)
    {
        var hashPassword = _customLogicService.HashPassword(admin.HashPassword!);
        var createdAdmin = new Admin(admin.Username!,admin.Email!,hashPassword) ;
        var adminUser = _adminRepository.CreateAdmin(createdAdmin);

        if (adminUser == null)
        {
            throw new UserAlreadyExistException("Admin already exist with that email or username");
        }
        
        return adminUser; 
    }
}
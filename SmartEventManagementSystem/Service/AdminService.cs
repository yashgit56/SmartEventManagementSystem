using FluentValidation;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _AdminRepository;
    private readonly CustomLogicService _customLogicService;

    public AdminService(CustomLogicService customLogicService,
        IAdminRepository AdminRepository)
    {
        _customLogicService = customLogicService;
        _AdminRepository = AdminRepository;
    }

    public Admin? GetAdminByUsernameAndPassword(string username, string password)
    {
        var hashPassword = _customLogicService.HashPassword(password);

        var Admin = _AdminRepository.GetAdminByUsernameAndPassword(username, hashPassword);

        if (Admin == null)
        {
            throw new NotFoundException("Admin does not exist with that username");
        }

        return Admin;
    }
}
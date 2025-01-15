using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Service;

public interface IAdminService
{
    Admin ValidateAdmin(string username, string password);
    
    Admin CreateAdmin(Admin admin);
}
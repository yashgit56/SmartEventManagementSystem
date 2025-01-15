using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public interface IAdminRepository
{
    Admin? GetAdminByUsernameAndPassword(string username, string password);
    
    Admin CreateAdmin(Admin admin);
}
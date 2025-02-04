using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.Models;

namespace Smart_Event_Management_System.Repository;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Admin? GetAdminByUsernameAndPassword(string username, string password)
    {
        return _context.Admins.FirstOrDefault(manager =>
            manager.Username == username && manager.Password == password);
    }
}
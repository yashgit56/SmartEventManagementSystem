﻿using Smart_Event_Management_System.Context;
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
        return _context.Admins.FirstOrDefault(admin =>
            admin.Username == username && admin.HashPassword == password);
    }

    public Admin? CreateAdmin(Admin admin)
    {
        var adminUser = _context.Admins.FirstOrDefault(a => a.Username == admin.Username || a.Email == admin.Email);

        if (adminUser != null)
        {
            return null;
        }
        
        _context.Admins.Add(admin);
        _context.SaveChanges();
        return admin;
    }
}
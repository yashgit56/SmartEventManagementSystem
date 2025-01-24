using System.Security.Cryptography;
using System.Text;

namespace Smart_Event_Management_System.CustomLogic;

public class CustomLogicService
{
    public virtual string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
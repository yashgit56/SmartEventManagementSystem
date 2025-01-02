using System.Security.Claims;
using Smart_Event_Management_System.Dto.AuthenticationDto;

namespace Smart_Event_Management_System.Service;

public interface IJwtService
{
    string GenerateToken<T>(T user, LoginRequest request);
    bool ValidateToken(string token, out ClaimsPrincipal claimsPrincipal);
}
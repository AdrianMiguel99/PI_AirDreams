using System.Security.Claims;
using AirDreams.API.Models;

namespace AirDreams.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserModel user);
        ClaimsPrincipal? ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        DateTime GetTokenExpiration();
    }
}
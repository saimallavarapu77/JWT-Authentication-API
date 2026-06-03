using JWT_Authentication.Models;
using System.Threading.Tasks;

namespace JWT_Authentication.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Message)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string? Message, string? Token, DateTime? ExpiresAt)> AuthenticateAsync(LoginRequest request);
    }
}

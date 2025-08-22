using ASPIdentityApp.DTOs;

namespace ASPIdentityApp.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(string userId);
    }
}

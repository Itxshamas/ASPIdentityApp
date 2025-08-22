using ASPIdentityApp.DTOs;
using ASPIdentityApp.Entities;

namespace ASPIdentityApp.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<bool> LogoutAsync(string userId);
    }
}

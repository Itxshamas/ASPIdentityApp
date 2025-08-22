using ASPIdentityApp.DTOs;
using ASPIdentityApp.Entities;

namespace ASPIdentityApp.Interfaces
{
    public interface IUserProfileService
    {
        Task<IEnumerable<UserProfileDto>> GetAllAsync();
        Task<UserProfileDto?> GetByIdAsync(int id);
        Task<UserProfileDto> CreateAsync(UserProfileDto dto);
        Task<UserProfileDto?> UpdateAsync(int id, UserProfileDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

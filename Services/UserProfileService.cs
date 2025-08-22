using ASPIdentityApp.Data;
using ASPIdentityApp.DTOs;
using ASPIdentityApp.Entities;
using ASPIdentityApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto> CreateAsync(UserProfileDto dto)
        {
            var entity = new UserProfile
            {
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserProfiles.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.CreatedAt = entity.CreatedAt;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.UserProfiles.FindAsync(id);
            if (entity == null) return false;
            _context.UserProfiles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync()
        {
            return await _context.UserProfiles
                .Select(p => new UserProfileDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToListAsync();
        }

        public async Task<UserProfileDto?> GetByIdAsync(int id)
        {
            var p = await _context.UserProfiles.FindAsync(id);
            if (p == null) return null;
            return new UserProfileDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        public async Task<UserProfileDto?> UpdateAsync(int id, UserProfileDto dto)
        {
            var entity = await _context.UserProfiles.FindAsync(id);
            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.UserProfiles.Update(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.UpdatedAt = entity.UpdatedAt;
            dto.CreatedAt = entity.CreatedAt;
            return dto;
        }
    }
}

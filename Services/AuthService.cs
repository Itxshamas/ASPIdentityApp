using ASPIdentityApp.Data;
using ASPIdentityApp.DTOs;
using ASPIdentityApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASPIdentityApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Save registration info
            _context.RegisterRequests.Add(request);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                Email = request.Email,
                UserId = request.Id.ToString(),
                Token = Guid.NewGuid().ToString(),
                Expiration = DateTime.UtcNow.AddHours(1)
            };

            _context.AuthResponses.Add(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.RegisterRequests
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var response = new AuthResponse
            {
                Email = user.Email,
                UserId = user.Id.ToString(),
                Token = tokenString,
                Expiration = token.ValidTo
            };

            // Storing the token in the database might not be necessary if you are not tracking active sessions.
            // If you do need to track active sessions, you should store the token and its expiration.
            _context.AuthResponses.Add(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var tokens = await _context.AuthResponses
                .Where(r => r.UserId == userId)
                .ToListAsync();

            _context.AuthResponses.RemoveRange(tokens);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

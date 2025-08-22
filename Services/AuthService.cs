using ASPIdentityApp.Data;
using ASPIdentityApp.DTOs;
using ASPIdentityApp.Entities;
using ASPIdentityApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASPIdentityApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<RegisterRequest> _passwordHasher;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<RegisterRequest>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // check if email exists
            var exists = await _context.RegisterRequests.AnyAsync(r => r.Email == request.Email);
            if (exists) throw new Exception("Email already registered.");

            var registerEntity = new RegisterRequest
            {
                Email = request.Email
            };

            // hash password
            registerEntity.Password = _passwordHasher.HashPassword(registerEntity, request.Password);

            _context.RegisterRequests.Add(registerEntity);
            await _context.SaveChangesAsync();

            // Generate token
            var token = GenerateJwtToken(registerEntity.Id.ToString(), registerEntity.Email, out DateTime expiry);

            var authResponse = new AuthResponse
            {
                UserId = registerEntity.Id.ToString(),
                Email = registerEntity.Email,
                Token = token,
                Expiration = expiry
            };

            _context.AuthResponses.Add(authResponse);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = authResponse.Token,
                Expiration = authResponse.Expiration,
                Email = authResponse.Email,
                UserId = authResponse.UserId
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.RegisterRequests.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");

            var verification = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            // If password doesn't match hashed value, but stored value equals plain text (legacy), accept and re-hash.
            if (verification == PasswordVerificationResult.Failed)
            {
                if (user.Password == request.Password)
                {
                    // legacy plaintext password — re-hash for future
                    user.Password = _passwordHasher.HashPassword(user, request.Password);
                    _context.RegisterRequests.Update(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }
            }

            // generate JWT
            var token = GenerateJwtToken(user.Id.ToString(), user.Email, out DateTime expiry);

            var response = new AuthResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Token = token,
                Expiration = expiry
            };

            _context.AuthResponses.Add(response);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = response.Token,
                Expiration = response.Expiration,
                Email = response.Email,
                UserId = response.UserId
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var tokens = await _context.AuthResponses.Where(a => a.UserId == userId).ToListAsync();
            if (tokens.Any())
            {
                _context.AuthResponses.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        private string GenerateJwtToken(string userId, string email, out DateTime validTo)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            validTo = token.ValidTo;
            return tokenHandler.WriteToken(token);
        }
    }
}

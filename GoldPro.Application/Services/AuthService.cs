using GoldPro.Application.DTOs.AuthDtos;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GoldPro.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, ITenantContext tenant, IConfiguration config)
        {
            _db = db;
            _tenant = tenant;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            // basic uniqueness check
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) throw new InvalidOperationException("Email already registered");

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // return JWT
            return GenerateToken(user);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId==dto.TenantId);
            var user = await _db.Users
     .IgnoreQueryFilters()
     .AsNoTracking()
     .FirstOrDefaultAsync(u =>
         u.Email == dto.Email &&
         u.TenantId == dto.TenantId);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return GenerateToken(user);
        }

        private string GenerateToken(AppUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? "Admin"), // BEST PRACTICE
                    new Claim("tenant_id", user.TenantId.ToString())
                };

            var keyString =
                 Environment.GetEnvironmentVariable("Key")
                 ?? _config["Jwt:Key"]
                 ?? throw new InvalidOperationException("JWT key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256 //  CRITICAL FIX
            );

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("Issuer") ?? _config["Jwt:Issuer"],
                audience: Environment.GetEnvironmentVariable("Audience") ?? _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _db.Users
        .AsNoTracking()
        .Where(u => u.TenantId == _tenant.TenantId)
        .Select(u => new AppUser
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role,
            TenantId = u.TenantId
            // PasswordHash intentionally excluded
        })
        .ToListAsync();
        }

    }
}

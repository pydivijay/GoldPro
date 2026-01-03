using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly AppDbContext _db;
        private readonly IAuthService _auth;

        public TenantService(AppDbContext db, IAuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        public async Task<(string token, Guid tenantId)> CreateTenantAsync(RegisterTenantDto dto)
        {
            var tenantId = Guid.NewGuid();

            var tenant = new Tenant
            {
                Id = tenantId,
                Name = dto.BusinessName
            };

            await _db.Tenants.AddAsync(tenant);

            // Create admin user
            var registerDto = new DTOs.AuthDtos.RegisterDto(dto.OwnerName, dto.Email, dto.Phone, dto.Password);
            // AuthService.RegisterAsync will create user with TenantId from TenantContext normally.
            // We need to temporarily set TenantContext or create user directly.

            // Create user directly to ensure TenantId is set
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FullName = dto.OwnerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            // Generate token using AuthService.LoginAsync pattern
            var token = await _auth.LoginAsync(new DTOs.AuthDtos.LoginDto(dto.Email, dto.Password));

            return (token, tenantId);
        }
    }
}

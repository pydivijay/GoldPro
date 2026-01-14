using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoldPro.Api.Controllers.Settings
{
    [Authorize]
    [ApiController]
    [Route("api/settings/user")]
    public class UserAccountController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TenantContext _tenant;

        public UserAccountController(AppDbContext db, TenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // ensure the user belongs to the current tenant and handle missing user gracefully
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId && x.TenantId == _tenant.TenantId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> Update(AppUser model)
        {
            model.TenantId = _tenant.TenantId;
            _db.Users.Update(model);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId && x.TenantId == _tenant.TenantId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest("Invalid current password");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}

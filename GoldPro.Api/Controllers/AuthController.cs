using GoldPro.Application.DTOs;
using GoldPro.Application.DTOs.AuthDtos;
using GoldPro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoldPro.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ITenantService _tenantService;

        public AuthController(IAuthService auth, ITenantService tenantService)
        {
            _auth = auth;
            _tenantService = tenantService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _auth.RegisterAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _auth.LoginAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("register-tenant")]
        public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantDto dto)
        {
            var (token, tenantId) = await _tenantService.CreateTenantAsync(dto);
            return Ok(new { token, tenantId });
        }
    }
}

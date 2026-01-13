using GoldPro.Application.DTOs.AuthDtos;
using GoldPro.Domain.Entities;

namespace GoldPro.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<List<AppUser>> GetUsersAsync();
    }
}

using GoldPro.Application.DTOs;

namespace GoldPro.Application.Interfaces
{
    public interface ITenantService
    {
        Task<(string token, Guid tenantId)> CreateTenantAsync(RegisterTenantDto dto);
    }
}

namespace GoldPro.Application.DTOs.AuthDtos
{
    public record LoginDto(string Email, string Password,Guid TenantId);
}

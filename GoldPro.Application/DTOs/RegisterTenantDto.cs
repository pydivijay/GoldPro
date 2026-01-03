namespace GoldPro.Application.DTOs
{
    public record RegisterTenantDto(string BusinessName, string OwnerName, string Email, string Phone, string Password);
}

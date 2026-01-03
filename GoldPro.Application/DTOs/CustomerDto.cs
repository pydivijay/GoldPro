namespace GoldPro.Application.DTOs
{
    public record CustomerDto(
        Guid Id,
        string FullName,
        string PhoneNumber,
        string? Email,
        string? Address,
        string? Gstin,
        DateTime CreatedAt
    );
}

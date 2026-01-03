namespace GoldPro.Application.DTOs
{
    public record CreateCustomerDto(
        string FullName,
        string PhoneNumber,
        string? Email,
        string? Address,
        string? Gstin,
        DateTime? DateTime
    );
}

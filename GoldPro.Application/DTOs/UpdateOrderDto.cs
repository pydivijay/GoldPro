using System;

namespace GoldPro.Application.DTOs
{
    public record UpdateOrderDto(
        DateTime? DueDate,
        string? Notes,
        decimal AdvanceReceived,
        string PaymentStatus
    );
}

using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record CreateOrderItemDto(
        string Description,
        string Purity,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal MakingCharges,
        decimal WastagePercent,
        string? Instructions
    );

    public record CreateOrderDto(
        Guid? CustomerId,
        DateTime? DueDate,
        string? Notes,
        IEnumerable<CreateOrderItemDto> Items,
        decimal AdvanceReceived,
        string PaymentMethod,
        string PaymentStatus,
        DateTime? DateTime
    );
}

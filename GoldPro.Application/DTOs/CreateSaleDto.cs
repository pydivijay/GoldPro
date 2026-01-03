using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record CreateSaleItemDto(
        string Description,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal WastagePercent,
        string Purity,
        decimal MakingCharges
    );

    public record CreateSaleDto(
        Guid? CustomerId,
        bool IsInterState,
        IEnumerable<CreateSaleItemDto> Items,
        string PaymentMethod,
        string PaymentStatus,
        DateTime? DateTime
    );
}

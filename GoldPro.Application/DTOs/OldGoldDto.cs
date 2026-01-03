using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record OldGoldItemDto(
        Guid Id,
        string Purity,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal DeductionPercent,
        string? Description,
        decimal GrossValue,
        decimal DeductionValue,
        decimal NetValue
    );

    public record OldGoldSlipDto(
        Guid Id,
        Guid? CustomerId,
        string? CustomerName,
        IEnumerable<OldGoldItemDto> Items,
        decimal GoldValue,
        decimal DeductionPercent,
        decimal DeductionValue,
        decimal NetPayable,
        DateTime CreatedAt
    );
}

using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record CreateOldGoldItemDto(
        string Purity,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal DeductionPercent,
        string? Description
    );

    public record CreateOldGoldSlipDto(
        Guid? CustomerId,
        string? customerName,
        string? Notes,
        IEnumerable<CreateOldGoldItemDto> Items,
        DateTime? DateTime
    );
}

using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record CreateEstimateItemDto(
        string Description,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal MakingCharges,
        decimal WastagePercent,
        string Purity
    );

    public record CreateEstimateDto(
        Guid? CustomerId,
        bool IsInterState,
        IEnumerable<CreateEstimateItemDto> Items,
        DateTime? DateTime
    );
}

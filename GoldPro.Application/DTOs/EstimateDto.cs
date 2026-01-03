using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record EstimateItemDto(
        Guid Id,
        string Description,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal MakingCharges,
        decimal WastagePercent,
        string Purity,
        decimal GoldValue,
        decimal DeductionValue,
        decimal GstValue
    );

    public record EstimateDto(
        Guid Id,
        Guid? CustomerId,
        string? CustomerName,
        bool IsInterState,
        IEnumerable<EstimateItemDto> Items,
        decimal GoldValue,
        decimal MakingCharges,
        decimal Deduction,
        decimal Subtotal,
        decimal GstPercent,
        decimal Cgst,
        decimal Sgst,
        decimal Igst,
        decimal TotalGstAmount,
        decimal EstimatedTotal,
        DateTime CreatedAt
    );
}

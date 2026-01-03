using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record OrderItemDto(
        Guid Id,
        string Description,
        string Purity,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal MakingCharges,
        decimal WastagePercent,
        decimal GoldValue,
        decimal DeductionValue,
        decimal GstValue,
        string? Instructions
    );

    public record OrderDto(
        Guid Id,
        Guid? CustomerId,
        string? CustomerName,
        DateTime? DueDate,
        string? Notes,
        IEnumerable<OrderItemDto> Items,
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
        decimal AdvanceReceived,
        decimal AmountPayable,
        string PaymentMethod,
        string PaymentStatus,
        DateTime CreatedAt
    );
}

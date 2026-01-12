using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs
{
    public record SaleItemDto(
        Guid Id,
        string Description,
        decimal WeightGrams,
        decimal RatePerGram,
        decimal WastagePercent,
        string Purity,
        decimal MakingCharges,
        decimal GoldValue,
        decimal DeductionValue,
        decimal GstValue,
        string InvoiceNo
    );

    public record SaleDto(
        Guid Id,
        Guid? CustomerId,
        string InvoiceNo,
        string? CustomerName,
        bool IsInterState,
        IEnumerable<SaleItemDto> Items,
        decimal GoldValue,
        decimal MakingCharges,
        decimal Deduction,
        decimal Subtotal,
        decimal GstPercent,
        decimal Cgst,
        decimal Sgst,
        decimal Igst,
        decimal TotalGstAmount,
        decimal GrandTotal,
        decimal TotalAmount,
        string PaymentMethod,
        string PaymentStatus,
        DateTime CreatedAt
    );
}

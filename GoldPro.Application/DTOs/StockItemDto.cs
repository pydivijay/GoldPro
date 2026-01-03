using System;

namespace GoldPro.Application.DTOs
{
    public record StockItemDto(
        Guid Id,
        string Name,
        string Type,
        string Purity,
        decimal WeightGrams,
        int Quantity,
        string HsnCode,
        DateTime CreatedAt
    );
}

namespace GoldPro.Application.DTOs
{
    public record CreateStockItemDto(
        string Name,
        string Type,
        string Purity,
        decimal WeightGrams,
        int Quantity,
        string HsnCode
    );
}

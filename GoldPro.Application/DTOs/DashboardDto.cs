using System;

namespace GoldPro.Application.DTOs
{
    public record StockBreakdownDto(string Purity, decimal WeightGrams);

    public record DashboardDto(
        decimal TodaysSales,
        decimal MonthlySales,
        decimal GstCollected,
        int TotalCustomers,
        (string InvoiceNo, string CustomerName, DateTime Date, decimal Amount, string Status)[] RecentInvoices,
        StockBreakdownDto[] StockBreakdown,
        decimal TotalStock
    );
}

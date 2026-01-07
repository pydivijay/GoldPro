using System;

namespace GoldPro.Application.DTOs
{
    public record StockBreakdownDto(string Purity, decimal WeightGrams,string Type);

    public record RecentInvoiceDto(string InvoiceNo, string CustomerName, DateTime Date, decimal Amount, string Status);

    public record DashboardDto(
        decimal TodaysSales,
        decimal MonthlySales,
        decimal GstCollected,
        int TotalCustomers,
        RecentInvoiceDto[] RecentInvoices,
        StockBreakdownDto[] StockBreakdown,
        decimal TotalStock
    );
}

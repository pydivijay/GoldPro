using System;
using System.Collections.Generic;

namespace GoldPro.Application.DTOs.Reports
{
    public record ReportDateRange(DateTime From, DateTime To);

    public record SalesReportRow(DateTime Date, decimal SalesAmount, decimal Cgst, decimal Sgst, decimal Igst, decimal TotalGst);
    public record SalesReportDto(IEnumerable<SalesReportRow> Rows, decimal TotalSales, decimal TotalCgst, decimal TotalSgst, decimal TotalIgst, decimal TotalGst);

    public record GstSummaryRow(DateTime Date, decimal Cgst, decimal Sgst, decimal Igst, decimal TotalGst);
    public record GstSummaryDto(IEnumerable<GstSummaryRow> Rows, decimal TotalCgst, decimal TotalSgst, decimal TotalIgst, decimal TotalGst);

    public record InventoryItemRow(string Name, decimal Opening, decimal Inward, decimal Outward, decimal Closing);
    public record InventoryReportDto(IEnumerable<InventoryItemRow> Items, decimal OpeningTotal, decimal InwardTotal, decimal OutwardTotal, decimal ClosingTotal);

    public record CustomerReportRow(Guid CustomerId, string CustomerName, decimal TotalPurchases, int OrdersCount);
    public record CustomerReportDto(IEnumerable<CustomerReportRow> Rows, decimal TotalPurchases,string topCustomerName);
}

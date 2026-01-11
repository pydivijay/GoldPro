using GoldPro.Application.DTOs.Reports;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public ReportService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to)
        {
            var rows = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Date >= from.Date && s.CreatedAt.Date <= to.Date)
                .GroupBy(s => s.CreatedAt.Date)
                .Select(g => new SalesReportRow(
                    DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),
                    g.Sum(x => x.GrandTotal),
                    g.Sum(x => x.Cgst),
                    g.Sum(x => x.Sgst),
                    g.Sum(x => x.Igst),
                    g.Sum(x => x.TotalGstAmount)
                ))
                .OrderBy(r => r.Date)
                .ToListAsync();

            var totalSales = rows.Sum(r => r.SalesAmount);
            var totalCgst = rows.Sum(r => r.Cgst);
            var totalSgst = rows.Sum(r => r.Sgst);
            var totalIgst = rows.Sum(r => r.Igst);
            var totalGst = rows.Sum(r => r.TotalGst);

            return new SalesReportDto(rows, totalSales, totalCgst, totalSgst, totalIgst, totalGst);
        }

        public async Task<GstSummaryDto> GetGstSummaryAsync(DateTime from, DateTime to)
        {
            var rows = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Date >= from.Date && s.CreatedAt.Date <= to.Date)
                .GroupBy(s => s.CreatedAt.Date)
                .Select(g => new GstSummaryRow(
                    DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),
                    g.Sum(x => x.Cgst),
                    g.Sum(x => x.Sgst),
                    g.Sum(x => x.Igst),
                    g.Sum(x => x.TotalGstAmount)
                ))
                .OrderBy(r => r.Date)
                .ToListAsync();

            var totalCgst = rows.Sum(r => r.Cgst);
            var totalSgst = rows.Sum(r => r.Sgst);
            var totalIgst = rows.Sum(r => r.Igst);
            var totalGst = rows.Sum(r => r.TotalGst);

            return new GstSummaryDto(rows, totalCgst, totalSgst, totalIgst, totalGst);
        }

        public async Task<InventoryReportDto> GetInventoryReportAsync(DateTime from, DateTime to)
        {
            // opening stock = sum of stock for this tenant
            var openingGroups = await _db.StockItems
                .Where(s => s.TenantId == _tenant.TenantId)
                .GroupBy(s => s.Name)
                .Select(g => new { Name = g.Key, Weight = g.Sum(x => x.WeightGrams * x.Quantity) })
                .ToListAsync();

            // For demo, inward/outward are derived from Orders/Sales during period
            var inward = await _db.Orders
                .Where(o => o.TenantId == _tenant.TenantId && o.CreatedAt.Date >= from.Date && o.CreatedAt.Date <= to.Date)
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Purity)
                .Select(g => new { Purity = g.Key, Weight = g.Sum(x => x.WeightGrams) })
                .ToListAsync();

            var outward = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Date >= from.Date && s.CreatedAt.Date <= to.Date)
                .SelectMany(s => s.Items)
                .GroupBy(i => i.Purity)
                .Select(g => new { Purity = g.Key, Weight = g.Sum(x => x.WeightGrams) })
                .ToListAsync();

            var items = openingGroups.Select(g =>
            {
                var inw = inward.FirstOrDefault(x => x.Purity == g.Name)?.Weight ?? 0m;
                var outw = outward.FirstOrDefault(x => x.Purity == g.Name)?.Weight ?? 0m;
                var closing = g.Weight + inw - outw;
                return new InventoryItemRow(g.Name, g.Weight, inw, outw, closing);
            }).ToList();

            var openingTotal = items.Sum(i => i.Opening);
            var inwardTotal = items.Sum(i => i.Inward);
            var outwardTotal = items.Sum(i => i.Outward);
            var closingTotal = items.Sum(i => i.Closing);

            return new InventoryReportDto(items, openingTotal, inwardTotal, outwardTotal, closingTotal);
        }

        public async Task<CustomerReportDto> GetCustomerReportAsync(DateTime from, DateTime to)
        {
            var rows = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Date >= from.Date && s.CreatedAt.Date <= to.Date && s.CustomerId != null)
                .GroupBy(s => s.CustomerId)
                .Select(g => new CustomerReportRow(g.Key ?? Guid.Empty, g.FirstOrDefault().CustomerName ?? string.Empty, g.Sum(x => x.GrandTotal), g.Count()))
                .ToListAsync();

            var totalPurchases = rows.Sum(r => r.TotalPurchases);
            return new CustomerReportDto(rows, totalPurchases);
        }
    }
}

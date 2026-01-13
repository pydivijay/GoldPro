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
            //  Force UTC
            var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
            var toUtcExclusive = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

            var rows = await _db.Database
                .SqlQuery<SalesReportRow>($@"
            SELECT
                s.""CreatedAt""::date      AS ""Date"",
                SUM(s.""GrandTotal"")      AS ""SalesAmount"",
                SUM(s.""Cgst"")            AS ""Cgst"",
                SUM(s.""Sgst"")            AS ""Sgst"",
                SUM(s.""Igst"")            AS ""Igst"",
                SUM(s.""TotalGstAmount"")  AS ""TotalGst""
            FROM ""Sales"" s
            WHERE s.""TenantId"" = {_tenant.TenantId}
              AND s.""CreatedAt"" >= {fromUtc}
              AND s.""CreatedAt"" < {toUtcExclusive}
            GROUP BY s.""CreatedAt""::date
            ORDER BY ""Date""
        ")
                .ToListAsync();

            return new SalesReportDto(
                rows,
                rows.Sum(r => r.SalesAmount),
                rows.Sum(r => r.Cgst),
                rows.Sum(r => r.Sgst),
                rows.Sum(r => r.Igst),
                rows.Sum(r => r.TotalGst)
            );
        }


        public async Task<GstSummaryDto> GetGstSummaryAsync(DateTime from, DateTime to)
        {
            // PostgreSQL timestamptz requires UTC
            var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
            var toUtcExclusive = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

            var rows = await _db.Database
                .SqlQuery<GstSummaryRow>($@"
            SELECT
                s.""CreatedAt""::date        AS ""Date"",
                SUM(s.""Cgst"")              AS ""Cgst"",
                SUM(s.""Sgst"")              AS ""Sgst"",
                SUM(s.""Igst"")              AS ""Igst"",
                SUM(s.""TotalGstAmount"")    AS ""TotalGst""
            FROM ""Sales"" s
            WHERE s.""TenantId"" = {_tenant.TenantId}
              AND s.""CreatedAt"" >= {fromUtc}
              AND s.""CreatedAt"" < {toUtcExclusive}
            GROUP BY s.""CreatedAt""::date
            ORDER BY ""Date""
        ")
                .AsNoTracking()
                .ToListAsync();

            return new GstSummaryDto(
                rows,
                rows.Sum(r => r.Cgst),
                rows.Sum(r => r.Sgst),
                rows.Sum(r => r.Igst),
                rows.Sum(r => r.TotalGst)
            );
        }


        public async Task<InventoryReportDto> GetInventoryReportAsync(DateTime from, DateTime to)
        {
            //  Convert ONCE and use everywhere
            var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
            var toUtcExclusive = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

            // Opening stock (no date filter OK)
            var openingGroups = await _db.StockItems
                .Where(s => s.TenantId == _tenant.TenantId)
                .GroupBy(s => s.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Weight = g.Sum(x => x.WeightGrams * x.Quantity)
                })
                .ToListAsync();

            // Inward (Orders)
            var inward = await _db.Orders
                .Where(o =>
                    o.TenantId == _tenant.TenantId &&
                    o.CreatedAt >= fromUtc &&
                    o.CreatedAt < toUtcExclusive
                )
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Purity)
                .Select(g => new
                {
                    Purity = g.Key,
                    Weight = g.Sum(x => x.WeightGrams)
                })
                .ToListAsync();

            // Outward (Sales)
            var outward = await _db.Sales
                .Where(s =>
                    s.TenantId == _tenant.TenantId &&
                    s.CreatedAt >= fromUtc &&
                    s.CreatedAt < toUtcExclusive
                )
                .SelectMany(s => s.Items)
                .GroupBy(i => i.Purity)
                .Select(g => new
                {
                    Purity = g.Key,
                    Weight = g.Sum(x => x.WeightGrams)
                })
                .ToListAsync();

            var items = openingGroups.Select(g =>
            {
                var inw = inward.FirstOrDefault(x => x.Purity == g.Name)?.Weight ?? 0m;
                var outw = outward.FirstOrDefault(x => x.Purity == g.Name)?.Weight ?? 0m;
                var closing = g.Weight + inw - outw;

                return new InventoryItemRow(
                    g.Name,
                    g.Weight,
                    inw,
                    outw,
                    closing
                );
            }).ToList();

            return new InventoryReportDto(
                items,
                items.Sum(i => i.Opening),
                items.Sum(i => i.Inward),
                items.Sum(i => i.Outward),
                items.Sum(i => i.Closing)
            );
        }


        public async Task<CustomerReportDto> GetCustomerReportAsync(DateTime from, DateTime to)
        {
            // Convert ONCE
            var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
            var toUtcExclusive = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

            var rows = await (
                from s in _db.Sales
                join c in _db.Customers
                    on s.CustomerId equals c.Id into customerJoin
                from c in customerJoin.DefaultIfEmpty()
                where s.TenantId == _tenant.TenantId
                      && s.CustomerId != null
                      && s.CreatedAt >= fromUtc
                      && s.CreatedAt < toUtcExclusive
                group new { s, c } by new
                {
                    s.CustomerId,
                    CustomerName = !string.IsNullOrEmpty(s.CustomerName)
                        ? s.CustomerName
                        : c!.FullName
                }
                into g
                select new CustomerReportRow(
                    g.Key.CustomerId!.Value,
                    g.Key.CustomerName ?? string.Empty,
                    g.Sum(x => x.s.GrandTotal),
                    g.Count()
                )
            ).ToListAsync();

            var totalPurchases = rows.Sum(r => r.TotalPurchases);

            var topCustomer = rows
                .OrderByDescending(r => r.TotalPurchases)
                .FirstOrDefault();

            return new CustomerReportDto(
                rows,
                totalPurchases,
                topCustomerName: topCustomer?.CustomerName ?? string.Empty
            );
        }

    }
}

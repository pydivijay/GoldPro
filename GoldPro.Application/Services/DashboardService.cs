using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public DashboardService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<DashboardDto> GetSummaryAsync()
        {
            // Today's sales: sum of Sales created today
            var today = DateTime.UtcNow.Date;
            var todaysSales = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Date == today)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // Monthly sales: sum for current month
            var now = DateTime.UtcNow;
            var monthlySales = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId && s.CreatedAt.Year == now.Year && s.CreatedAt.Month == now.Month)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // GST collected: sum of TotalGstAmount for completed sales (assume all sales are relevant)
            var gstCollected = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId)
                .SumAsync(s => (decimal?)s.TotalGstAmount) ?? 0m;

            // Total customers
            var totalCustomers = await _db.Customers.Where(c => c.TenantId == _tenant.TenantId).CountAsync();

            // Recent invoices: use Sales as invoice placeholders; convert top 3
            var recent = await _db.Sales
                .Where(s => s.TenantId == _tenant.TenantId)
                .OrderByDescending(s => s.CreatedAt)
                .Take(3)
                .ToListAsync();

            // Load customer names for referenced customer ids
            var customerIds = recent.Where(s => s.CustomerId.HasValue).Select(s => s.CustomerId!.Value).Distinct().ToList();
            var customerMap = new Dictionary<Guid, string>();
            if (customerIds.Any())
            {
                var custs = await _db.Customers
                    .AsNoTracking()
                    .Where(c => customerIds.Contains(c.Id) && c.TenantId == _tenant.TenantId)
                    .ToListAsync();
                customerMap = custs.ToDictionary(c => c.Id, c => c.FullName);
            }

            var recentInvoices = recent.Select(s =>
            {
                string custName = s.CustomerName ?? "-";
                if (s.CustomerId.HasValue && customerMap.TryGetValue(s.CustomerId.Value, out var name))
                {
                    custName = name;
                }
                return new RecentInvoiceDto(s.InvoiceNo.ToString(), custName, s.CreatedAt, s.GrandTotal, s.PaymentStatus);
            }).ToArray();

            // Stock breakdown - aggregate by purity and type from StockItems
            var stockGroups = await _db.StockItems
                .Where(x => x.TenantId == _tenant.TenantId)
                .GroupBy(x => new { x.Purity, x.Type })
                .Select(g => new { Purity = g.Key.Purity, Type = g.Key.Type, Weight = g.Sum(x => x.WeightGrams * x.Quantity) })
                .ToListAsync();
            var stockBreakdown = stockGroups.Select(g => new StockBreakdownDto(g.Purity, g.Weight, g.Type)).ToArray();
            var totalStock = stockGroups.Sum(g => g.Weight);

            return new DashboardDto(todaysSales, monthlySales, gstCollected, totalCustomers, recentInvoices, stockBreakdown, totalStock);
        }
    }
}

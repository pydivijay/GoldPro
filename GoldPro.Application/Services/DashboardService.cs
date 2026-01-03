using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;

        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetSummaryAsync()
        {
            // Today's sales: sum of Sales created today
            var today = DateTime.UtcNow.Date;
            var todaysSales = await _db.Sales.Where(s => s.CreatedAt.Date == today).SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // Monthly sales: sum for current month
            var now = DateTime.UtcNow;
            var monthlySales = await _db.Sales.Where(s => s.CreatedAt.Year == now.Year && s.CreatedAt.Month == now.Month)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // GST collected: sum of TotalGstAmount for completed sales (assume all sales are relevant)
            var gstCollected = await _db.Sales.SumAsync(s => (decimal?)s.TotalGstAmount) ?? 0m;

            // Total customers
            var totalCustomers = await _db.Customers.CountAsync();

            // Recent invoices: use Sales as invoice placeholders; convert top 3
            var recent = await _db.Sales.OrderByDescending(s => s.CreatedAt).Take(3).ToListAsync();
            var recentInvoices = recent.Select(s => (InvoiceNo: s.Id.ToString(), CustomerName: s.CustomerName ?? "-", Date: s.CreatedAt, Amount: s.GrandTotal, Status: s.PaymentStatus)).ToArray();

            // Stock breakdown - aggregate by purity from StockItems
            var stockGroups = await _db.StockItems.GroupBy(x => x.Purity).Select(g => new { Purity = g.Key, Weight = g.Sum(x => x.WeightGrams * x.Quantity) }).ToListAsync();
            var stockBreakdown = stockGroups.Select(g => new StockBreakdownDto(g.Purity, g.Weight)).ToArray();
            var totalStock = stockGroups.Sum(g => g.Weight);

            return new DashboardDto(todaysSales, monthlySales, gstCollected, totalCustomers, recentInvoices, stockBreakdown, totalStock);
        }
    }
}

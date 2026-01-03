using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public SaleService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<SaleDto> CreateAsync(CreateSaleDto dto)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                CustomerId = dto.CustomerId,
                IsInterState = dto.IsInterState,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow
            };

            decimal totalGoldValue = 0;
            decimal totalMaking = 0;
            decimal totalDeduction = 0;
            decimal totalGst = 0;

            foreach (var it in dto.Items)
            {
                var item = new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = sale.Id,
                    Description = it.Description,
                    WeightGrams = it.WeightGrams,
                    RatePerGram = it.RatePerGram,
                    WastagePercent = it.WastagePercent,
                    Purity = it.Purity,
                    MakingCharges = it.MakingCharges
                };

                // Gold value calculation
                var gross = item.WeightGrams * item.RatePerGram;
                var deduction = gross * (item.WastagePercent / 100m);
                var goldValue = gross - deduction;

                item.GoldValue = goldValue;
                item.DeductionValue = deduction;

                totalGoldValue += goldValue;
                totalMaking += item.MakingCharges;
                totalDeduction += deduction;

                sale.Items.Add(item);
            }

            sale.GoldValue = totalGoldValue;
            sale.MakingCharges = totalMaking;
            sale.Deduction = totalDeduction;
            sale.Subtotal = sale.GoldValue + sale.MakingCharges - sale.Deduction;

            // GST calculation: assume 3% total (1.5% CGST + 1.5% SGST) for intra-state, IGST for inter-state
            sale.GstPercent = 3m;
            if (sale.IsInterState)
            {
                sale.Igst = Math.Round(sale.Subtotal * (sale.GstPercent / 100m), 2);
                sale.Cgst = 0;
                sale.Sgst = 0;
                totalGst = sale.Igst;
            }
            else
            {
                sale.Igst = 0;
                sale.Cgst = Math.Round(sale.Subtotal * (sale.GstPercent / 2 / 100m), 2);
                sale.Sgst = Math.Round(sale.Subtotal * (sale.GstPercent / 2 / 100m), 2);
                totalGst = sale.Cgst + sale.Sgst;
            }

            sale.TotalGstAmount = totalGst;
            sale.GrandTotal = sale.Subtotal + totalGst;
            sale.TotalAmount = sale.GrandTotal;

            // Persist GstValue per item proportionally
            if (sale.Items.Any() && sale.TotalGstAmount > 0)
            {
                foreach (var item in sale.Items)
                {
                    var proportion = item.GoldValue / sale.GoldValue;
                    item.GstValue = Math.Round(sale.TotalGstAmount * proportion, 2);
                }
            }

            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();

            return await GetAsync(sale.Id) as SaleDto;
        }

        public async Task<SaleDto?> GetAsync(Guid id)
        {
            var s = await _db.Sales.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return null;

            var items = s.Items.Select(i => new SaleItemDto(i.Id, i.Description, i.WeightGrams, i.RatePerGram, i.WastagePercent, i.Purity, i.MakingCharges, i.GoldValue, i.DeductionValue, i.GstValue));

            return new SaleDto(s.Id, s.CustomerId, s.CustomerName, s.IsInterState, items, s.GoldValue, s.MakingCharges, s.Deduction, s.Subtotal, s.GstPercent, s.Cgst, s.Sgst, s.Igst, s.TotalGstAmount, s.GrandTotal, s.TotalAmount, s.PaymentMethod, s.PaymentStatus, s.CreatedAt);
        }

        public async Task<IEnumerable<SaleDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            var query = _db.Sales.Include(x => x.Items).OrderByDescending(x => x.CreatedAt).AsQueryable();
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return list.Select(s =>
            {
                var items = s.Items.Select(i => new SaleItemDto(i.Id, i.Description, i.WeightGrams, i.RatePerGram, i.WastagePercent, i.Purity, i.MakingCharges, i.GoldValue, i.DeductionValue, i.GstValue));
                return new SaleDto(s.Id, s.CustomerId, s.CustomerName, s.IsInterState, items, s.GoldValue, s.MakingCharges, s.Deduction, s.Subtotal, s.GstPercent, s.Cgst, s.Sgst, s.Igst, s.TotalGstAmount, s.GrandTotal, s.TotalAmount, s.PaymentMethod, s.PaymentStatus, s.CreatedAt);
            });
        }
    }
}

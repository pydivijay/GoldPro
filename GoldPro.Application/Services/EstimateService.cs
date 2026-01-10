using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class EstimateService : IEstimateService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public EstimateService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<EstimateDto> CreateAsync(CreateEstimateDto dto)
        {
            var estimate = new Estimate
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                CustomerId = dto.CustomerId,
                IsInterState = dto.IsInterState,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow
            };

            decimal totalGoldValue = 0;
            decimal totalMaking = 0;
            decimal totalDeduction = 0;

            foreach (var it in dto.Items)
            {
                var item = new EstimateItem
                {
                    Id = Guid.NewGuid(),
                    EstimateId = estimate.Id,
                    Description = it.Description,
                    WeightGrams = it.WeightGrams,
                    RatePerGram = it.RatePerGram,
                    MakingCharges = it.MakingCharges,
                    WastagePercent = it.WastagePercent,
                    Purity = it.Purity
                };

                var gross = item.WeightGrams * item.RatePerGram;
                var deduction = gross * (item.WastagePercent / 100m);
                var goldValue = gross + deduction;

                item.GoldValue = goldValue;
                item.DeductionValue = deduction;

                totalGoldValue += goldValue;
                totalMaking += item.MakingCharges;
                totalDeduction += deduction;

                estimate.Items.Add(item);
            }

            estimate.GoldValue = totalGoldValue;
            estimate.MakingCharges = totalMaking;
            estimate.Deduction = totalDeduction;
            estimate.Subtotal = estimate.GoldValue + estimate.MakingCharges + estimate.Deduction;

            estimate.GstPercent = 3m;
            if (estimate.IsInterState)
            {
                estimate.Igst = Math.Round(estimate.Subtotal * (estimate.GstPercent / 100m), 2);
                estimate.Cgst = 0;
                estimate.Sgst = 0;
                estimate.TotalGstAmount = estimate.Igst;
            }
            else
            {
                estimate.Igst = 0;
                estimate.Cgst = Math.Round(estimate.Subtotal * (estimate.GstPercent / 2 / 100m), 2);
                estimate.Sgst = Math.Round(estimate.Subtotal * (estimate.GstPercent / 2 / 100m), 2);
                estimate.TotalGstAmount = estimate.Cgst + estimate.Sgst;
            }

            estimate.EstimatedTotal = estimate.Subtotal + estimate.TotalGstAmount;

            // per-item GST allocation
            if (estimate.Items.Any() && estimate.TotalGstAmount > 0)
            {
                foreach (var item in estimate.Items)
                {
                    var prop = item.GoldValue / estimate.GoldValue;
                    item.GstValue = Math.Round(estimate.TotalGstAmount * prop, 2);
                }
            }

            _db.Estimates.Add(estimate);
            await _db.SaveChangesAsync();

            return await GetAsync(estimate.Id) as EstimateDto;
        }

        public async Task<EstimateDto?> GetAsync(Guid id)
        {
            var e = await _db.Estimates.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return null;

            var items = e.Items.Select(i => new EstimateItemDto(i.Id, i.Description, i.WeightGrams, i.RatePerGram, i.MakingCharges, i.WastagePercent, i.Purity, i.GoldValue, i.DeductionValue, i.GstValue));

            return new EstimateDto(e.Id, e.CustomerId, e.CustomerName, e.IsInterState, items, e.GoldValue, e.MakingCharges, e.Deduction, e.Subtotal, e.GstPercent, e.Cgst, e.Sgst, e.Igst, e.TotalGstAmount, e.EstimatedTotal, e.CreatedAt);
        }

        public async Task<IEnumerable<EstimateDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            var query = _db.Estimates.Include(x => x.Items).OrderByDescending(x => x.CreatedAt).AsQueryable();
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return list.Select(e =>
            {
                var items = e.Items.Select(i => new EstimateItemDto(i.Id, i.Description, i.WeightGrams, i.RatePerGram, i.MakingCharges, i.WastagePercent, i.Purity, i.GoldValue, i.DeductionValue, i.GstValue));
                return new EstimateDto(e.Id, e.CustomerId, e.CustomerName, e.IsInterState, items, e.GoldValue, e.MakingCharges, e.Deduction, e.Subtotal, e.GstPercent, e.Cgst, e.Sgst, e.Igst, e.TotalGstAmount, e.EstimatedTotal, e.CreatedAt);
            });
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var e = await _db.Estimates.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId);
            if (e == null) return false;

            _db.Estimates.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

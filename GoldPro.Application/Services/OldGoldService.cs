using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class OldGoldService : IOldGoldService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public OldGoldService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<OldGoldSlipDto> CreateAsync(CreateOldGoldSlipDto dto)
        {
            var slip = new OldGoldSlip
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                CustomerId = dto.CustomerId,
                Notes = dto.Notes,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow
            };

            decimal totalGross = 0;
            decimal totalDeduction = 0;

            foreach (var it in dto.Items)
            {
                var item = new OldGoldItem
                {
                    Id = Guid.NewGuid(),
                    SlipId = slip.Id,
                    Purity = it.Purity,
                    WeightGrams = it.WeightGrams,
                    RatePerGram = it.RatePerGram,
                    DeductionPercent = it.DeductionPercent,
                    Description = it.Description
                };

                var gross = item.WeightGrams * item.RatePerGram;
                var deduction = Math.Round(gross * (item.DeductionPercent / 100m), 2);
                var net = gross - deduction;

                item.GrossValue = gross;
                item.DeductionValue = deduction;
                item.NetValue = net;

                totalGross += gross;
                totalDeduction += deduction;

                slip.Items.Add(item);
            }

            slip.GoldValue = totalGross;
            slip.DeductionPercent = slip.Items.Any() ? slip.Items.First().DeductionPercent : 0;
            slip.DeductionValue = totalDeduction;
            slip.NetPayable = totalGross - totalDeduction;

            _db.OldGoldSlips.Add(slip);
            await _db.SaveChangesAsync();

            return await GetAsync(slip.Id) as OldGoldSlipDto;
        }

        public async Task<OldGoldSlipDto?> GetAsync(Guid id)
        {
            var s = await _db.OldGoldSlips.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return null;

            var items = s.Items.Select(i => new OldGoldItemDto(i.Id, i.Purity, i.WeightGrams, i.RatePerGram, i.DeductionPercent, i.Description, i.GrossValue, i.DeductionValue, i.NetValue));

            return new OldGoldSlipDto(s.Id, s.CustomerId, s.CustomerName, items, s.GoldValue, s.DeductionPercent, s.DeductionValue, s.NetPayable, s.CreatedAt);
        }

        public async Task<IEnumerable<OldGoldSlipDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            var query = _db.OldGoldSlips.Include(x => x.Items).OrderByDescending(x => x.CreatedAt).AsQueryable();
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return list.Select(s =>
            {
                var items = s.Items.Select(i => new OldGoldItemDto(i.Id, i.Purity, i.WeightGrams, i.RatePerGram, i.DeductionPercent, i.Description, i.GrossValue, i.DeductionValue, i.NetValue));
                return new OldGoldSlipDto(s.Id, s.CustomerId, s.CustomerName, items, s.GoldValue, s.DeductionPercent, s.DeductionValue, s.NetPayable, s.CreatedAt);
            });
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var s = await _db.OldGoldSlips.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId);
            if (s == null) return false;

            _db.OldGoldSlips.Remove(s);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

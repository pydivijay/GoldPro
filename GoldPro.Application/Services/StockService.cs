using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public StockService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<IEnumerable<StockItemDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            var items = await _db.StockItems.Where(o => o.TenantId == _tenant.TenantId).OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return items.Select(i => new StockItemDto(i.Id, i.Name, i.Type, i.Purity, i.WeightGrams, i.Quantity, i.HsnCode, i.CreatedAt));
        }

        public async Task<StockItemDto?> GetAsync(Guid id)
        {
            var i = await _db.StockItems.FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return null;
            return new StockItemDto(i.Id, i.Name, i.Type, i.Purity, i.WeightGrams, i.Quantity, i.HsnCode, i.CreatedAt);
        }

        public async Task<StockItemDto> CreateAsync(CreateStockItemDto dto)
        {
            var item = new StockItem
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                Name = dto.Name,
                Type = dto.Type,
                Purity = dto.Purity,
                WeightGrams = dto.WeightGrams,
                Quantity = dto.Quantity,
                HsnCode = dto.HsnCode
            };

            _db.StockItems.Add(item);
            await _db.SaveChangesAsync();

            return new StockItemDto(item.Id, item.Name, item.Type, item.Purity, item.WeightGrams, item.Quantity, item.HsnCode, item.CreatedAt);
        }

        public async Task UpdateAsync(Guid id, CreateStockItemDto dto)
        {
            var item = await _db.StockItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) throw new KeyNotFoundException("Stock item not found");

            item.Name = dto.Name;
            item.Type = dto.Type;
            item.Purity = dto.Purity;
            item.WeightGrams = dto.WeightGrams;
            item.Quantity = dto.Quantity;
            item.HsnCode = dto.HsnCode;

            _db.StockItems.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _db.StockItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return;
            _db.StockItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}

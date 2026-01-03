using GoldPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<StockItemDto>> ListAsync(int page = 1, int pageSize = 20);
        Task<StockItemDto?> GetAsync(Guid id);
        Task<StockItemDto> CreateAsync(CreateStockItemDto dto);
        Task UpdateAsync(Guid id, CreateStockItemDto dto);
        Task DeleteAsync(Guid id);
    }
}

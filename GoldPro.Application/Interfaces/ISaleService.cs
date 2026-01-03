using GoldPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface ISaleService
    {
        Task<SaleDto> CreateAsync(CreateSaleDto dto);
        Task<SaleDto?> GetAsync(Guid id);
        Task<IEnumerable<SaleDto>> ListAsync(int page = 1, int pageSize = 20);
        Task UpdateAsync(Guid id, UpdateSaleDto dto);
        Task DeleteAsync(Guid id);
    }
}

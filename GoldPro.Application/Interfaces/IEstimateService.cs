using GoldPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IEstimateService
    {
        Task<EstimateDto> CreateAsync(CreateEstimateDto dto);
        Task<EstimateDto?> GetAsync(Guid id);
        Task<IEnumerable<EstimateDto>> ListAsync(int page = 1, int pageSize = 20);
        Task<bool> DeleteAsync(Guid id);
    }
}

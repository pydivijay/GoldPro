using GoldPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IOldGoldService
    {
        Task<OldGoldSlipDto> CreateAsync(CreateOldGoldSlipDto dto);
        Task<OldGoldSlipDto?> GetAsync(Guid id);
        Task<IEnumerable<OldGoldSlipDto>> ListAsync(int page = 1, int pageSize = 20);
    }
}

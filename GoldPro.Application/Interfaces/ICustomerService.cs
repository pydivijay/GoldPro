using GoldPro.Application.DTOs;
using GoldPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> ListAsync(string? search = null, int page = 1, int pageSize = 20);
        Task<CustomerDto?> GetAsync(Guid id);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
        Task UpdateAsync(Guid id, CreateCustomerDto dto);
        Task DeleteAsync(Guid id);
    }
}

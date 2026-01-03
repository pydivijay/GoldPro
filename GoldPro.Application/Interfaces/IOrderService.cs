using GoldPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto?> GetAsync(Guid id);
        Task<IEnumerable<OrderDto>> ListAsync(int page = 1, int pageSize = 20);
    }
}

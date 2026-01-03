using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public CustomerService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<IEnumerable<CustomerDto>> ListAsync(string? search = null, int page = 1, int pageSize = 20)
        {
            var query = _db.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.FullName.Contains(search) || c.PhoneNumber.Contains(search) || (c.Email != null && c.Email.Contains(search)));
            }

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto(c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.Gstin, c.CreatedAt))
                .ToListAsync();

            return items;
        }

        public async Task<CustomerDto?> GetAsync(Guid id)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;
            return new CustomerDto(c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.Gstin, c.CreatedAt);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var c = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                Gstin = dto.Gstin,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow
            };

            _db.Customers.Add(c);
            await _db.SaveChangesAsync();

            return new CustomerDto(c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.Gstin, c.CreatedAt);
        }

        public async Task UpdateAsync(Guid id, CreateCustomerDto dto)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) throw new KeyNotFoundException("Customer not found");

            c.FullName = dto.FullName;
            c.PhoneNumber = dto.PhoneNumber;
            c.Email = dto.Email;
            c.Address = dto.Address;
            c.Gstin = dto.Gstin;
            // do not update CreatedAt

            _db.Customers.Update(c);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return;
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync();
        }
    }
}

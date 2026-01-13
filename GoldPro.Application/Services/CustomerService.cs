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

        public async Task<IEnumerable<CustomerDto>> ListAsync(
      string? search = null,
      int page = 1,
      int pageSize = 20)
        {
            var query = _db.Customers
                .Where(c => c.TenantId == _tenant.TenantId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.PhoneNumber.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)));
            }

            var items = await (
                from c in query
                join s in _db.Sales.Where(s => s.TenantId == _tenant.TenantId)
                    on c.Id equals s.CustomerId into salesGroup
                select new
                {
                    Customer = c,
                    TotalPurchase = salesGroup.Sum(s => (decimal?)s.GrandTotal) ?? 0
                }
            )
            .OrderByDescending(x => x.Customer.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerDto(
                x.Customer.Id,
                x.Customer.FullName,
                x.Customer.PhoneNumber,
                x.Customer.Email,
                x.Customer.Address,
                x.Customer.Gstin,
                x.Customer.CreatedAt,
                x.TotalPurchase
            ))
            .ToListAsync();

            return items;
        }



        public async Task<CustomerDto?> GetAsync(Guid id)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;
            return new CustomerDto(c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.Gstin, c.CreatedAt,0);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            // normalize phone for comparison
            var phone = dto.PhoneNumber?.Trim() ?? string.Empty;

            // Check uniqueness within the current tenant
            var exists = await _db.Customers
                .AnyAsync(c => c.TenantId == _tenant.TenantId && c.PhoneNumber == phone);
            if (exists)
                throw new ArgumentException("Phone number already exists");

            var c = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                FullName = dto.FullName,
                PhoneNumber = phone,
                // Ensure non-nullable DB columns get non-null values
                Email = dto.Email ?? string.Empty,
                Address = dto.Address ?? string.Empty,
                Gstin = dto.Gstin ?? string.Empty,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow
            };

            _db.Customers.Add(c);
            await _db.SaveChangesAsync();

            return new CustomerDto(c.Id, c.FullName, c.PhoneNumber, c.Email, c.Address, c.Gstin, c.CreatedAt,0);
        }

        public async Task UpdateAsync(Guid id, CreateCustomerDto dto)
        {
            var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) throw new KeyNotFoundException("Customer not found");

            c.FullName = dto.FullName;
            c.PhoneNumber = dto.PhoneNumber;
            // Only update optional fields when provided to avoid setting DB non-nullable fields to null
            c.Email = dto.Email ?? c.Email;
            c.Address = dto.Address ?? c.Address;
            c.Gstin = dto.Gstin ?? c.Gstin;
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

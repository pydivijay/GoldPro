using GoldPro.Application.DTOs;
using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public OrderService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                CustomerId = dto.CustomerId,
                DueDate = dto.DueDate,
                Notes = dto.Notes,
                CreatedAt = dto.DateTime ?? DateTime.UtcNow,
                AdvanceReceived = dto.AdvanceReceived,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus
            };

            decimal totalGoldValue = 0;
            decimal totalMaking = 0;
            decimal totalDeduction = 0;

            foreach (var it in dto.Items)
            {
                var item = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Description = it.Description,
                    Purity = it.Purity,
                    WeightGrams = it.WeightGrams,
                    RatePerGram = it.RatePerGram,
                    MakingCharges = it.MakingCharges,
                    WastagePercent = it.WastagePercent
                };

                var gross = item.WeightGrams * item.RatePerGram;
                var deduction = gross * (item.WastagePercent / 100m);
                var goldValue = gross;

                item.GoldValue = goldValue;
                item.DeductionValue = deduction;

                totalGoldValue += goldValue;
                totalMaking += item.MakingCharges;
                totalDeduction += deduction;

                order.Items.Add(item);
            }

            order.GoldValue = totalGoldValue;
            order.MakingCharges = totalMaking;
            order.Deduction = totalDeduction;
            order.Subtotal = order.GoldValue + order.MakingCharges +order.Deduction;

            order.GstPercent = 3m;
            if (order.Items.Any())
            {
                // assuming intra-state by default
                order.Cgst = Math.Round(order.Subtotal * (order.GstPercent / 2 / 100m), 2);
                order.Sgst = Math.Round(order.Subtotal * (order.GstPercent / 2 / 100m), 2);
                order.Igst = 0;
                order.TotalGstAmount = order.Cgst + order.Sgst;
            }

            order.EstimatedTotal = order.Subtotal + order.TotalGstAmount;
            order.AmountPayable = Math.Max(0, order.EstimatedTotal - order.AdvanceReceived);

            // allocate item gst proportionally
            if (order.Items.Any() && order.TotalGstAmount > 0)
            {
                foreach (var item in order.Items)
                {
                    var prop = item.GoldValue / order.GoldValue;
                    item.GstValue = Math.Round(order.TotalGstAmount * prop, 2);
                }
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return await GetAsync(order.Id) as OrderDto;
        }

        public async Task<OrderDto?> GetAsync(Guid id)
        {
            var o = await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return null;

            var items = o.Items.Select(i => new OrderItemDto(i.Id, i.Description, i.Purity, i.WeightGrams, i.RatePerGram, i.MakingCharges, i.WastagePercent, i.GoldValue, i.DeductionValue, i.GstValue, null));

            return new OrderDto(o.Id, o.CustomerId, o.CustomerName, o.DueDate, o.Notes, items, o.GoldValue, o.MakingCharges, o.Deduction, o.Subtotal, o.GstPercent, o.Cgst, o.Sgst, o.Igst, o.TotalGstAmount, o.EstimatedTotal, o.AdvanceReceived, o.AmountPayable, o.PaymentMethod, o.PaymentStatus, o.CreatedAt);
        }

        public async Task<IEnumerable<OrderDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            // Ensure we only list orders for the current tenant
            var query = _db.Orders
                .Where(o => o.TenantId == _tenant.TenantId)
                .Include(x => x.Items)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Load customer names for orders that have a CustomerId
            var customerIds = list.Where(o => o.CustomerId.HasValue).Select(o => o.CustomerId!.Value).Distinct().ToList();
            var customerMap = new Dictionary<Guid, string>();
            if (customerIds.Any())
            {
                customerMap = await _db.Customers
                    .Where(c => customerIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.FullName);
            }

            return list.Select(o =>
            {
                var items = o.Items.Select(i => new OrderItemDto(i.Id, i.Description, i.Purity, i.WeightGrams, i.RatePerGram, i.MakingCharges, i.WastagePercent, i.GoldValue, i.DeductionValue, i.GstValue, null));

                // Prefer stored CustomerName on order, otherwise lookup from customers table
                string? customerName = o.CustomerName;
                if (string.IsNullOrWhiteSpace(customerName) && o.CustomerId.HasValue)
                {
                    if (customerMap.TryGetValue(o.CustomerId.Value, out var name))
                        customerName = name;
                }

                return new OrderDto(o.Id, o.CustomerId, customerName, o.DueDate, o.Notes, items, o.GoldValue, o.MakingCharges, o.Deduction, o.Subtotal, o.GstPercent, o.Cgst, o.Sgst, o.Igst, o.TotalGstAmount, o.EstimatedTotal, o.AdvanceReceived, o.AmountPayable, o.PaymentMethod, o.PaymentStatus, o.CreatedAt);
            });
        }

        public async Task UpdateAsync(Guid id, UpdateOrderDto dto)
        {
            var order = await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null) throw new KeyNotFoundException("Order not found");

            order.DueDate = dto.DueDate;
            order.Notes = dto.Notes;
            order.AdvanceReceived = dto.AdvanceReceived;
            order.PaymentStatus = dto.PaymentStatus;

            // Recalculate amount payable
            order.AmountPayable = Math.Max(0, order.EstimatedTotal - order.AdvanceReceived);

            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null) return;

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
    }
}

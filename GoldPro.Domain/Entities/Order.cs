using System;
using System.Collections.Generic;

namespace GoldPro.Domain.Entities
{
    public class Order : TenantEntity
    {
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }

        public decimal GoldValue { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal Deduction { get; set; }
        public decimal Subtotal { get; set; }

        public decimal GstPercent { get; set; }
        public decimal Cgst { get; set; }
        public decimal Sgst { get; set; }
        public decimal Igst { get; set; }
        public decimal TotalGstAmount { get; set; }

        public decimal EstimatedTotal { get; set; }
        public decimal AdvanceReceived { get; set; }
        public decimal AmountPayable { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public string Description { get; set; }
        public string Purity { get; set; }
        public decimal WeightGrams { get; set; }
        public decimal RatePerGram { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal WastagePercent { get; set; }

        // Computed
        public decimal GoldValue { get; set; }
        public decimal DeductionValue { get; set; }
        public decimal GstValue { get; set; }

        public Order Order { get; set; }
    }
}

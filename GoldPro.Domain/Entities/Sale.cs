using System;
using System.Collections.Generic;

namespace GoldPro.Domain.Entities
{
    public class Sale : TenantEntity
    {
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public bool IsInterState { get; set; }

        public decimal GoldValue { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal Deduction { get; set; }
        public decimal Subtotal { get; set; }

        public decimal GstPercent { get; set; }
        public decimal Cgst { get; set; }
        public decimal Sgst { get; set; }
        public decimal Igst { get; set; }

        public decimal GrandTotal { get; set; }

        // New fields to store totals explicitly
        public decimal TotalGstAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SaleItem> Items { get; set; } = new();
    }

    public class SaleItem
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public string Description { get; set; }
        public decimal WeightGrams { get; set; }
        public decimal RatePerGram { get; set; }
        public decimal WastagePercent { get; set; }
        public string Purity { get; set; }
        public decimal MakingCharges { get; set; }

        // Computed
        public decimal GoldValue { get; set; }
        public decimal DeductionValue { get; set; }
        public decimal GstValue { get; set; }

        public Sale Sale { get; set; }
    }
}

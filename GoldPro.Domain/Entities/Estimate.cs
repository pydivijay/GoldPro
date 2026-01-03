using System;
using System.Collections.Generic;

namespace GoldPro.Domain.Entities
{
    public class Estimate : TenantEntity
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
        public decimal TotalGstAmount { get; set; }

        public decimal EstimatedTotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<EstimateItem> Items { get; set; } = new();
    }

    public class EstimateItem
    {
        public Guid Id { get; set; }
        public Guid EstimateId { get; set; }

        public string Description { get; set; }
        public decimal WeightGrams { get; set; }
        public decimal RatePerGram { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal WastagePercent { get; set; }
        public string Purity { get; set; }

        // computed
        public decimal GoldValue { get; set; }
        public decimal DeductionValue { get; set; }
        public decimal GstValue { get; set; }

        public Estimate Estimate { get; set; }
    }
}

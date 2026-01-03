using System;
using System.Collections.Generic;

namespace GoldPro.Domain.Entities
{
    public class OldGoldSlip : TenantEntity
    {
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }

        public decimal GoldValue { get; set; }
        public decimal DeductionPercent { get; set; }
        public decimal DeductionValue { get; set; }
        public decimal NetPayable { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OldGoldItem> Items { get; set; } = new();
    }

    public class OldGoldItem
    {
        public Guid Id { get; set; }
        public Guid SlipId { get; set; }

        public string Purity { get; set; }
        public decimal WeightGrams { get; set; }
        public decimal RatePerGram { get; set; }
        public decimal DeductionPercent { get; set; }
        public string? Description { get; set; }

        // computed
        public decimal GrossValue { get; set; }
        public decimal DeductionValue { get; set; }
        public decimal NetValue { get; set; }

        public OldGoldSlip Slip { get; set; }
    }
}

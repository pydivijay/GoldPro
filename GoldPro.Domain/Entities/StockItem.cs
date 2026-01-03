namespace GoldPro.Domain.Entities
{
    public class StockItem : TenantEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Purity { get; set; }
        public decimal WeightGrams { get; set; } // per unit weight
        public int Quantity { get; set; }
        public string HsnCode { get; set; }

        // Optional additional fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

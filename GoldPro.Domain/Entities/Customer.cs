namespace GoldPro.Domain.Entities
{
    public class Customer : TenantEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Gstin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

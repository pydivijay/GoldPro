using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Entities
{
    public class NotificationSettings : TenantEntity
    {
        public bool EmailEnabled { get; set; }
        public bool SmsEnabled { get; set; }

        public bool NewSale { get; set; }
        public bool PaymentReminder { get; set; }
        public bool LowStock { get; set; }
        public bool MonthlyReport { get; set; }
        public bool InvoiceGenerated { get; set; }
        public bool NewCustomer { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Entities
{
    public class InvoiceSettings : TenantEntity
    {
        public string Prefix { get; set; }
        public int StartingNumber { get; set; }

        public bool ShowLogo { get; set; }
        public bool ShowQr { get; set; }

        public string FooterText { get; set; }
        public string Terms { get; set; }

        public string Currency { get; set; }
        public string DateFormat { get; set; }
    }
}

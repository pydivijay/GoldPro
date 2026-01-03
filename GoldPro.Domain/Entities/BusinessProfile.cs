using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Entities
{
    public class BusinessProfile : TenantEntity
    {
        public string BusinessName { get; set; }
        public string Gstin { get; set; }
        public string Pan { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string StateCode { get; set; }
        public string StateName { get; set; }

        public decimal Gold24KRate { get; set; }
        public decimal Gold22KRate { get; set; }
        public decimal Gold18KRate { get; set; }

        public decimal SilverRateGram { get; set; }
        public decimal SilverRateKg { get; set; }
    }

}

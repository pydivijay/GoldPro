using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Entities
{
    public abstract class TenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
    }
}

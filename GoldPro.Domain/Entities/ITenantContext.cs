using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
    }

    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }
    }

}

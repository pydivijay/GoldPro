using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Entities
{
    public class AppUser : TenantEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Staff
    }
}

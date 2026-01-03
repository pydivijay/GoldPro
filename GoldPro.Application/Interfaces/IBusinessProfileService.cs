using GoldPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IBusinessProfileService
    {
        Task<BusinessProfile> GetAsync();
        Task UpdateAsync(BusinessProfile profile);
    }
}

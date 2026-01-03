using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldPro.Application.Services
{
    public class BusinessProfileService : IBusinessProfileService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public BusinessProfileService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<BusinessProfile> GetAsync()
            => await _db.BusinessProfiles.FirstOrDefaultAsync();

        public async Task UpdateAsync(BusinessProfile profile)
        {
            profile.TenantId = _tenant.TenantId;
            _db.BusinessProfiles.Update(profile);
            await _db.SaveChangesAsync();
        }
    }
}

using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GoldPro.Application.Services
{
    public class NotificationSettingsService : INotificationSettingsService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public NotificationSettingsService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<NotificationSettings> GetAsync()
            => await _db.NotificationSettings.FirstOrDefaultAsync();

        public async Task UpdateAsync(NotificationSettings settings)
        {
            settings.TenantId = _tenant.TenantId;
            _db.NotificationSettings.Update(settings);
            await _db.SaveChangesAsync();
        }
    }
}

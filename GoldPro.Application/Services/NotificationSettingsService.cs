using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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
            => await _db.NotificationSettings.FirstOrDefaultAsync(n=>n.TenantId==_tenant.TenantId);

        public async Task UpdateAsync(NotificationSettings settings)
        {
            // Find existing settings for the current tenant
            var tenantId = _tenant.TenantId;
            var existing = await _db.NotificationSettings
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (existing == null)
            {
                // Insert new settings for tenant
                settings.TenantId = tenantId;
                if (settings.Id == Guid.Empty)
                    settings.Id = Guid.NewGuid();

                await _db.NotificationSettings.AddAsync(settings);
            }
            else
            {
                // Update existing settings fields
                existing.EmailEnabled = settings.EmailEnabled;
                existing.SmsEnabled = settings.SmsEnabled;
                existing.NewSale = settings.NewSale;
                existing.PaymentReminder = settings.PaymentReminder;
                existing.LowStock = settings.LowStock;
                existing.MonthlyReport = settings.MonthlyReport;
                existing.InvoiceGenerated = settings.InvoiceGenerated;
                existing.NewCustomer = settings.NewCustomer;

                _db.NotificationSettings.Update(existing);
            }

            await _db.SaveChangesAsync();
        }
    }
}

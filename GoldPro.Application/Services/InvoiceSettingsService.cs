using GoldPro.Application.Interfaces;
using GoldPro.Domain.Data;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GoldPro.Application.Services
{
    public class InvoiceSettingsService : IInvoiceSettingsService
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public InvoiceSettingsService(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<InvoiceSettings> GetAsync()
            => await _db.InvoiceSettings.FirstOrDefaultAsync();

        public async Task UpdateAsync(InvoiceSettings settings)
        {
            settings.TenantId = _tenant.TenantId;
            _db.InvoiceSettings.Update(settings);
            await _db.SaveChangesAsync();
        }
    }
}

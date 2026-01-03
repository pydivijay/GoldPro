using GoldPro.Domain.Entities;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IInvoiceSettingsService
    {
        Task<InvoiceSettings> GetAsync();
        Task UpdateAsync(InvoiceSettings settings);
    }
}

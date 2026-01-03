using GoldPro.Domain.Entities;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface INotificationSettingsService
    {
        Task<NotificationSettings> GetAsync();
        Task UpdateAsync(NotificationSettings settings);
    }
}

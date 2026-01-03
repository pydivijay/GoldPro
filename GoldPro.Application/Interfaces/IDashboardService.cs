using GoldPro.Application.DTOs;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetSummaryAsync();
    }
}

using GoldPro.Application.DTOs.Reports;
using System;
using System.Threading.Tasks;

namespace GoldPro.Application.Interfaces
{
    public interface IReportService
    {
        Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
        Task<GstSummaryDto> GetGstSummaryAsync(DateTime from, DateTime to);
        Task<InventoryReportDto> GetInventoryReportAsync(DateTime from, DateTime to);
        Task<CustomerReportDto> GetCustomerReportAsync(DateTime from, DateTime to);
    }
}

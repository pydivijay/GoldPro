using GoldPro.Application.DTOs.Reports;
using GoldPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoldPro.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> Sales([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var dto = await _service.GetSalesReportAsync(dateFrom, dateTo);
            return Ok(dto);
        }

        [HttpGet("gst-summary")]
        public async Task<IActionResult> Gst([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var dto = await _service.GetGstSummaryAsync(dateFrom, dateTo);
            return Ok(dto);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> Inventory([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var dto = await _service.GetInventoryReportAsync(dateFrom, dateTo);
            return Ok(dto);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> Customers([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var dto = await _service.GetCustomerReportAsync(dateFrom, dateTo);
            return Ok(dto);
        }

        [HttpGet("export")]
        public IActionResult Export([FromQuery] string type, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo, [FromQuery] string format = "pdf")
        {
            // Placeholder - implement PDF/CSV generation using a library such as DinkToPdf or CsvHelper
            return BadRequest(new { message = "Export not implemented" });
        }
    }
}

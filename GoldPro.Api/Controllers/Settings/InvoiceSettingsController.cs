using GoldPro.Application.Interfaces;
using GoldPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldPro.Api.Controllers.Settings
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/settings/invoice")]
    public class InvoiceSettingsController : ControllerBase
    {
        private readonly IInvoiceSettingsService _service;

        public InvoiceSettingsController(IInvoiceSettingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _service.GetAsync());

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InvoiceSettings settings)
        {
            await _service.UpdateAsync(settings);
            return Ok();
        }
    }
}

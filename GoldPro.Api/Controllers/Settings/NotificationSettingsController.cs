using GoldPro.Application.Interfaces;
using GoldPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldPro.Api.Controllers.Settings
{
    [Authorize]
    [ApiController]
    [Route("api/settings/notifications")]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly INotificationSettingsService _service;

        public NotificationSettingsController(INotificationSettingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _service.GetAsync());

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificationSettings settings)
        {
            await _service.UpdateAsync(settings);
            return Ok();
        }
    }
}

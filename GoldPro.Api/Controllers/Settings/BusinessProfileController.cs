using GoldPro.Application.Interfaces;
using GoldPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldPro.Api.Controllers.Settings
{
    [Authorize]
    [ApiController]
    [Route("api/settings/business")]
    public class BusinessProfileController : ControllerBase
    {
        private readonly IBusinessProfileService _service;

        public BusinessProfileController(IBusinessProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _service.GetAsync());

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] BusinessProfile profile)
        {
            await _service.UpdateAsync(profile);
            return Ok();
        }
    }
}

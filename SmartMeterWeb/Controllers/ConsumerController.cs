using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        public readonly IConsumerService _ConsumerService;

        public ConsumerController(IConsumerService consumerService)
        {
            _ConsumerService = consumerService;
        }

        [Authorize(Roles ="User")]
        [HttpGet("{Name}")]
        public async Task<ActionResult> GetConsumerByName(string Name)
        {
            var response = await _ConsumerService.GetConsumerDetailsAsync(Name);
            return Ok(response);
        }

        [Authorize(Roles = "Consumer")]
        [HttpPost("Photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadConsumerPhotoAsync([FromForm] PhotoDto dto)
        {
            return await _ConsumerService.UploadConsumerPhotoAsync(dto.ConsumerName, dto.File);
        }

    }
}

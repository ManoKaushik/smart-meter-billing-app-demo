using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerCareController : BaseController
    {
        private readonly ICustomerCareService _customerCareService;

        public CustomerCareController(ICustomerCareService customerCareService)
        {
            _customerCareService = customerCareService;
        }

        [AllowAnonymous]
        [HttpPost("send")]
        public async Task<IActionResult> AddMessageAsync([FromBody] CustomerCareDto dto)
        {
            await _customerCareService.AddMessageAsync(dto);
            return Success<object>(null, "Your message has been received. We’ll respond within 3 days.");
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessagesAsync()
        {
            var messages = await _customerCareService.GetAllMessagesAsync();

            if (messages == null || !messages.Any())
                return Error("No messages found", 404);

            return Success(messages, "Messages fetched successfully.");
        }

        [AllowAnonymous]
        [HttpPost("reply")]
        public async Task<IActionResult> SendReplyToCustomer([FromBody] CustomerReplyDto dto)
        {
            await _customerCareService.SendReplyToCustomer(dto);
            return Success<object>(null, "Reply sent to the customer successfully.");
        }
    }
}

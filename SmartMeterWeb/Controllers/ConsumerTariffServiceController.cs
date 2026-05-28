using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Common; 

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ConsumerTariffServiceController : BaseController
    {
        private readonly IConsumerTariffService _consumerTariffService;

        public ConsumerTariffServiceController(IConsumerTariffService consumerTariffService)
        {
            _consumerTariffService = consumerTariffService;
        }

        
        [HttpGet("consumer/{consumerId}")]
        public async Task<IActionResult> GetConsumerTariffDetails(long consumerId)
        {
            var data = await _consumerTariffService.GetConsumerTariffDetailsAsync(consumerId);

            if (data == null)
                return Error("Consumer not found or inactive", 404);

            return Success(data, "Consumer tariff details fetched successfully");
        }
    }
}

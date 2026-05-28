using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.UserTarrif;

namespace SmartMeterWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TariffController : BaseController
    {
        private readonly ITariffService _tariffService;

        public TariffController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [AllowAnonymous]
        [HttpPut("{tariffId}")]
        public async Task<IActionResult> UpdateTariff(int tariffId, [FromBody] UpdateTariffDto dto)
        {
            var result = await _tariffService.UpdateTariffAsync(tariffId, dto);
            return result
                ? Success<object>(null, "Tariff updated successfully")
                : Error("Tariff not found", 404);
        }

        [AllowAnonymous]
        [HttpPut("todrule/{todRuleId}")]
        public async Task<IActionResult> UpdateTodRule(int todRuleId, [FromBody] UpdateTodRuleDto dto)
        {
            var result = await _tariffService.UpdateTodRuleAsync(todRuleId, dto);
            return result
                ? Success<object>(null, "TOD Rule updated successfully")
                : Error("TOD Rule not found", 404);
        }

        [AllowAnonymous]
        [HttpPut("slab/{tariffSlabId}")]
        public async Task<IActionResult> UpdateTariffSlab(int tariffSlabId, [FromBody] UpdateTariffSlabDto dto)
        {
            var result = await _tariffService.UpdateTariffSlabAsync(tariffSlabId, dto);
            return result
                ? Success<object>(null, "Tariff Slab updated successfully")
                : Error("Tariff Slab not found", 404);
        }
    }
}

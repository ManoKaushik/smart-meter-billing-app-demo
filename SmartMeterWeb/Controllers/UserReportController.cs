using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Common;
using SmartMeterWeb.Models.Reports;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserReportController : BaseController  
    {
        private readonly IUserReportService _userReportService;

        public UserReportController(IUserReportService userReportService)
        {
            _userReportService = userReportService;
        }

        [AllowAnonymous]
        [HttpGet("monthly-consumption")]
        public async Task<IActionResult> GetDailyConsumption(
            [FromQuery] DateTime date, [FromQuery] int? orgUnitId = null)
        {
            var request = new HistoricalConsumptionRequestDto
            {
                Date = date,
                OrgUnitId = orgUnitId
            };

            var result = await _userReportService.GetHistoricalConsumptionAsync(request);

            if (result == null || !result.Any())
                return Error("No consumption data found for the provided date and OrgUnit.", 404);

            return Success(result, "Daily consumption data retrieved successfully.");
        }
    }
}

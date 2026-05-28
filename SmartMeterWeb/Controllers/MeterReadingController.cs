using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Billing;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Consumer")]
    public class MeterReadingController : BaseController
    {
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpPost("record")]
        public async Task<IActionResult> RecordReading([FromBody] MeterReadingDto dto)
        {
            try
            {
                var reading = await _meterReadingService.RecordReadingAsync(dto);
                if (reading == null)
                {
                    return Error("Failed to record meter reading.",404);
                }
                return Success(reading, "Meter reading recorded successfully.");
            }

            catch (Exception ex)
            {
                return Error($"An error occurred while recording the meter reading: {ex.Message}", 500);
            }

        }

        [HttpGet("{meterId}")]
        public async Task<IActionResult> GetReadings(string meterId)
        {
            try
            {

                var readings = await _meterReadingService.GetReadingsByMeterAsync(meterId);
                if (readings == null || !readings.Any())
                {
                    return Error("No readings found for the specified meter.", 404);
                }
                return Success(readings, "Meter readings fetched successfully.");
            }
            catch (Exception ex)
            {
                return Error($"An error occurred while fetching meter readings: {ex.Message}", 500);
            }
        }
    }
}

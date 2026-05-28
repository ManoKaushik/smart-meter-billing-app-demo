using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Services;
using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Models;
using System.Collections.Generic;
namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController:BaseController
    {
        private readonly IMonthlyTariffReportService _reportService;

        public ReportController(IMonthlyTariffReportService reportService)
        {
            _reportService = reportService;
        }
        [AllowAnonymous]
        [HttpGet("monthly-tariff-report")]
        public async Task<IActionResult> GetMonthlyTariffReport([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var result = await _reportService.GetMonthlyTariffReportAsync(year, month);

                if (result == null || !result.Any())
                    return Error("No data found for the given month and year", 404);

                return Success(result, "Monthly tariff report generated successfully.");
            }
            catch (ArgumentException ex)
            {
                // For example, if invalid year/month is passed
                return Error($"Invalid input: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                // Generic fallback for unexpected errors
                return Error($"An error occurred while generating the report: {ex.Message}", 500);
            }
        }



    }

}

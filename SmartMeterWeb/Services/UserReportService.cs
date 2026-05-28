// SmartMeterWeb/Services/UserReportService.cs
using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Models.Reports;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Services
{
    public class UserReportService : IUserReportService
    {
        private readonly AppDbContext _context;

        public UserReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistoricalConsumptionDto>> GetHistoricalConsumptionAsync(
            HistoricalConsumptionRequestDto request)
        {
            // Validate date
            if (request.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Date cannot be in the future");

            // Calculate date range for the specific day
            var startDate = request.Date.Date;
            var endDate = startDate.AddDays(1).AddSeconds(-1);

            // Build base query for the specific date
            var baseQuery = from mr in _context.MeterReadings
                            join meter in _context.Meters on mr.MeterId equals meter.MeterSerialNo
                            join consumer in _context.Consumers on meter.ConsumerId equals consumer.ConsumerId
                            join orgUnit in _context.OrgUnits on consumer.OrgUnitId equals orgUnit.OrgUnitId
                            where mr.ReadingDateTime >= startDate &&
                                  mr.ReadingDateTime <= endDate &&
                                  consumer.Status == "Active" &&
                                  meter.Status == "Active"
                            select new
                            {
                                mr.ReadingDateTime,
                                mr.EnergyConsumed,
                                orgUnit.OrgUnitId,
                                orgUnit.Name,
                                orgUnit.Type,
                                consumer.ConsumerId
                            };

            // Apply OrgUnit filter if specified
            if (request.OrgUnitId.HasValue)
            {
                var childOrgUnits = await GetChildOrgUnitsAsync(request.OrgUnitId.Value);
                baseQuery = baseQuery.Where(x => childOrgUnits.Contains(x.OrgUnitId));
            }

            // Group by OrgUnit for the single date
            var result = await baseQuery
                .GroupBy(x => new { x.OrgUnitId, x.Name, x.Type })
                .Select(g => new HistoricalConsumptionDto
                {
                    Date = request.Date,
                    OrgUnitName = g.Key.Name,
                    OrgUnitType = g.Key.Type,
                    TotalEnergyConsumed = g.Sum(x => (double)x.EnergyConsumed),
                    ConsumerCount = g.Select(x => x.ConsumerId).Distinct().Count(),
                    AverageConsumption = g.Average(x => (double)x.EnergyConsumed),
                    PeakConsumption = g.Max(x => (double)x.EnergyConsumed),
                    LowConsumption = g.Min(x => (double)x.EnergyConsumed)
                })
                .OrderBy(x => x.OrgUnitName)
                .ToListAsync();

            return result;
        }

        private async Task<List<int>> GetChildOrgUnitsAsync(int parentOrgUnitId)
        {
            var query = @"
                WITH RECURSIVE OrgUnitTree AS (
                    SELECT OrgUnitId, ParentId
                    FROM OrgUnit
                    WHERE OrgUnitId = {0}
                    UNION ALL
                    SELECT ou.OrgUnitId, ou.ParentId
                    FROM OrgUnit ou
                    INNER JOIN OrgUnitTree out ON ou.ParentId = out.OrgUnitId
                )
                SELECT OrgUnitId FROM OrgUnitTree";

            var orgUnitIds = await _context.OrgUnits
                .FromSqlRaw(query, parentOrgUnitId)
                .Select(ou => ou.OrgUnitId)
                .ToListAsync();

            return orgUnitIds;
        }
    }
}
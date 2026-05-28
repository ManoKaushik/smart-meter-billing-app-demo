using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Reports;

namespace SmartMeterWeb.Services
{
    public class MonthlyTariffReportService : IMonthlyTariffReportService
    {

        private readonly AppDbContext _context;

        public MonthlyTariffReportService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<MonthlyTariffReportDto>> GetMonthlyTariffReportAsync(int year, int month)


        {
            var fromDate = new DateOnly(year, month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);

            var result = await (from b in _context.Billings
                                join c in _context.Consumers on b.ConsumerId equals c.ConsumerId
                                join t in _context.Tariffs on c.TariffId equals t.TariffId
                                where b.BillingPeriodStart >= fromDate
                                         && b.BillingPeriodStart < toDate.AddDays(1)


                                group new { b, c, t } by t.Name into g
                                select new MonthlyTariffReportDto
                                {
                                    TariffName = g.Key,
                                    TotalConsumers = g.Select(x => x.c.ConsumerId).Distinct().Count(),
                                    TotalUnits = g.Sum(x => x.b.TotalUnitsConsumed),
                                    BaseRevenue = g.Sum(x => x.b.BaseAmount),
                                    TaxCollected = g.Sum(x => x.b.TaxAmount),
                                    TotalRevenue = g.Sum(x => x.b.BaseAmount + x.b.TaxAmount),
                                    AvgPerConsumer = g.Average(x => x.b.BaseAmount + x.b.TaxAmount),
                                    OverdueBills = g.Count(x => x.b.PaymentStatus == "Overdue")
                                }).ToListAsync();

            return result;
        }
    }
}

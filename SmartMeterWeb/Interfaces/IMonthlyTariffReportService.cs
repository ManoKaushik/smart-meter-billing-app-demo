using SmartMeterWeb.Models.Reports;
using SmartMeterWeb.Models.Tariffs;

namespace SmartMeterWeb.Interfaces
{
    public interface IMonthlyTariffReportService
    {
        //it generates monthy tarrif reports by taking year and month as input from the user 
        Task<List<MonthlyTariffReportDto>> GetMonthlyTariffReportAsync(int year, int month);
    }
}

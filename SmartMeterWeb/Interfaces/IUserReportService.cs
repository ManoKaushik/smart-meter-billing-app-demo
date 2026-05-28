using SmartMeterWeb.Models.Reports;

namespace SmartMeterWeb.Interfaces
{
    public interface IUserReportService
    {

        Task<List<HistoricalConsumptionDto>> GetHistoricalConsumptionAsync(HistoricalConsumptionRequestDto request);
    }
}


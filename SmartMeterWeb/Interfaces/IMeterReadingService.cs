using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Models.Billing;

namespace SmartMeterWeb.Interfaces
{
    public interface IMeterReadingService
    {
        Task<MeterReading> RecordReadingAsync(MeterReadingDto dto);
        Task<IEnumerable<MeterReading>> GetReadingsByMeterAsync(string meterId);
    }
}

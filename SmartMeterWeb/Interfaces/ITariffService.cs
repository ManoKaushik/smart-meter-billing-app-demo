using SmartMeterWeb.Models;

using SmartMeterWeb.Models.UserTarrif;

namespace SmartMeterWeb.Interfaces
{
    public interface ITariffService
    {
        Task<bool> UpdateTariffAsync(int tariffId, UpdateTariffDto dto);
        Task<bool> UpdateTodRuleAsync(int todRuleId, UpdateTodRuleDto dto);
        Task<bool> UpdateTariffSlabAsync(int tariffSlabId, UpdateTariffSlabDto dto);
        
    }
}

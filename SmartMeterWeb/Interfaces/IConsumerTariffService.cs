using SmartMeterWeb.Models.Tariffs;
using System.Threading.Tasks;

namespace SmartMeterWeb.Interfaces
{
    public interface IConsumerTariffService
    {
        //to the tariff info (with slabs & TOD rules) for a given consumer.

        Task<TariffInfoDto?> GetConsumerTariffDetailsAsync(long consumerId);

    }
}

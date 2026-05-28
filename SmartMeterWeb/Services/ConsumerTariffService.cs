using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Tariffs;

namespace SmartMeterWeb.Services
{
    public class ConsumerTariffService : IConsumerTariffService
    {
        private readonly AppDbContext _context;

        public ConsumerTariffService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches tariff details (base rate, slabs, TOD rules) for a given consumer.
        /// </summary>
        public async Task<TariffInfoDto> GetConsumerTariffDetailsAsync(long consumerId)
        {
            if (consumerId <= 0)
                throw new ApiException("Invalid Consumer ID provided.", 400);

            try
            {
                // Step 1: Get Consumer’s TariffId
                var consumerTariff = await _context.Consumers
                    .Where(c => c.ConsumerId == consumerId && !c.IsDeleted)
                    .Select(c => new { c.TariffId })
                    .FirstOrDefaultAsync();

                if (consumerTariff == null)
                    throw new ApiException("Consumer not found or inactive.", 404);

                var tariffId = consumerTariff.TariffId;

                // Step 2: Fetch Tariff Basic Info
                var tariffInfo = await _context.Tariffs
                    .Where(t => t.TariffId == tariffId)
                    .Select(t => new TariffInfoDto
                    {
                        TariffName = t.Name,
                        BaseRate = t.BaseRate,
                        TaxRate = t.TaxRate
                    })
                    .FirstOrDefaultAsync();

                if (tariffInfo == null)
                    throw new ApiException("No tariff information found for this consumer.", 404);

                // Step 3: Fetch TOD Rules
                tariffInfo.TodRules = await _context.TodRules
                    .Where(tr => tr.TariffId == tariffId && !tr.IsDeleted)
                    .OrderBy(tr => tr.StartTime)
                    .Select(tr => new TodRuleDto
                    {
                        Name = tr.Name,
                        StartTime = tr.StartTime,
                        EndTime = tr.EndTime,
                        RatePerKwh = tr.RatePerKwh
                    })
                    .ToListAsync();

                // Step 4: Fetch Tariff Slabs
                tariffInfo.TariffSlabs = await _context.TariffSlabs
                    .Where(ts => ts.TariffId == tariffId && !ts.IsDeleted)
                    .OrderBy(ts => ts.FromKwh)
                    .Select(ts => new TariffSlabDto
                    {
                        FromKwh = ts.FromKwh,
                        ToKwh = ts.ToKwh,
                        RatePerKwh = ts.RatePerKwh
                    })
                    .ToListAsync();

                return tariffInfo;
            }
            catch (ApiException)
            {
                // Known application errors — just rethrow
                throw;
            }
            catch (Exception ex)
            {
                // Unhandled errors (e.g., DB issues)
                throw new ApiException($"An unexpected error occurred while fetching tariff details: {ex.Message}", 500);
            }
        }
    }
}

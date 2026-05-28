using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.UserTarrif;

namespace SmartMeterWeb.Services
{
    public class TariffService : ITariffService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TariffService> _logger;

        public TariffService(AppDbContext context, ILogger<TariffService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //  Update Tariff
        public async Task<bool> UpdateTariffAsync(int tariffId, UpdateTariffDto dto)
        {
            try
            {
                var tariff = await _context.Tariffs.FirstOrDefaultAsync(t => t.TariffId == tariffId);
                if (tariff == null)
                    throw new ApiException($"Tariff with ID {tariffId} not found", 404);

                tariff.Name = dto.Name;
                tariff.BaseRate = dto.BaseRate;
                tariff.TaxRate = dto.TaxRate;
                tariff.EffectiveFrom = dto.EffectiveFrom;
                tariff.EffectiveTo = dto.EffectiveTo;

                await _context.SaveChangesAsync();
                _logger.LogInformation(" Tariff updated successfully. ID: {Id}, Name: {Name}", tariffId, tariff.Name);
                return true;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating tariff with ID {Id}", tariffId);
                throw new ApiException("Database error while updating tariff", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating tariff with ID {Id}", tariffId);
                throw new ApiException("Unexpected error occurred while updating tariff", 500);
            }
        }

        //  Update TOD Rule
        public async Task<bool> UpdateTodRuleAsync(int todRuleId, UpdateTodRuleDto dto)
        {
            try
            {
                var rule = await _context.TodRules.FirstOrDefaultAsync(t => t.TodRuleId == todRuleId && !t.IsDeleted);
                if (rule == null)
                    throw new ApiException($"TOD Rule with ID {todRuleId} not found", 404);

                rule.Name = dto.Name;
                rule.StartTime = dto.StartTime;
                rule.EndTime = dto.EndTime;
                rule.RatePerKwh = dto.RatePerKwh;

                await _context.SaveChangesAsync();
                _logger.LogInformation(" TOD Rule updated successfully. ID: {Id}, Name: {Name}", todRuleId, rule.Name);
                return true;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating TOD Rule {Id}", todRuleId);
                throw new ApiException("Database error while updating TOD Rule", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating TOD Rule {Id}", todRuleId);
                throw new ApiException("Unexpected error occurred while updating TOD Rule", 500);
            }
        }

        //  Update Tariff Slab
        public async Task<bool> UpdateTariffSlabAsync(int tariffSlabId, UpdateTariffSlabDto dto)
        {
            try
            {
                var slab = await _context.TariffSlabs.FirstOrDefaultAsync(t => t.TariffSlabId == tariffSlabId && !t.IsDeleted);
                if (slab == null)
                    throw new ApiException($"Tariff Slab with ID {tariffSlabId} not found", 404);

                slab.FromKwh = dto.FromKwh;
                slab.ToKwh = dto.ToKwh;
                slab.RatePerKwh = dto.RatePerKwh;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Tariff Slab updated successfully. ID: {Id}", tariffSlabId);
                return true;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating Tariff Slab {Id}", tariffSlabId);
                throw new ApiException("Database error while updating Tariff Slab", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating Tariff Slab {Id}", tariffSlabId);
                throw new ApiException("Unexpected error occurred while updating Tariff Slab", 500);
            }
        }
    }
}

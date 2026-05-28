using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Models.Billing;

namespace SmartMeterWeb.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly AppDbContext _context;

        public MeterReadingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MeterReading> RecordReadingAsync(MeterReadingDto dto)
        {
            var meterExists = await _context.Meters.AnyAsync(m => m.MeterSerialNo == dto.MeterId);
            if (!meterExists)
                throw new Exception("Meter not found");

            var energyConsumed = CalculateEnergyConsumed(dto.Voltage, dto.Current, dto.PowerFactor, 1); // assuming 1-hour interval
            
            var reading = new MeterReading
            {
                MeterId = dto.MeterId,
                EnergyConsumed = energyConsumed,
                Voltage = dto.Voltage,
                Current = dto.Current,
                PowerFactor = dto.PowerFactor,
                ReadingDateTime = DateTime.SpecifyKind(dto.ReadingDate, DateTimeKind.Utc)
            };

            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync();

            return reading;
        }

        public async Task<IEnumerable<MeterReading>> GetReadingsByMeterAsync(string meterId)
        {
            return await _context.MeterReadings
                .Where(r => r.MeterId == meterId)
                .OrderByDescending(r => r.ReadingDateTime)
                .ToListAsync();
        }

        private double CalculateEnergyConsumed(double voltage, double current, double powerFactor, double hours = 1)
        {
            double energy = (voltage * current * powerFactor * hours) / 1000;
            return Math.Round(energy, 6);
        }
    }
}

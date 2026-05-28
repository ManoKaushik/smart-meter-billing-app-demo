using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Interfaces;
using static SmartMeterWeb.Models.Billing.BillingDto;

namespace SmartMeterWeb.Services
{
    public class BillingService : IBillingService
    {
        private readonly IMailService _mailService;
        private readonly AppDbContext _context;

        public BillingService(AppDbContext context,IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
            
        }


        public async Task<BillingResponseDto> GenerateMonthlyBillAsync(BillingRequestDto dto)
        {
            var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.ConsumerId == dto.ConsumerId);
            if (consumer == null)
                throw new ApiException("Consumer not found", 404);

            var meter = await _context.Meters.FirstOrDefaultAsync(m => m.MeterSerialNo == dto.MeterId && m.ConsumerId == dto.ConsumerId);
            if (meter == null)
                throw new ApiException("Meter not found", 404);

            var startDate = DateTime.SpecifyKind(new DateTime(dto.Year, dto.Month, 1), DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddDays(-1), DateTimeKind.Utc);

            var readings = await _context.MeterReadings
                .Where(r => r.MeterId == meter.MeterSerialNo &&
                            r.ReadingDateTime >= startDate &&
                            r.ReadingDateTime <= endDate)
                .ToListAsync();

            if (!readings.Any())
                throw new ApiException("No readings found for the given month", 404);

            var tariff = await _context.Tariffs
                .Where(t => t.TariffId == consumer.TariffId)
                .FirstOrDefaultAsync();

            if (tariff == null)
                throw new ApiException("Applicable tariff not found", 404);

            var todRules = await _context.TodRules
                .Where(t => t.TariffId == tariff.TariffId && !t.IsDeleted)
                .ToListAsync();

            var slabs = await _context.TariffSlabs
                .Where(s => s.TariffId == tariff.TariffId && !s.IsDeleted)
                .OrderBy(s => s.FromKwh)
                .ToListAsync();

            double baseAmount = 0;
            double TotalEnergy = 0;

            foreach (var reading in readings)
            {

                var readingTime = DateTime.SpecifyKind(reading.ReadingDateTime, DateTimeKind.Utc);

                
                double kwh = reading.EnergyConsumed;
                TotalEnergy += kwh;

                // TOD check
                var tod = todRules.FirstOrDefault(t => readingTime.TimeOfDay >= t.StartTime.ToTimeSpan() &&
                                                       readingTime.TimeOfDay < t.EndTime.ToTimeSpan());
                if (tod != null)
                {
                    baseAmount += Math.Round(kwh * tod.RatePerKwh, 2);
                }               
            }

            // Slab calculation
            double remainingKwh = TotalEnergy;
            foreach (var slab in slabs)
            {
                if (remainingKwh <= 0) break;

                double slabKwh = Math.Min(remainingKwh, slab.ToKwh - slab.FromKwh);
                baseAmount += Math.Round(slabKwh * slab.RatePerKwh, 2);
                remainingKwh -= slabKwh;
            }

            // Add BaseRate and tax
            baseAmount += tariff.BaseRate;

            var solarReading = await _context.SolarMeterReadings
                .Where(s => s.MeterId == meter.MeterSerialNo &&
                            s.ReadingDateTime >= startDate &&
                            s.ReadingDateTime <= endDate)
                .ToListAsync();

            double totalExportedEnergy = solarReading.Sum(s => s.EnergyExportedToGrid);
            double solarDiscount = Math.Round(totalExportedEnergy * 1.5, 2); // ₹1.5 per exported unit

            if (solarDiscount > 0)
            {
                baseAmount -= solarDiscount;
                if (baseAmount < 0) baseAmount = 0; // avoid negative bills
            }

            double taxAmount = Math.Round(baseAmount * tariff.TaxRate / 100, 2);



            var bill = new Billing
            {
                ConsumerId = dto.ConsumerId,
                MeterId = dto.MeterId,
                BillingPeriodStart = DateOnly.FromDateTime(startDate),
                BillingPeriodEnd = DateOnly.FromDateTime(endDate),
                TotalUnitsConsumed = readings.Sum(r => r.EnergyConsumed),
                BaseAmount = baseAmount,
                TaxAmount = taxAmount,
                //TotalAmount = totalAmount,
                //GeneratedAt = DateTime.UtcNow, // ✅ UTC
                DueDate = DateOnly.FromDateTime(endDate.AddDays(10))
            };

            _context.Billings.Add(bill);

            await _context.SaveChangesAsync();


            try
            {
                await _mailService.SendEmailAsync(
                    consumer.Email ?? "user.test@local.com", // fallback for testing
                    "Your Monthly Electricity Bill",
                    $@"
                        <h3>Hello {consumer.Name},</h3>
                        <p>Your monthly electricity bill for <b>{dto.Month}/{dto.Year}</b> has been generated.</p>
                        <p>
                            <b>Total Units:</b> {bill.TotalUnitsConsumed}<br/>
                            <b>Base Amount:</b> ₹{bill.BaseAmount}<br/> 
                            <b>Tax Amount:</b> ₹{bill.TaxAmount}<br/>
                            <b>Total Payable:</b> ₹{bill.TotalAmount}<br/>
                            <b>Due Date:</b> {bill.DueDate}
                        </p>
                        <p>Thank you,<br/>Smart Meter System</p>"
                );
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error sending email for bill {BillId}", bill.BillId);
                throw new ApiException("Bill generated but failed to send email notification.", 500);
            }

            return new BillingResponseDto
            {
                BillId = bill.BillId,
                ConsumerId = bill.ConsumerId,
                MeterId = bill.MeterId,
                TotalUnitsConsumed = bill.TotalUnitsConsumed,
                BaseAmount = bill.BaseAmount,
                TaxAmount = bill.TaxAmount,
                TotalAmount = bill.TotalAmount,
                BillingMonth = $"{dto.Month:D2}-{dto.Year}",
                PaymentStatus = bill.PaymentStatus
            };
        }

        public async Task<IEnumerable<BillingResponseDto>> GetPreviousBillsAsync(long consumerId)
        {
            try
            {
                var bills = await _context.Billings
                    .Where(b => b.ConsumerId == consumerId)
                    .OrderByDescending(b => b.BillingPeriodStart)
                    .Select(b => new BillingResponseDto
                    {
                        BillId = b.BillId,
                        ConsumerId = b.ConsumerId,
                        MeterId = b.MeterId,
                        TotalUnitsConsumed = b.TotalUnitsConsumed,
                        BaseAmount = b.BaseAmount,
                        TaxAmount = b.TaxAmount,
                        TotalAmount = b.TotalAmount,
                        BillingMonth = $"{b.BillingPeriodStart:MMM yyyy}",
                        PaymentStatus = b.PaymentStatus
                    })
                    .ToListAsync();

                if (!bills.Any())
                    throw new ApiException("No previous bills found for this consumer", 404);

                return bills;

            }

            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error retrieving previous bills for consumer {ConsumerId}", consumerId);
                throw new ApiException("Failed to retrieve billing history", 500);
            }

        }

        public async Task<BillingResponseDto?> GetBillByIdAsync(int billId)
        {
            try
            {
                var bill = await _context.Billings.FirstOrDefaultAsync(b => b.BillId == billId);
                if (bill == null) return null;

                return new BillingResponseDto
                {
                    BillId = bill.BillId,
                    ConsumerId = bill.ConsumerId,
                    MeterId = bill.MeterId,
                    TotalUnitsConsumed = bill.TotalUnitsConsumed,
                    BaseAmount = bill.BaseAmount,
                    TaxAmount = bill.TaxAmount,
                    TotalAmount = bill.TotalAmount,
                    BillingMonth = $"{bill.BillingPeriodStart:MM-yyyy}",
                    PaymentStatus = bill.PaymentStatus
                };

            }

            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error retrieving bill with ID {BillId}", billId);
                throw new ApiException("Failed to fetch bill details", 500);
            }
        }

        public async Task<string> PayBillAsync(long consumerId, long billId, double amount)
        {
            var bill = await _context.Billings
                .FirstOrDefaultAsync(b => b.ConsumerId == consumerId && b.BillId == billId);

            if (bill == null)
            {
                throw new ApiException("Bill not found for the given ConsumerId and BillId.", 404);
            }

            if (bill.PaymentStatus == "Paid")
            {
                return $"Bill ID {billId} is already paid.";
            }

            // Calculate the outstanding balance
            // Added a small tolerance for floating-point comparison
            double balance = Math.Round(bill.TotalAmount - bill.AmountPaid, 4);

            if (amount <= 0)
            {
                return "Payment amount must be greater than zero.";
            }

            // Check if amount exceeds the balance
            if (amount > balance)
            {
                return $"Entered amount {amount} exceeds the outstanding balance. The outstanding balance is {balance}.";
            }
            // Check if amount exactly matches the balance
            else if (Math.Abs(amount - balance) < 0.0001) // Using tolerance for double comparison
            {
                bill.AmountPaid += amount;
                bill.PaymentStatus = "Paid"; // As requested
                bill.PaidDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return "Bill payment successful.";
            }
            // Amount is less than the balance
            else
            {
                bill.AmountPaid += amount;
                bill.PaymentStatus = "Partially-Paid"; // As requested
                bill.PaidDate = DateTime.UtcNow;

                double newBalance = bill.TotalAmount - bill.AmountPaid;
                await _context.SaveChangesAsync();
                return $"Partial payment successful. Remaining amount: {Math.Round(newBalance, 2)}";
            }
        }


    }
}

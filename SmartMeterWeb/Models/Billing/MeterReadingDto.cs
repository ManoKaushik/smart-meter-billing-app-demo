using System;

namespace SmartMeterWeb.Models.Billing
{
    public class MeterReadingDto
    {
        public string MeterId { get; set; } = null!;
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double PowerFactor { get; set; }
        public DateTime ReadingDate { get; set; }
    }
}

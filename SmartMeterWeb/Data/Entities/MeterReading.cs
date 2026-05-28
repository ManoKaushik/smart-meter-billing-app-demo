using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class MeterReading
    {
        [Key]
        public int Id { get; set; }

        [Required][ForeignKey("Meter")] public string MeterId { get; set; }
        
        public Meter Meter { get; set; }

        [Required]
        public DateTime ReadingDateTime { get; set; }

        [Required]
        public double Voltage { get; set; }

        [Required]
        public double Current { get; set; }

        [Required]
        public double PowerFactor { get; set; }

        [Required]
        public double EnergyConsumed { get; set; }

    }
}

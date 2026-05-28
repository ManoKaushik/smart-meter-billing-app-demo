using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartMeterWeb.Data.Entities
{
    public class SolarMeterReading
    {
        [Key]
        public int Id { get; set; }

        [Required][ForeignKey("Meter")] public string MeterId { get; set; }

        public Meter Meter { get; set; }

        [Required]
        public DateTime ReadingDateTime { get; set; }

        [Required]
        public double EnergyGenerated { get; set; }

        [Required]
        public double EnergyExportedToGrid { get; set; }
    }
}
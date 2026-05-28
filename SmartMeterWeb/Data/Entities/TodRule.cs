
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class TodRule
    {
        [Key] public int TodRuleId { get; set; }
        [Required][ForeignKey("Tariff")] public int TariffId { get; set; }
        public Tariff Tariff { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        [Required] public TimeOnly StartTime { get; set; }
        [Required] public TimeOnly EndTime { get; set; }
        [Required] public double RatePerKwh { get; set; }
        [Required] public bool IsDeleted { get; set; } = false;
    }
}

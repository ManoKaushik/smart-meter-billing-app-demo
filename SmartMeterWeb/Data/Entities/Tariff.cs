
using System.ComponentModel.DataAnnotations;

namespace SmartMeterWeb.Data.Entities
{
    public class Tariff
    {
        [Key] public int TariffId { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Required] public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        [Required] public double BaseRate { get; set; }
        [Required] public double TaxRate { get; set; } = 0;
    }
}

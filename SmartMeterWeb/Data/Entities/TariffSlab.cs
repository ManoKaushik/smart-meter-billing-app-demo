using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class TariffSlab
    {
        [Key] public int TariffSlabId { get; set; }
        [Required][ForeignKey("Tariff")] public int TariffId { get; set; }
        public Tariff Tariff { get; set; } = null!;
        [Required][Column(TypeName = "numeric(18,6)")] public double FromKwh { get; set; }
        [Required][Column(TypeName = "numeric(18,6)")] public double ToKwh { get; set; }
        [Required][Column(TypeName = "numeric(18,6)")] public double RatePerKwh { get; set; }
        [Required] public bool IsDeleted { get; set; } = false;
    }
}

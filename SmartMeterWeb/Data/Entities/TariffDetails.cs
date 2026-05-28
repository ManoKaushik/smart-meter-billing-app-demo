
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class TariffDetails
    {
        [Key] public int TariffDetailsId { get; set; }
        [ForeignKey("TariffSlab")] public int? TariffSlabId { get; set; }
        public TariffSlab? TariffSlab { get; set; }
        [ForeignKey("TodRule")] public int? TodRuleId { get; set; }
        public TodRule? TodRule { get; set; }
    }
}

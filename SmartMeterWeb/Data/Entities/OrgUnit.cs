using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class OrgUnit
    {
        [Key] public int OrgUnitId { get; set; }
        [Required] public string Type { get; set; } = null!; // 'Zone','Substation','Feeder','DTR'
        [Required] public string Name { get; set; } = null!;
        [ForeignKey("OrgUnit")] public int? ParentId { get; set; }
        public OrgUnit? Parent { get; set; }
    }
}

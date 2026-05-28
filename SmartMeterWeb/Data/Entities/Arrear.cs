
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class Arrear
    {
        [Key] public long ArrearId { get; set; }
        [Required][ForeignKey("Consumer")] public long ConsumerId { get; set; }
        public Consumer Consumer { get; set; } = null!;
        [Required] public string ArrearType { get; set; } = null!; // 'Overdue','Penalty','Interest'
        [Required] public double Amount { get; set; }
        [Required] public string PaidStatus { get; set; } = null!; // 'Paid','Unpaid','Partially Paid'
        [ForeignKey("Billing")] public long? BillId { get; set; }
        [Required] public DateTime CreatedAt { get; set; }
    }
}

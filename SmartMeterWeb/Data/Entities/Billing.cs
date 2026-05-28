using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class Billing
    {
        [Key]
        public long BillId { get; set; }

        [Required][ForeignKey("Consumer")] public long ConsumerId { get; set; }
        public Consumer Consumer { get; set; } = null!;

        [Required]
        [ForeignKey("Meter")]
        public string MeterId { get; set; } = null!; // References Meter.MeterSerialNo

        [Required]
        public DateOnly BillingPeriodStart { get; set; }

        [Required]
        public DateOnly BillingPeriodEnd { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,6)")]
        public double TotalUnitsConsumed { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,4)")]
        public double BaseAmount { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,4)")]
        public double TaxAmount { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,4)")]
        public double TotalAmount { get; private set; } 
        public double AmountPaid { get; set; } 

        [Required]
        public DateTime GeneratedAt { get; set; } 

        [Required]
        public DateOnly DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Overdue, Cancelled

        public DateTime? DisconnectionDate { get; set; }
    }
}

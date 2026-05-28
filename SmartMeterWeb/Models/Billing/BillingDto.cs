using System.ComponentModel.DataAnnotations;

namespace SmartMeterWeb.Models.Billing
{
    public class BillingDto
    {
        public class BillingRequestDto
        {
            public long ConsumerId { get; set; }
            public string MeterId { get; set; } = null!;
            public int Year { get; set; }
            public int Month { get; set; }
        }

        public class BillingResponseDto
        {
            public long BillId { get; set; }
            public long ConsumerId { get; set; }
            public string MeterId { get; set; } = null!;
            public double TotalUnitsConsumed { get; set; }
            public double BaseAmount { get; set; }
            public double TaxAmount { get; set; }
            public double TotalAmount { get; set; }
            public string BillingMonth { get; set; } = null!;
            public string PaymentStatus { get; set; } = "Unpaid";
        }

        public class PayBillRequestDto
        {
            [Required]
            public long ConsumerId { get; set; }

            [Required]
            public long BillId { get; set; }

            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
            public double Amount { get; set; }
        }
    }
}


using static SmartMeterWeb.Models.Billing.BillingDto;

namespace SmartMeterWeb.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateBillPdf(BillingResponseDto bill);
    }
}

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartMeterWeb.Interfaces;
using static SmartMeterWeb.Models.Billing.BillingDto;

namespace SmartMeterWeb.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateBillPdf(BillingResponseDto bill)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text("SMART METER BILL")
                        .FontSize(24).Bold().AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Bill ID: {bill.BillId}");
                        col.Item().Text($"Consumer ID: {bill.ConsumerId}");
                        col.Item().Text($"Meter ID: {bill.MeterId}");
                        col.Item().Text($"Billing Month: {bill.BillingMonth}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text($"Total Units Consumed: {bill.TotalUnitsConsumed} kWh");
                        col.Item().Text($"Base Amount: ₹ {bill.BaseAmount}");
                        col.Item().Text($"Tax Amount: ₹ {bill.TaxAmount}");
                        col.Item().Text($"Total Amount: ₹ {bill.TotalAmount}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Thank you for using Smart Meter System.")
                            .AlignCenter().Italic().FontSize(12);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}

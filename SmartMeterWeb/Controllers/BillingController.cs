using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using static SmartMeterWeb.Models.Billing.BillingDto;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IPdfService _pdfService;

        public BillingController(IBillingService billingService, IPdfService pdfService)
        {
            _billingService = billingService;
            _pdfService = pdfService;
        }
        
        [HttpPost("Generate")]
        public async Task<ActionResult<BillingResponseDto>> GenerateMonthlyBill([FromBody] BillingRequestDto dto)
        {
            try
            {
                var bill = await _billingService.GenerateMonthlyBillAsync(dto);
                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("download/{billId}")]
        public async Task<IActionResult> DownloadBill(int billId)
        {
            var bill = await _billingService.GetBillByIdAsync(billId);
            if (bill == null) return NotFound("Bill not found");

            var pdfBytes = _pdfService.GenerateBillPdf(bill);

            return File(pdfBytes, "application/pdf", $"Bill_{billId}.pdf");
        }


        [AllowAnonymous]
        [HttpGet("Previous/{consumerId}")]
        public async Task<ActionResult<IEnumerable<BillingResponseDto>>> GetPreviousBills(long consumerId)
        {
            try
            {
                var bills = await _billingService.GetPreviousBillsAsync(consumerId);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

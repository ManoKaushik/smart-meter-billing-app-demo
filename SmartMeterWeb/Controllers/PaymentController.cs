using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Common;
using SmartMeterWeb.Exceptions;
using static SmartMeterWeb.Models.Billing.BillingDto;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PaymentController : BaseController
    {
        private readonly IBillingService _billingService;

        public PaymentController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        /// <summary>
        /// Makes a payment towards a bill.
        /// </summary>
        [HttpPost("pay")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> PayBill([FromBody] PayBillRequestDto request)
        {
            if (!ModelState.IsValid)
                return Error("Invalid payment request. Please check the data.", 400);

            // Any exception thrown below will automatically be caught by ErrorHandlingMiddleware
            var resultMessage = await _billingService.PayBillAsync(request.ConsumerId, request.BillId, request.Amount);

            return Success<string>(resultMessage, "Payment processed successfully");
        }
    }
}


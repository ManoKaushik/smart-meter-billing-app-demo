using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Models.Common;

namespace SmartMeterWeb.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success")
            => Ok(ApiResponse<T>.SuccessResponse(data, message));

        protected IActionResult Error(string message, int statusCode = 400)
            => StatusCode(statusCode, ApiResponse<string>.ErrorResponse(message));
    }
}

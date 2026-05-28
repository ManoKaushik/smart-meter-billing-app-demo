using FluentValidation;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Models.Common;
using System.Net;
using System.Text.Json;

namespace SmartMeterWeb.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // handle non-existing endpoints
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await HandleExceptionAsync(context, context.Response.StatusCode, "Resource Not Found");
                }
            }
            catch (ValidationException ex)
            {
                // Handle FluentValidation errors
                _logger.LogWarning("Validation failed: {Message}", ex.Message);

                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                await HandleExceptionAsync(context, (int)HttpStatusCode.BadRequest, "Validation failed", errors);
            }
            catch (ApiException ex)
            {
                // Handle known API-level exceptions
                _logger.LogWarning("API Error: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Unhandled exception");
                var message = _env.IsDevelopment() ? ex.Message : "Internal Server Error";
                await HandleExceptionAsync(context, 500, message);

            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, object? errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors,
                Time = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}

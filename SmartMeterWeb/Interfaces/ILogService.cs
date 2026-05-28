using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Models;
using SmartMeterWeb.Models.AuthDto;

namespace SmartMeterWeb.Interfaces
{
    public interface ILogService
    {
        public Task LogLoginAttemptAsync(string name, string role, bool issuccess, string? message, string? ipAddress);
    }
}

using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Interfaces;

namespace SmartMeterWeb.Services
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogLoginAttemptAsync(string name, string role, bool issuccess, string? message, string? ipAddress)
        {
            var log = new LoginLog 
            { 
                Name = name,
                Role = role,
                IsSuccess = issuccess,
                Message = message,
                TimeStamp = DateTime.UtcNow,
                IpAddress = ipAddress
            };

            _context.LoginLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}

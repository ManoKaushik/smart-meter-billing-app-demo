using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;
using SmartMeterWeb.Models.AuthDto;
using SmartMeterWeb.Models.Common;
using System.Security.Claims;


namespace SmartMeterWeb.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;

        public ApplicationService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
        }

        public async Task<IEnumerable<Application>> GetApplicationsAsync()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<AuthResponseDto> ApproveApplicationAsync(ApplicationDto dto)
        {
            var person = await _context.Applications.FirstOrDefaultAsync(p => p.Name == dto.Name);
            if (person == null)
            {
                throw new ApiException("No Application Found", 404);
            }

            var consumer = new Consumer
            {
                Name = person.Name,
                Email = person.Email,
                Phone = person.Phone,
                OrgUnitId = dto.OrgUnitId,
                TariffId = dto.TariffId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "System",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Consumer123")
            };

            

            await _mailService.SendEmailAsync(
                    consumer.Email ?? "consumer.test@local.com", // fallback for testing
                    "Consumer Registration Appproved",
                    $@"
                        <h3>Hello {consumer.Name},</h3>
                        <p>Congratulations! Your application for registering for smartmeter services is approved.</p>
                        <p>
                            You can apply for a smart meter now.
                        </p>
                        <p> Your default password is ""Consumer123"". Please change your password after first login.
                        <p>Thank you,<br/>Smart Meter System</p>
                    "
            );
            person.Status = "Approved";
            _context.Consumers.Add(consumer);
            await _context.SaveChangesAsync();
            return new AuthResponseDto { Name = consumer.Name, Role = "Consumer", Message = "Email sent to consumer"};
        }
    }
}

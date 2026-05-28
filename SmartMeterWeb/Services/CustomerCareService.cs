using Microsoft.EntityFrameworkCore;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Exceptions;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;


namespace SmartMeterWeb.Services
{
    public class CustomerCareService : ICustomerCareService
    {
        
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        private readonly ILogger<CustomerCareService> _logger;

        public CustomerCareService(AppDbContext context, IMailService mailService, ILogger<CustomerCareService> logger)
        {
            _context = context;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task AddMessageAsync(CustomerCareDto dto)
        {
            _logger.LogInformation("Adding new message from {Name}", dto.Name);

            try
            {
                var message = new CustomerCareMessage
                {
                    ConsumerId = dto.ConsumerId,
                    Name = dto.Name,
                    Phone = dto.PhoneNumber,
                    Message = dto.Message,
                    mailid = dto.mailid
                };

                _context.CustomerCareMessages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer message saved successfully for {Name}", dto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving message for {Name}", dto.Name);
                throw new ApiException("Unexpected error occurred while saving message", 500);
            }
        }

        public async Task<List<CustomerCareMessage>> GetAllMessagesAsync()
        {
            return await _context.CustomerCareMessages
                .OrderByDescending(m => m.MessageId)
                .ToListAsync();

        }


        public async Task SendReplyToCustomer(CustomerReplyDto dto)
        {
            var reply = new CustomerCareReply
            {
                ResponseId = dto.ResponseId,
                consumerID = dto.consumerID,
                MessageText = dto.MessageText
            };
            await _context.CustomerCareReplies.AddAsync(reply);

            await _context.SaveChangesAsync();

            var consumer = await _context.CustomerCareMessages
                .FirstOrDefaultAsync(c => c.ConsumerId == dto.consumerID);

            
            if (consumer == null)
                throw new ApiException("Consumer not found", 404);

            string customerEmail = consumer.mailid;


            if (string.IsNullOrEmpty(customerEmail))
                throw new ApiException("Consumer email not available", 400);


            try
            {
                await _mailService.SendEmailAsync(
                    customerEmail,
                    "Reply to your query",
                    dto.MessageText
                );
            }
            catch (Exception ex)
            {
                throw new ApiException($"Failed to send email: {ex.Message}", 500);
            }

        }


    }
}

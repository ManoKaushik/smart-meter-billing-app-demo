using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Interfaces
{
    public interface ICustomerCareService
    {
        Task AddMessageAsync(CustomerCareDto dto);
        Task<List<CustomerCareMessage>> GetAllMessagesAsync();

        Task SendReplyToCustomer(CustomerReplyDto dto);

    }
}

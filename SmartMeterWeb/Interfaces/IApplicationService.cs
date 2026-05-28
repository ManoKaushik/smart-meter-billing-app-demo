using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Models;
using SmartMeterWeb.Models.AuthDto;

namespace SmartMeterWeb.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetApplicationsAsync();
        Task<AuthResponseDto> ApproveApplicationAsync(ApplicationDto dto);
    }
}

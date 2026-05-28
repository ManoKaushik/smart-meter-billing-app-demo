using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("Applications")]
        public async Task<ActionResult> GetApplications()
        {
            var response = await _applicationService.GetApplicationsAsync();
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpPost("Approve")]
        public async Task<ActionResult> ApproveConsumer([FromBody] ApplicationDto dto)
        {
            var response = await _applicationService.ApproveApplicationAsync(dto);
            return Ok(response);
        }
    }


}

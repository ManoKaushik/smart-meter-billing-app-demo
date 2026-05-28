using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Models.AuthDto;
using SmartMeterWeb.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using SmartMeterWeb.Interfaces;


namespace SmartMeterWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Login")] 
        public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut("PassWord")]
        public async Task<ActionResult> UpdatePassword([FromBody] PasswordUpdateDto request)
        {
            var res = await _authService.UpdatePassWord(request);
            return Ok(res);
        }
    }
}

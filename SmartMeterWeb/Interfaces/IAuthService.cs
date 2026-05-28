using Microsoft.AspNetCore.Mvc;
using SmartMeterWeb.Models.AuthDto;

namespace SmartMeterWeb.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto Request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto Request);
        Task<ActionResult> UpdatePassWord(PasswordUpdateDto request);
    }
}

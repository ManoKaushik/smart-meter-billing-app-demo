using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SmartMeterWeb.Models.AuthDto
{
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

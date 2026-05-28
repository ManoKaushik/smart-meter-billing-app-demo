namespace SmartMeterWeb.Models.AuthDto
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        public string? DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        
    }
}

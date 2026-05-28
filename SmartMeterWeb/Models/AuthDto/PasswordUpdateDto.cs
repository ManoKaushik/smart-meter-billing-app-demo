namespace SmartMeterWeb.Models.AuthDto
{
    public class PasswordUpdateDto
    {
        public string nameOrEmail { get; set; }
        public string role {  get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
    }
}

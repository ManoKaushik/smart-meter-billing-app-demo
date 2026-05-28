namespace SmartMeterWeb.Data.Entities
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? IpAddress { get; set; }
    }
}

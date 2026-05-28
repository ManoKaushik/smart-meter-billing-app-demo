namespace SmartMeterWeb.Models
{
    public class CustomerCareDto
    {

        public long ConsumerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string mailid { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

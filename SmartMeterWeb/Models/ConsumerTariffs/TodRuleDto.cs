namespace SmartMeterWeb.Models.Tariffs
{
    public class TodRuleDto
    {
        public string Name { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double RatePerKwh { get; set; }
    }
}

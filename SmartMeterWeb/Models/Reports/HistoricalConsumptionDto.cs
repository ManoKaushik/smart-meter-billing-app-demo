namespace SmartMeterWeb.Models.Reports
{
    public class HistoricalConsumptionDto
    {
        public DateTime Date { get; set; }
        public string OrgUnitName { get; set; } = string.Empty;
        public string OrgUnitType { get; set; } = string.Empty;
        public double TotalEnergyConsumed { get; set; }
        public int ConsumerCount { get; set; }
        public double AverageConsumption { get; set; }
        public double PeakConsumption { get; set; }
        public double LowConsumption { get; set; }
    }

    public class HistoricalConsumptionRequestDto
    {
        public DateTime Date { get; set; }
        public int? OrgUnitId { get; set; }
    }

}

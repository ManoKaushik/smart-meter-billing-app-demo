namespace SmartMeterWeb.Models.Reports
{
    public class MonthlyTariffReportDto
    {
        public string TariffName { get; set; } = string.Empty;
        public int TotalConsumers { get; set; }
        public double TotalUnits { get; set; }
        public double BaseRevenue { get; set; }
        public double TaxCollected { get; set; }
        public double TotalRevenue { get; set; }
        public double AvgPerConsumer { get; set; }
        public int OverdueBills { get; set; }
    }


}


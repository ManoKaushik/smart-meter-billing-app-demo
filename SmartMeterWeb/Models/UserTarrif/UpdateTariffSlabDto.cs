namespace SmartMeterWeb.Models.UserTarrif
{
    public class UpdateTariffSlabDto
    {
        public double FromKwh { get; set; }
        public double ToKwh { get; set; }
        public double RatePerKwh { get; set; }
    }
}

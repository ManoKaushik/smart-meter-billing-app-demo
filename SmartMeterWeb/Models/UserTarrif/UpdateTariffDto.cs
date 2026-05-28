namespace SmartMeterWeb.Models.UserTarrif
{
    public class UpdateTariffDto
    {
        public string Name { get; set; }
        public double BaseRate { get; set; }
        public double TaxRate { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly EffectiveTo { get; set; }
    }
}

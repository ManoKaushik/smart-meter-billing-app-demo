namespace SmartMeterWeb.Models.Tariffs
{
    public class TariffInfoDto
    {
        public string TariffName { get; set; }
        public double BaseRate { get; set; }
        public double TaxRate { get; set; }
        public List<TodRuleDto> TodRules { get; set; }
        public List<TariffSlabDto> TariffSlabs { get; set; }
    }
}

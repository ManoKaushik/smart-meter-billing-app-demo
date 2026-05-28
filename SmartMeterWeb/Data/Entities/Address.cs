using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class Address
    {
        [Key] public long AddressId { get; set; }
        [Required][ForeignKey("Consumer")] public long ConsumerId { get; set; }
        public string? HouseNo { get; set; }
        public string? Locality { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public Consumer Consumer { get; set; } = null!;
    }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class Meter
    {
        [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MeterSerialNo { get; set; } = null!;

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string ICCID { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string IMSI { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; } = null!;

        [MaxLength(50)]
        public string? Firmware { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = null!;

        [Required]
        public DateTimeOffset InstallTsUtc { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        [ForeignKey("Consumer")]
        public long? ConsumerId { get; set; }

        public Consumer? Consumer { get; set; }
    }
}

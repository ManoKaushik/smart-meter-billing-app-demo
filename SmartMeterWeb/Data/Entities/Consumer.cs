using SmartMeterWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class Consumer
    {
        [Key] public long ConsumerId { get; set; }
        [Required] public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Required][ForeignKey("OrgUnit")] public int OrgUnitId { get; set; }
        public OrgUnit OrgUnit { get; set; } = null!;
        [Required][ForeignKey("Tariff")] public int TariffId { get; set; }
        public Tariff Tariff { get; set; } = null!;
        [Required] public string Status { get; set; } = "Active";
        [Required] public DateTime CreatedAt { get; set; }
        [Required] public string CreatedBy { get; set; } = "System";
        public DateTime? UpdatedAt { get; set; } 
        public string? UpdatedBy { get; set; } 
        [Required] public string PasswordHash { get; set; }
        [Required] public bool IsDeleted { get; set; } = false;
        public string? PhotoPath { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LoginLockEnd { get; set; }

    }
}

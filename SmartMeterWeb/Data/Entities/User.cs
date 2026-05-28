using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartMeterWeb.Data.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        [Key] public long UserId { get; set; }
        [Required] public string UserName { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public string DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        [Required] public bool IsActive { get; set; } = true;
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LoginLockEnd {  get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class CustomerCareMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }
        public long ConsumerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public string mailid { get; set; }
        public string Message { get; set; }
    }
}


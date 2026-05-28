using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMeterWeb.Data.Entities
{
    public class CustomerCareReply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ResponseId { get; set; }

        public int  consumerID {get;set;}
        public string MessageText { get; set; }


    }
}

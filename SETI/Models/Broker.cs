using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class Broker
    {
        [Key]
        public int BrokerId { get; set; }

        [Display(Name = "BrokerCode")]
        public string BrokerCode { get; set; }

        [Display(Name = "BrokerName")]
        public string BrokerName { get; set; }

        [Required]
        [Display(Name = "LocationRegionId")]
        public int LocationRegionId { get; set; }
    }
}

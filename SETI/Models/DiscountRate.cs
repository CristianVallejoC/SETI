using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class DiscountRate
    {
        [Key]
        public int DiscountRateId { get; set; }

        [Display(Name = "DiscountRatePercentage")]
        public int DiscountRatePercentage { get; set; }

        [Display(Name = "PeriodId")]
        public int PeriodId { get; set; }
    }
}

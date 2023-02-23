using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class Period
    {
        [Required]
        public int PeriodId { get; set; }

        [Display(Name = "PeriodYear")]
        public int PeriodYear { get; set; }

        [Display(Name = "PeriodMonth")]
        public int PeriodMonth { get; set; }
    }
}

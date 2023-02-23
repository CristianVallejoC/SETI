using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class InvestmentSubsector
    {
        [Key]
        public int SubsectorId { get; set; }

        [Display(Name = "SubsectorName")]
        public string SubsectorName { get; set; }

        [Display(Name = "SectorId")]
        public int SectorId { get; set; }
    }
}

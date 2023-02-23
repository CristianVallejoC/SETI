using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class InvestmentSector
    {
        [Key]
        public int SectorId { get; set; }

        [Display(Name = "SectorName")]
        public string SectorName { get; set; }
    }
}

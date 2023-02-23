using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class InvestmentProject
    {
        [Key]
        public int ProjectId { get; set; }

        [Display(Name = "ProjectCode")]
        public string ProjectCode { get; set; }

        [Display(Name = "ProjectDescription")]
        public string ProjectDescription { get; set; }

        [Display(Name = "SubSectorId")]
        public int SubSectorId { get; set; }

        [Display(Name = "BrokerId")]
        public int BrokerId { get; set; }

        [Display(Name = "InvestmentRegionId")]
        public int InvestmentRegionId { get; set; }

        [Display(Name = "InvestmentAmount")]
        public decimal InvestmentAmount { get; set; }
    }
}

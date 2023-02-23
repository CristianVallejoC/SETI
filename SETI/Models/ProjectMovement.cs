using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class ProjectMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Display(Name = "MovementAmout")]
        public decimal MovementAmount { get; set; }

        [Display(Name = "PeriodId")]
        public int PeriodId { get; set; }

        [Display(Name = "ProjectId")]
        public int ProjectId { get; set; }

    }
}

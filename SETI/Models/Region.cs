using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class Region
    {
        [Key]
        public int RegionId { get; set; }

        [Display(Name = "RegionCode")]
        public string RegionCode { get; set; }

        [Display(Name = "RegionName")]
        public string RegionName{ get; set; }
    }
}

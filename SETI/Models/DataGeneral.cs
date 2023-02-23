using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SETI.Models
{
    public class DataGeneral
    {
        [Key]
        public int IdData { get; set; }
        public int BrokerId { get; set; }
        public string BrokerName { get; set; }
        public int ProjectId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal ValueVan { get; set; }
        public decimal ValuePayback { get; set; }
    }
}

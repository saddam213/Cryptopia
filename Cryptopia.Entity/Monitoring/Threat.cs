using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class Threat : SummaryBase
    {
        public string Name { get; set; }
        public int Incidents { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public string StatusTextId { get; set; }
        public string FollowUp { get; set; }
        public string FollowupText { get; set; }
        public string FollowupUrl { get; set; }
    }
}

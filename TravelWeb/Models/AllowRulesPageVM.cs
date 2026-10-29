using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class AllowRulesPageVM
    {
        public IEnumerable<AllowAirlineRuleDto> Rules { get; set; }
        public AllowAirlineRuleDto FormRule { get; set; }
    }
}

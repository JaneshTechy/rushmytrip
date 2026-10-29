using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class MarkupRulesPageVM
    {
        public IEnumerable<MarkupRuleDto> Rules { get; set; }
        public MarkupRuleDto FormRule { get; set; }
    }
}

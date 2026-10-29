
using System;
using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class BlockRulesPageVM
    {
        public IEnumerable<BlockAirlineRule> Rules { get; set; }
        public BlockAirlineRule FormRule { get; set; }

    }
}

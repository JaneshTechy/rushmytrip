using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class ProductOptionResponse
    {
        public string FieldName { get; set; }
        public string KeyName { get; set; }
        public List<ProductOptionValue> OptionValues { get; set; }
    }

    public class ProductOptionValue
    {
        public string DisplayText { get; set; }
        public string OptionValue { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal? ItemCost { get; set; }
        public string ItemMessage { get; set; }
    }
}

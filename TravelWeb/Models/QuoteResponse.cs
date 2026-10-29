using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class QuoteResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<AvailableOption> AvailableOptions { get; set; }
        public List<QuoteViewModelProductDetail> QuoteViewModelProductDetails { get; set; }
    }

    public class AvailableOption
    {
        public string FieldName { get; set; }
        public string KeyName { get; set; }
        public bool IsRequired { get; set; }
        public bool SelectList { get; set; }
        public string DisplayText { get; set; }
        public int DisplayOrder { get; set; }
        public List<OptionValue> OptionValues { get; set; }
    }

    public class OptionValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class QuoteViewModelProductDetail
    {
        public int OrderId { get; set; }
        public QuoteResponseDetail QuoteResponseDetail { get; set; }
    }

    public class QuoteResponseDetail
    {
        public int ProductId { get; set; }
        public decimal TotalPrice { get; set; }
        public long QuoteNumber { get; set; }
        public string BuyNowLink { get; set; }
        public string QuotedEffDate { get; set; }
        public string QuotedTermDate { get; set; }
        public string DayCount { get; set; }
        public int OrderId { get; set; }

    }
}

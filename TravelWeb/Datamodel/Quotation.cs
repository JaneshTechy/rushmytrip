using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Quotation
    {
        public int QuoteId { get; set; }
        public string QuoteRefId { get; set; }
        public string QuoteType { get; set; }
        public int? ConsolidatorId { get; set; }
        public int? AgentId { get; set; }
        public string EmailSubject { get; set; }
        public string QuoteToTitle { get; set; }
        public string QuoteToFirstName { get; set; }
        public string QuoteToLastName { get; set; }
        public string QuoteToEmail { get; set; }
        public string QuoteCcemail { get; set; }
        public string QuoteRemarks { get; set; }
        public string QuoteAddedBy { get; set; }
        public DateTime? QuoteAddedOn { get; set; }
        public string QuoteBookedFor { get; set; }
        public int? QuoteBookedForAgentCompanyId { get; set; }
    }
}

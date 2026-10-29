using System;
using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class CliqPaymentResponse
    {
        public string Raw { get; set; }

        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        public bool Approved => Values.TryGetValue("response", out var r) && (r == "1" || r.Equals("approved", StringComparison.OrdinalIgnoreCase));
        public string TransactionId => Values.TryGetValue("transactionid", out var v) ? v : null;
        public string Message => Values.TryGetValue("responsetext", out var v) ? v : Values.TryGetValue("message", out var m) ? m : null;
    }
}

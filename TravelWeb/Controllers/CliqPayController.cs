using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace TravelWeb.Controllers
{
    public class CliqPayController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _gatewayUrl = "https://secure.cardflexonline.com/api/transact.php";

        public CliqPayController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public IActionResult CliqPaymentGateway()
        {
            return View();
        }
        [HttpPost("sale")]
        public async Task<IActionResult> Sale([FromForm] TransactionRequest formReq, [FromBody] TransactionRequest jsonReq)
        {
            // Accept from either form or JSON. Prefer JSON if provided.
            var req = jsonReq ?? formReq;

            // If both null, return BadRequest
            if (req == null) return BadRequest("Request body is required (form-data or JSON).");

            // Manual minimal validation (you can expand)
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(req.CcNumber) && string.IsNullOrWhiteSpace(req.PaymentToken))
                missing.Add("CcNumber or PaymentToken");
            if (string.IsNullOrWhiteSpace(req.CcExp) && string.IsNullOrWhiteSpace(req.PaymentToken))
                missing.Add("CcExp or PaymentToken");
            if (string.IsNullOrWhiteSpace(req.Cvv) && string.IsNullOrWhiteSpace(req.PaymentToken))
                missing.Add("Cvv or PaymentToken");
            if (!req.Amount.HasValue || req.Amount <= 0) missing.Add("Amount");
            if (string.IsNullOrWhiteSpace(req.FirstName)) missing.Add("FirstName");
            if (string.IsNullOrWhiteSpace(req.LastName)) missing.Add("LastName");
            if (string.IsNullOrWhiteSpace(req.Email)) missing.Add("Email");
            if (string.IsNullOrWhiteSpace(req.OrderId)) missing.Add("OrderId");

            if (missing.Any())
                return BadRequest(new { error = "Missing required fields", missing });

            // Build fields and send to gateway (same logic as before)
            var fields = req.ToDictionary();
            fields["type"] = "sale";

            if (!string.IsNullOrEmpty(req.PaymentToken))
            {
                fields["payment_token"] = req.PaymentToken.Trim();
                fields.Remove("ccnumber");
                fields.Remove("ccexp");
                fields.Remove("cvv");
            }

            // Masked debug
            var debug = string.Join("&", fields.Select(kv =>
            {
                if (kv.Key.Equals("ccnumber", StringComparison.OrdinalIgnoreCase))
                    return $"{kv.Key}=****{kv.Value?.Substring(Math.Max(0, kv.Value.Length - 4))}";
                if (kv.Key.Equals("cvv", StringComparison.OrdinalIgnoreCase))
                    return $"{kv.Key}=***";
                return $"{kv.Key}={kv.Value}";
            }));
            Console.WriteLine($"[DEBUG] Posting to gateway: {debug}");

            var res = await PostToGatewayAsync(fields);
            return Ok(res);
        }

        public class TransactionRequest
        {
            public string CcNumber { get; set; }           // ccnumber
            public string CcExp { get; set; }              // ccexp (MMYY or MM/YY)
            public string Cvv { get; set; }                // cvv
            public decimal? Amount { get; set; }           // amount
            public string FirstName { get; set; }          // first_name
            public string LastName { get; set; }           // last_name
            public string Address1 { get; set; }           // address1
            public string City { get; set; }               // city
            public string State { get; set; }              // state
            public string Zip { get; set; }                // zip
            public string Country { get; set; }
            public string Email { get; set; }              // email
            public string OrderId { get; set; }            // orderid
            public string PaymentToken { get; set; }       // payment_token
            public string GooglePayData { get; set; }      // googlepay_payment_data
            public string ApplePayData { get; set; }       // applepay_payment_data
            public string CustomerVaultId { get; set; }    // customer_vault_id
            public bool? CustomerReceipt { get; set; }     // customer_receipt

            public Dictionary<string, string> ToDictionary()
            {
                var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(CcNumber)) d["ccnumber"] = CcNumber;
                if (!string.IsNullOrEmpty(CcExp)) d["ccexp"] = CcExp.Replace("/", "");
                if (!string.IsNullOrEmpty(Cvv)) d["cvv"] = Cvv;
                if (Amount.HasValue) d["amount"] = Amount.Value.ToString("F2");
                if (!string.IsNullOrEmpty(FirstName)) d["first_name"] = FirstName;
                if (!string.IsNullOrEmpty(LastName)) d["last_name"] = LastName;
                if (!string.IsNullOrEmpty(Address1)) d["address1"] = Address1;
                if (!string.IsNullOrEmpty(City)) d["city"] = City;
                if (!string.IsNullOrEmpty(State)) d["state"] = State;
                if (!string.IsNullOrEmpty(Zip)) d["zip"] = Zip;
                if (!string.IsNullOrEmpty(Country)) d["country"] = Country;     // <-- INCLUDE HERE
                if (!string.IsNullOrEmpty(Email)) d["email"] = Email;
                if (!string.IsNullOrEmpty(OrderId)) d["orderid"] = OrderId;
                if (!string.IsNullOrEmpty(PaymentToken)) d["payment_token"] = PaymentToken;
                if (!string.IsNullOrEmpty(GooglePayData)) d["googlepay_payment_data"] = GooglePayData;
                if (!string.IsNullOrEmpty(ApplePayData)) d["applepay_payment_data"] = ApplePayData;
                if (!string.IsNullOrEmpty(CustomerVaultId)) d["customer_vault_id"] = CustomerVaultId;
                if (CustomerReceipt.HasValue) d["customer_receipt"] = CustomerReceipt.Value ? "true" : "false";
                return d;
            }
        }

        private async Task<Dictionary<string, string>> PostToGatewayAsync(Dictionary<string, string> formFields)
        {
            // Add security key automatically if not supplied
            if (!formFields.ContainsKey("security_key"))
                formFields["security_key"] = "9Uypg2438KK8fYCkjYCZ4N7V7kmTbEHS";

            var client = _httpFactory.CreateClient();
            var content = new FormUrlEncodedContent(formFields);

            var resp = await client.PostAsync(_gatewayUrl, content);
            var respText = await resp.Content.ReadAsStringAsync();

            // Gateway returns querystring-like body: var1=val1&var2=val2
            var parsed = ParseGatewayResponse(respText);
            // Add HTTP status so caller can inspect if desired
            parsed["_http_status"] = ((int)resp.StatusCode).ToString();
            parsed["_raw_response"] = respText;
            return parsed;
        }
        private Dictionary<string, string> ParseGatewayResponse(string body)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(body)) return dict;

            var parts = body.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                var kv = p.Split('=', 2);
                var key = HttpUtility.UrlDecode(kv[0] ?? "").Trim();
                var val = kv.Length > 1 ? HttpUtility.UrlDecode(kv[1] ?? "").Trim() : "";
                if (!string.IsNullOrEmpty(key))
                {
                    // if duplicate keys, append index
                    if (dict.ContainsKey(key))
                    {
                        var i = 1;
                        while (dict.ContainsKey($"{key}_{i}")) i++;
                        dict[$"{key}_{i}"] = val;
                    }
                    else dict[key] = val;
                }
            }
            return dict;
        }
    }
}

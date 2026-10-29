using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace PayCliqMethod.Services
{
    public interface ICardflexPaymentService
    {
        Task<CliqPaymentResponse> SaleAsync(CliqPaymentRequest req, CancellationToken ct = default);
        Task<CliqPaymentResponse> CaptureAsync(string transactionId, decimal amount, CancellationToken ct = default);
        Task<CliqPaymentResponse> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default);
        Task<CliqPaymentResponse> AddCustomerToVaultAsync(Dictionary<string, string> fields, CancellationToken ct = default);
        Task<CliqPaymentResponse> CreateInvoiceAsync(Dictionary<string, string> fields, CancellationToken ct = default);
    }

    public class CardflexPaymentService : ICardflexPaymentService
    {
        private readonly HttpClient _http;
        private readonly CliqPaymentConfig _cfg;
        private readonly ILogger<CardflexPaymentService> _log;

        public CardflexPaymentService(HttpClient http, IOptions<CliqPaymentConfig> config, ILogger<CardflexPaymentService> log)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _cfg = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            if (!Uri.TryCreate(_cfg.ApiUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Payment API URL is invalid or missing in configuration.");
            }
        }

        private static string MaskPan(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan)) return null;
            var digits = pan.Where(char.IsDigit).ToArray();
            if (digits.Length <= 4) return new string('*', digits.Length);
            var last4 = new string(digits.Skip(digits.Length - 4).ToArray());
            return "**** **** **** " + last4;
        }

        private FormUrlEncodedContent BuildFormContent(Dictionary<string, string> fields)
        {
            return new FormUrlEncodedContent(fields);
        }

        private async Task<CliqPaymentResponse> PostFormAsync(Dictionary<string, string> fields, CancellationToken ct)
        {
            fields["security_key"] = _cfg.SecurityKey;

            if (fields.TryGetValue("ccnumber", out var pan))
            {
                _log.LogDebug("Submitting transaction for card ending {Last4}", pan.Length >= 4 ? pan[^4..] : pan);
            }
            else if (fields.TryGetValue("payment_token", out var tkn))
            {
                _log.LogDebug("Submitting transaction using payment token {Token}", tkn.Length > 8 ? tkn[..8] + "..." : tkn);
            }

            var content = BuildFormContent(fields);
            using var resp = await _http.PostAsync(_cfg.ApiUrl, content, ct);
            var body = await resp.Content.ReadAsStringAsync();
            ct.ThrowIfCancellationRequested();

            var pr = new CliqPaymentResponse { Raw = body };
            try
            {
                pr.Values = ParseResponse(body);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to parse gateway response: {Body}", body);
            }

            if (!resp.IsSuccessStatusCode)
            {
                _log.LogWarning("Gateway returned non-success status code {Status}. Body: {Body}", resp.StatusCode, MaskForLog(body));
            }

            return pr;
        }

        private static string MaskForLog(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return s
                .Replace("ccnumber=", "ccnumber=****")
                .Replace("cc_number=", "cc_number=****")
                .Replace("cvv=", "cvv=***")
                .Replace("ccexp=", "ccexp=****"); 
        }


        private static Dictionary<string, string> ParseResponse(string body)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(body)) return dict;

            var pairs = body.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in pairs)
            {
                var idx = p.IndexOf('=');
                if (idx < 0) continue;
                var key = WebUtility.UrlDecode(p.Substring(0, idx));
                var val = WebUtility.UrlDecode(p.Substring(idx + 1));
                if (!dict.ContainsKey(key)) dict[key] = val;
                else dict[key] = dict[key] + ";" + val;
            }
            return dict;
        }

        private Dictionary<string, string> BuildSaleFields(CliqPaymentRequest req)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["type"] = req.TransactionType ?? "sale",
                ["amount"] = req.Amount.ToString("0.00"),
                ["payment"] = req.PaymentType ?? "creditcard",
            };

            if (!string.IsNullOrWhiteSpace(req.FirstName)) fields["first_name"] = req.FirstName;
            if (!string.IsNullOrWhiteSpace(req.LastName)) fields["last_name"] = req.LastName;
            if (!string.IsNullOrWhiteSpace(req.Address1)) fields["address1"] = req.Address1;
            if (!string.IsNullOrWhiteSpace(req.City)) fields["city"] = req.City;
            if (!string.IsNullOrWhiteSpace(req.State)) fields["state"] = req.State;
            if (!string.IsNullOrWhiteSpace(req.Country)) fields["country"] = req.Country;
            if (!string.IsNullOrWhiteSpace(req.Zip)) fields["zip"] = req.Zip;
            if (!string.IsNullOrWhiteSpace(req.Email)) fields["email"] = req.Email;
            if (!string.IsNullOrWhiteSpace(req.IpAddress)) fields["ipaddress"] = req.IpAddress;
            if (!string.IsNullOrWhiteSpace(req.OrderId)) fields["orderid"] = req.OrderId;
            if (!string.IsNullOrWhiteSpace(req.OrderDescription)) fields["order_description"] = req.OrderDescription;

            if (!string.IsNullOrWhiteSpace(req.PaymentToken))
            {
                fields["payment_token"] = req.PaymentToken;
            }
            else if (!string.IsNullOrWhiteSpace(req.GooglePayPaymentData))
            {
                fields["googlepay_payment_data"] = req.GooglePayPaymentData;
            }
            else if (!string.IsNullOrWhiteSpace(req.ApplePayPaymentData))
            {
                fields["applepay_payment_data"] = req.ApplePayPaymentData;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(req.CardNumber))
                {
                    fields["ccnumber"] = req.CardNumber;
                }
                if (!string.IsNullOrWhiteSpace(req.CardExpiry))
                {
                    fields["ccexp"] = req.CardExpiry;
                }
                if (!string.IsNullOrWhiteSpace(req.CardCvv))
                {
                    fields["cvv"] = req.CardCvv;
                }
            }

            return fields;
        }

        public Task<CliqPaymentResponse> SaleAsync(CliqPaymentRequest req, CancellationToken ct = default)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            var fields = BuildSaleFields(req);
            return PostFormAsync(fields, ct);
        }

        public Task<CliqPaymentResponse> CaptureAsync(string transactionId, decimal amount, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(transactionId)) throw new ArgumentNullException(nameof(transactionId));
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["type"] = "capture",
                ["transactionid"] = transactionId,
                ["amount"] = amount.ToString("0.00")
            };
            return PostFormAsync(fields, ct);
        }

        public Task<CliqPaymentResponse> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(transactionId)) throw new ArgumentNullException(nameof(transactionId));
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["type"] = "refund",
                ["transactionid"] = transactionId,
                ["amount"] = amount.ToString("0.00")
            };
            return PostFormAsync(fields, ct);
        }

        public Task<CliqPaymentResponse> AddCustomerToVaultAsync(Dictionary<string, string> fields, CancellationToken ct = default)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            fields["customer_vault"] = fields.TryGetValue("customer_vault", out var v) ? v : "add_customer";
            return PostFormAsync(fields, ct);
        }

        public Task<CliqPaymentResponse> CreateInvoiceAsync(Dictionary<string, string> fields, CancellationToken ct = default)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            fields["invoicing"] = fields.TryGetValue("invoicing", out var v) ? v : "add_invoice";
            return PostFormAsync(fields, ct);
        }

    }
}

using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravelWeb.Datamodel;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentSettings _settings;


        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IOptions<PaymentSettings> options, ILogger<PaymentController> logger)
        {

            _settings = options.Value;
            _logger = logger;
        }

        private static string MapStatus(string responseCode, bool ok, bool heldForReview)
        {
            // Authorize.Net responseCode: "1"=Approved, "2"=Declined, "3"=Error, "4"=Held for review
            if (!ok)
            {
                return responseCode switch
                {
                    "2" => "Declined",
                    "3" => "Error",
                    "4" => "Review",
                    _ => "Error"
                };
            }
            if (heldForReview) return "Review";
            return "Approved";
        }

        private async Task<int> UpsertPaymentAsync(string orderId, string env, PaymentViewModel model,
      createTransactionResponse response, string invoice, string rawJson)
        {
            var context = new RSFLY();
            var tr = response?.transactionResponse;

            var entity = context.Payments.FirstOrDefault(p => p.OrderId == orderId);
            var now = DateTime.UtcNow;

            if (entity == null)
            {
                entity = new Payment
                {
                    OrderId = orderId,
                    Amount = model.Amount,
                    Currency = "USD",
                    CustomerEmail = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    //Address = model.Address,    
                    //City = model.City,            
                    //Country = model.Country,      
                    //ZipCode = model.ZipCode,      
                    InvoiceNumber = model.invoice,
                    GatewayEnv = env,
                    CreatedAt = now
                };
                context.Payments.Add(entity);
            }

            var ok = response?.messages?.resultCode == messageTypeEnum.Ok;
            var held = tr?.responseCode == "4";
            entity.Status = MapStatus(tr?.responseCode, ok, held);
            entity.AnetTransactionId = tr?.transId ?? entity.AnetTransactionId;
            entity.AuthCode = tr?.authCode ?? entity.AuthCode;
            entity.AvsResult = tr?.avsResultCode ?? entity.AvsResult;
            entity.CvvResult = tr?.cvvResultCode ?? entity.CvvResult;
            entity.RawResponseJson = rawJson;
            entity.UpdatedAt = now;

            context.SaveChanges();
            return entity.Id;
        }

        private async Task<int?> AddEventAsync(int? paymentId, string type, object payload)
        {
            var context = new RSFLY();
            var json = payload is string s ? s : JsonSerializer.Serialize(payload);
            var pe = new PaymentEvent
            {
                PaymentId = paymentId,
                EventType = type,
                PayloadJson = json,
                ReceivedAt = DateTime.UtcNow
            };
            context.PaymentEvents.Add(pe);
            await context.SaveChangesAsync();
            return pe.Id;
        }

        [HttpGet]
        public IActionResult Index(string? account = null)
        {
            try
            {
                //// Load City XML
                //var cityPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "xml", "City.xml");
                //var cities = new List<object>();

                //if (System.IO.File.Exists(cityPath))
                //{
                //    var xml = XDocument.Load(cityPath);
                //    cities = xml.Descendants("City")
                //        .Select(c => new
                //        {
                //            CityCode = c.Element("CityCode")?.Value ?? "",
                //            CityName = c.Element("CityName")?.Value ?? "",
                //            StateName = c.Element("StateName")?.Value ?? "",
                //            CountryName = c.Element("CountryName")?.Value ?? ""
                //        })
                //        .ToList<object>();
                //}

                // Load Countries JSON
                var countryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "data_iso_3166-1.json");
                var countries = new List<Country>();
                if (System.IO.File.Exists(countryPath))
                {
                    var countryJson = System.IO.File.ReadAllText(countryPath);
                    var countryWrapper = JsonSerializer.Deserialize<CountryWrapper>(
                        countryJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    countries = countryWrapper?.Countries ?? new List<Country>();
                }

                // Load States JSON
                var statePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "data_iso_3166-2.json");
                var states = new List<State>();
                if (System.IO.File.Exists(statePath))
                {
                    var stateJson = System.IO.File.ReadAllText(statePath);
                    var stateWrapper = JsonSerializer.Deserialize<StateWrapper>(
                        stateJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    states = stateWrapper?.States ?? new List<State>();
                }

                // Send serialized data to view
                //ViewBag.CitiesJson = JsonSerializer.Serialize(cities);
                ViewBag.CountriesJson = JsonSerializer.Serialize(countries);
                ViewBag.StatesJson = JsonSerializer.Serialize(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading location data");
                ViewBag.CitiesJson = "[]";
                ViewBag.CountriesJson = "[]";
                ViewBag.StatesJson = "[]";
            }
            var selected = (!string.IsNullOrWhiteSpace(account) && _settings.Accounts.ContainsKey(account))
        ? account
        : _settings.DefaultAccount;

            return View(new PaymentViewModel { Amount = 1.00M, Account = selected });

            //return View(new PaymentViewModel { Amount = 1.00M });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Charge(PaymentViewModel model)
        {
            var context = new RSFLY();

            if (model.Amount <= 0)
                ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(model.OpaqueDataDescriptor) ||
                string.IsNullOrWhiteSpace(model.OpaqueDataValue))
                ModelState.AddModelError("", "Payment token missing. Please try again.");
            
            if (!_settings.Accounts.TryGetValue(model.Account ?? "", out var acct))
                ModelState.AddModelError(nameof(model.Account), "Invalid payment account.");

            if (!ModelState.IsValid)
                return View("Index", model);

            // ===== 1) Setup ANet environment & merchant auth =====
            //ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment =
            //    _settings.UseSandbox ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;

            //var merchantAuth = new merchantAuthenticationType
            //{
            //    name = _settings.ApiLoginId,
            //    ItemElementName = ItemChoiceType.transactionKey,
            //    Item = _settings.TransactionKey
            //};

            var env = acct.UseSandbox ? "SANDBOX" : "PRODUCTION";

            var merchantAuth = new merchantAuthenticationType
            {
                name = acct.ApiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = acct.TransactionKey
            };
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = merchantAuth;

            // ===== 2) Accept.js opaque payment =====
            var payment = new paymentType
            {
                Item = new opaqueDataType
                {
                    dataDescriptor = model.OpaqueDataDescriptor,
                    dataValue = model.OpaqueDataValue
                }
            };

            // ===== 3) Create invoice and INSERT PENDING first =====
            // string invoice = MakeInvoice(); // <= 20 chars
            var now = DateTime.UtcNow;
            //var env = _settings.UseSandbox ? "SANDBOX" : "PRODUCTION";

            // Idempotency guard: if the same invoice already exists, reuse it (prevents duplicate rows on re-submit)
            var paymentEntity = context.Payments.FirstOrDefault(x => x.InvoiceNumber == model.invoice);
            if (paymentEntity == null)
            {
                paymentEntity = new Payment
                {
                    OrderId = model.invoice,
                    Amount = model.Amount,
                    Currency = "USD",
                    Status = "Pending",
                    CustomerEmail = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    //Address = model.Address,
                    //City = model.City,
                    //Country = model.Country,
                    //ZipCode = model.ZipCode,
                    InvoiceNumber = model.invoice,
                    GatewayEnv = env,
                    CreatedAt = now,
                    UpdatedAt = now,
                    GatewayAccount = model.Account
                };
                context.Payments.Add(paymentEntity);
                context.SaveChanges();
            }

            // Optional: log an init event
            context.PaymentEvents.Add(new PaymentEvent
            {
                PaymentId = paymentEntity.Id,
                EventType = "charge:init",
                PayloadJson = JsonSerializer.Serialize(new
                {
                    amount = model.Amount,
                    model.invoice,
                    env
                }),
                ReceivedAt = now
            });
            context.SaveChanges();

            // ===== 4) Build request and call gateway =====
            //var order = new orderType { invoiceNumber = model.invoice, description = "Web checkout" };

            var txnRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = model.Amount,
                payment = payment,
                order = new orderType
                {
                    invoiceNumber = model.invoice,
                    description = "Travel booking payment"
                },
                billTo = new customerAddressType
                {
                    firstName = model.FirstName,
                    email = model.Email,
                    country = model.Country,
                    lastName = model.LastName,
                    address = model.Address,
                    city = model.City,
                    state = model.State,
                    zip = model.ZipCode,
                    phoneNumber = CleanPhone(model.Phone)
                },
                customer = new customerDataType
                {
                    email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email,
                    id = model.invoice
                },
                transactionSettings = new[]
                {
            new settingType
            {
                settingName = "duplicateWindow",
                settingValue = _settings.DuplicateWindowSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        },
                //customerIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            //var request = new createTransactionRequest { transactionRequest = txnRequest };

            var request = new createTransactionRequest
            {
                merchantAuthentication = merchantAuth,   // per-request credentials
                transactionRequest = txnRequest
            };

            var controller = new createTransactionController(request);

            createTransactionResponse response = null;
            ANetApiResponse errorResponse = null;

            try
            {
                //controller.Execute();
                controller.Execute(acct.UseSandbox ? AuthorizeNet.Environment.SANDBOX
                                   : AuthorizeNet.Environment.PRODUCTION);
                response = controller.GetApiResponse();
                errorResponse = controller.GetErrorResponse();
            }
            catch (Exception ex)
            {
                // Transport/SDK error → mark as Error
                paymentEntity.Status = "Error";
                paymentEntity.RawResponseJson = JsonSerializer.Serialize(new { exception = ex.Message, model.invoice, env });
                paymentEntity.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();

                context.PaymentEvents.Add(new PaymentEvent
                {
                    PaymentId = paymentEntity.Id,
                    EventType = "charge:exception",
                    PayloadJson = paymentEntity.RawResponseJson,
                    ReceivedAt = DateTime.UtcNow
                });
                context.SaveChanges();

                ViewBag.IsSuccess = false;
                ViewBag.Message = "Payment processing error. Please try again.";
                ViewBag.Invoice = model.invoice;
                return View("Result");
            }

            // ===== 5) Update Payment with final status and log response =====
            var rawJson = JsonSerializer.Serialize(new { api = "createTransaction", response, error = errorResponse });
            var tr = response?.transactionResponse;
            var ok = response?.messages?.resultCode == messageTypeEnum.Ok;
            var held = tr?.responseCode == "4";
            var status = MapStatus(tr?.responseCode, ok, held);

            paymentEntity.Status = status;
            paymentEntity.AnetTransactionId = tr?.transId ?? paymentEntity.AnetTransactionId;
            paymentEntity.AuthCode = tr?.authCode ?? paymentEntity.AuthCode;
            paymentEntity.AvsResult = tr?.avsResultCode ?? paymentEntity.AvsResult;
            paymentEntity.CvvResult = tr?.cvvResultCode ?? paymentEntity.CvvResult;
            paymentEntity.RawResponseJson = rawJson;
            paymentEntity.UpdatedAt = DateTime.UtcNow;
            context.SaveChanges();

            context.PaymentEvents.Add(new PaymentEvent
            {
                PaymentId = paymentEntity.Id,
                EventType = "charge:response",
                PayloadJson = rawJson,
                ReceivedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            // ===== 6) Your existing UI flow =====
            if (response == null)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = errorResponse?.messages?.message?.FirstOrDefault()?.text
                                  ?? "Null response from payment gateway.";
                ViewBag.Invoice = model.invoice;
                ViewBag.PaymentId = paymentEntity.Id;
                return View("Result");
            }

            if (ok && tr?.responseCode == "1")
            {
                ViewBag.IsSuccess = true;
                ViewBag.TransactionId = tr.transId;
                ViewBag.AuthCode = tr.authCode;
                ViewBag.Message = tr.messages != null && tr.messages.Length > 0
                    ? tr.messages[0].description : "Approved";
                ViewBag.AVS = tr.avsResultCode;
                ViewBag.CVV = tr.cvvResultCode;
                ViewBag.Invoice = model.invoice;
                ViewBag.PaymentId = paymentEntity.Id;
            }
            else
            {
                ViewBag.IsSuccess = false;

                if (tr?.errors != null && tr.errors.Length > 0)
                    ViewBag.Message = string.Join("; ", tr.errors.Select(e => $"{e.errorCode}: {e.errorText}"));
                else if (response.messages?.message != null && response.messages.message.Length > 0)
                    ViewBag.Message = $"{response.messages.message[0].code}: {response.messages.message[0].text}";
                else
                    ViewBag.Message = "Unknown error from gateway.";

                ViewBag.AVS = tr?.avsResultCode;
                ViewBag.CVV = tr?.cvvResultCode;
                ViewBag.Invoice = model.invoice;
                ViewBag.PaymentId = paymentEntity.Id;
            }

            return View("Result");
        }
        private static string CleanPhone(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var trimmed = s.Trim();
            var digits = new string(trimmed.Where(char.IsDigit).ToArray());
            var withPlus = trimmed.StartsWith("+") ? "+" + digits : digits;
            return withPlus.Length > 25 ? withPlus[..25] : withPlus;
        }
        // Helper: make an invoice <= 20 chars (ASCII)
        private static string MakeInvoice()
        {
            // Pattern: INV-yyMMdd-XXXXXX (up to 18 chars, avoids length errors)
            //  - yyMMdd = 6
            //  - prefix "INV-" = 4
            //  - hyphen + 6 base36 chars = 7
            // Total = 17; safe under 20
            var date = DateTime.UtcNow.ToString("yyMMdd", CultureInfo.InvariantCulture);
            var rand6 = ShortToken(6); // base36 short token
            return $"INV-{date}-{rand6}";
        }

        private static string ShortToken(int length)
        {
            // Generate base36 token
            Span<byte> bytes = stackalloc byte[8];
            RandomNumberGenerator.Fill(bytes);
            ulong value = BitConverter.ToUInt64(bytes);
            const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] buff = new char[16];
            int i = 0;
            do
            {
                buff[i++] = alphabet[(int)(value % 36)];
                value /= 36;
            } while (value > 0 && i < buff.Length);

            Array.Reverse(buff, 0, i);
            var token = new string(buff, 0, i);
            if (token.Length >= length) return token[..length];
            // pad if needed
            return token.PadLeft(length, '0');
        }
    }
}
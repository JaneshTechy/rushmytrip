using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayCliqMethod.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class CliqPaymentController : Controller
    {
        private readonly ICardflexPaymentService _svc;
        private readonly ILogger<PaymentController> _log;

        public CliqPaymentController(ICardflexPaymentService svc, ILogger<PaymentController> log)
        {
            _svc = svc;
            _log = log;
        }

        [HttpGet]
        public IActionResult Index()
        {
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

            ViewBag.CountriesJson = JsonSerializer.Serialize(countries);
            ViewBag.StatesJson = JsonSerializer.Serialize(states);
            return View(new CliqPaymentRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(CliqPaymentRequest model)
        {

            model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var resp = await _svc.SaleAsync(model);

            model.CardCvv = null;
            model.CardNumber = null;

            return View("Result", resp);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FlightServices.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelWeb.Utilities;
using TravelWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace TravelWeb.Controllers
{
    
    public class CommonController : Controller
    {
        private const string LanguageKey = "SelectedLanguage";
        private const string CurrencyKey = "SelectedCurrency";

        public IActionResult Index()
        {
            return View();
        }

        public string SetSession()
        {
            if (HttpContext.Session.GetObjectFromJson<string>("CommonData") == null)
            {
                Common.GetAirportDataX = Common.GetAirportData();
                Common.GetAirlineDataX = Common.GetAirlineData();
                HttpContext.Session.SetObjectAsJson("CommonData", "TRUE");
            }

            SyncLocaleFromCookies();

            return "ok";
        }

        [HttpGet]
        public IActionResult GetLocale()
        {
            SyncLocaleFromCookies();
            var language = LocaleCurrencyHelper.GetSelectedLanguage(HttpContext);
            var currency = LocaleCurrencyHelper.GetSelectedCurrency(HttpContext);
            return Json(new
            {
                language,
                currency,
                currencySymbol = LocaleCurrencyHelper.GetCurrencySymbol(currency),
                languageLabel = LocaleCurrencyHelper.GetLanguageLabel(language)
            });
        }

        [HttpPost]
        public IActionResult SetLocale(string language, string currency)
        {
            if (!string.IsNullOrWhiteSpace(language))
            {
                HttpContext.Session.SetString(LanguageKey, language);
                Response.Cookies.Append(LanguageKey, language, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24),
                    IsEssential = true,
                    Path = "/"
                });
            }

            if (!string.IsNullOrWhiteSpace(currency))
            {
                HttpContext.Session.SetString(CurrencyKey, currency);
                Response.Cookies.Append(CurrencyKey, currency, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(24),
                    IsEssential = true,
                    Path = "/"
                });
            }

            return Json(new
            {
                success = true,
                language = LocaleCurrencyHelper.GetSelectedLanguage(HttpContext),
                currency = LocaleCurrencyHelper.GetSelectedCurrency(HttpContext),
                currencySymbol = LocaleCurrencyHelper.GetCurrencySymbol(currency ?? "USD")
            });
        }

        private void SyncLocaleFromCookies()
        {
            var language = Request.Cookies[LanguageKey];
            var currency = Request.Cookies[CurrencyKey];

            if (!string.IsNullOrWhiteSpace(language) && HttpContext.Session.GetString(LanguageKey) == null)
                HttpContext.Session.SetString(LanguageKey, language);

            if (!string.IsNullOrWhiteSpace(currency) && HttpContext.Session.GetString(CurrencyKey) == null)
                HttpContext.Session.SetString(CurrencyKey, currency);
        }
    }
}

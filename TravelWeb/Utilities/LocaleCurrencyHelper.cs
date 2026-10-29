using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TravelWeb.Utilities
{
    public class CurrencyOption
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string FlagCountry { get; set; }
    }

    public class LanguageOption
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string FlagCountry { get; set; }
    }

    public static class LocaleCurrencyHelper
    {
        public static readonly IReadOnlyList<LanguageOption> Languages = new List<LanguageOption>
        {
            new LanguageOption { Code = "en", Label = "United States - English", FlagCountry = "us" },
            new LanguageOption { Code = "es", Label = "Estados Unidos - Español", FlagCountry = "us" }
        };

        public static readonly IReadOnlyList<CurrencyOption> Currencies = new List<CurrencyOption>
        {
            new CurrencyOption { Code = "USD", Symbol = "$", FlagCountry = "us" },
            new CurrencyOption { Code = "GBP", Symbol = "£", FlagCountry = "gb" },
            new CurrencyOption { Code = "CAD", Symbol = "C$", FlagCountry = "ca" },
            new CurrencyOption { Code = "EUR", Symbol = "€", FlagCountry = "es" },
            new CurrencyOption { Code = "MXN", Symbol = "MX$", FlagCountry = "mx" },
            new CurrencyOption { Code = "BRL", Symbol = "R$", FlagCountry = "br" },
            new CurrencyOption { Code = "ARS", Symbol = "AR$", FlagCountry = "ar" },
            new CurrencyOption { Code = "COP", Symbol = "COL$", FlagCountry = "co" }
        };

        public static string GetCurrencySymbol(string code)
        {
            return Currencies.FirstOrDefault(c => c.Code == code)?.Symbol ?? "$";
        }

        public static string GetLanguageLabel(string code)
        {
            return Languages.FirstOrDefault(l => l.Code == code)?.Label ?? "United States - English";
        }

        public static string GetSelectedLanguage(HttpContext context)
        {
            return context.Request.Cookies["SelectedLanguage"]
                ?? context.Session.GetString("SelectedLanguage")
                ?? "en";
        }

        public static string GetSelectedCurrency(HttpContext context)
        {
            var selected = context.Request.Cookies["SelectedCurrency"]
                ?? context.Session.GetString("SelectedCurrency")
                ?? "USD";

            // Flight search / pricing should not use MXN from older preference cookies
            if (string.Equals(selected, "MXN", System.StringComparison.OrdinalIgnoreCase))
                selected = "USD";

            return Currencies.Any(c => c.Code == selected) ? selected : "USD";
        }
    }
}

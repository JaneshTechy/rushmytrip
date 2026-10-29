using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TravelWeb.Middleware
{
    public class LocaleMiddleware
    {
        private readonly RequestDelegate _next;
        public const string LanguageKey = "SelectedLanguage";
        public const string CurrencyKey = "SelectedCurrency";

        public LocaleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var language = context.Request.Cookies[LanguageKey];
            var currency = context.Request.Cookies[CurrencyKey];

            if (!string.IsNullOrWhiteSpace(language))
                context.Session.SetString(LanguageKey, language);

            if (!string.IsNullOrWhiteSpace(currency))
                context.Session.SetString(CurrencyKey, currency);

            await _next(context);
        }
    }
}

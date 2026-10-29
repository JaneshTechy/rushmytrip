using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelWeb.Models;


namespace TravelWeb.Controllers
{
    public class GoogleSearchConsoleController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public GoogleSearchConsoleController(
            IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

        }
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var accessToken = HttpContext.Session.GetString(
        //        "GoogleAccessToken"
        //    );

        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        return RedirectToAction("Connect");
        //    }

        //    var siteUrl = "https://www.rushmyflight.com/";

        //    var startDate = DateTime.UtcNow
        //        .AddMonths(-3)
        //        .ToString("yyyy-MM-dd");

        //    var endDate = DateTime.UtcNow
        //        .AddDays(-2)
        //        .ToString("yyyy-MM-dd");

        //    var model = new GoogleSearchConsoleVM();

        //    var summaryTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        null,
        //        1
        //    );

        //    var queryTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        "query",
        //        100
        //    );

        //    var pageTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        "page",
        //        100
        //    );

        //    var dateTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        "date",
        //        1000
        //    );

        //    var countryTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        "country",
        //        100
        //    );

        //    var deviceTask = GetSearchConsoleData(
        //        accessToken,
        //        siteUrl,
        //        startDate,
        //        endDate,
        //        "device",
        //        10
        //    );

        //    await Task.WhenAll(
        //        summaryTask,
        //        queryTask,
        //        pageTask,
        //        dateTask,
        //        countryTask,
        //        deviceTask
        //    );

        //    var summary = await summaryTask;
        //    var queries = await queryTask;
        //    var pages = await pageTask;
        //    var dates = await dateTask;
        //    var countries = await countryTask;
        //    var devices = await deviceTask;

        //    var summaryRow = summary.Rows.FirstOrDefault();

        //    if (summaryRow != null)
        //    {
        //        model.TotalClicks = summaryRow.Clicks;
        //        model.TotalImpressions = summaryRow.Impressions;
        //        model.AverageCtr = summaryRow.Ctr * 100;
        //        model.AveragePosition = summaryRow.Position;
        //    }

        //    model.Queries = queries.Rows
        //        .Select(x => new SearchConsoleQueryVM
        //        {
        //            Query = x.Keys.FirstOrDefault() ?? "",
        //            Clicks = x.Clicks,
        //            Impressions = x.Impressions,
        //            Ctr = x.Ctr * 100,
        //            Position = x.Position
        //        })
        //        .ToList();

        //    model.Pages = pages.Rows
        //        .Select(x => new SearchConsolePageVM
        //        {
        //            Page = x.Keys.FirstOrDefault() ?? "",
        //            Clicks = x.Clicks,
        //            Impressions = x.Impressions,
        //            Ctr = x.Ctr * 100,
        //            Position = x.Position
        //        })
        //        .ToList();

        //    model.Dates = dates.Rows
        //        .Select(x => new SearchConsoleDateVM
        //        {
        //            Date = x.Keys.FirstOrDefault() ?? "",
        //            Clicks = x.Clicks,
        //            Impressions = x.Impressions,
        //            Ctr = x.Ctr * 100,
        //            Position = x.Position
        //        })
        //        .ToList();

        //    model.Countries = countries.Rows
        //        .Select(x => new SearchConsoleCountryVM
        //        {
        //            Country = x.Keys.FirstOrDefault() ?? "",
        //            Clicks = x.Clicks,
        //            Impressions = x.Impressions,
        //            Ctr = x.Ctr * 100,
        //            Position = x.Position
        //        })
        //        .ToList();

        //    model.Devices = devices.Rows
        //        .Select(x => new SearchConsoleDeviceVM
        //        {
        //            Device = x.Keys.FirstOrDefault() ?? "",
        //            Clicks = x.Clicks,
        //            Impressions = x.Impressions,
        //            Ctr = x.Ctr * 100,
        //            Position = x.Position
        //        })
        //        .ToList();
        //    var completeApiResponse = new
        //    {
        //        SiteUrl = siteUrl,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        FetchedAtUtc = DateTime.UtcNow,

        //        Summary = summary,
        //        Queries = queries,
        //        Pages = pages,
        //        Dates = dates,
        //        Countries = countries,
        //        Devices = devices
        //    };

        //    var jsonResponse = JsonSerializer.Serialize(
        //        completeApiResponse,
        //        new JsonSerializerOptions
        //        {
        //            WriteIndented = true
        //        }
        //    );

        //    var jsonFolderPath = Path.Combine(
        //        Directory.GetCurrentDirectory(),
        //        "App_Data",
        //        "GoogleSearchConsole"
        //    );

        //    if (!Directory.Exists(jsonFolderPath))
        //    {
        //        Directory.CreateDirectory(jsonFolderPath);
        //    }

        //    var jsonFilePath = Path.Combine(
        //        jsonFolderPath,
        //        "SearchConsoleLatest.json"
        //    );

        //    await System.IO.File.WriteAllTextAsync(
        //        jsonFilePath,
        //        jsonResponse
        //    );

        //    return View(model);

        //}
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jsonFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "App_Data",
                "GoogleSearchConsole",
                "SearchConsoleLatest.json"
            );

            if (!System.IO.File.Exists(jsonFilePath))
            {
                return Content(
                    "SearchConsoleLatest.json file not found. Path: "
                    + jsonFilePath
                );
            }

            var jsonResponse = await System.IO.File.ReadAllTextAsync(
                jsonFilePath
            );

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                return Content(
                    "SearchConsoleLatest.json file is empty."
                );
            }

            using var jsonDocument = JsonDocument.Parse(jsonResponse);

            var root = jsonDocument.RootElement;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };


            // ==========================================
            // SAME EXISTING API RESPONSE CLASS
            // ==========================================

            var summary = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Summary").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            var queries = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Queries").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            var pages = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Pages").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            var dates = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Dates").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            var countries = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Countries").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            var devices = JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                root.GetProperty("Devices").GetRawText(),
                jsonOptions
            ) ?? new SearchConsoleApiResponse();


            // ==========================================
            // EXISTING VIEW MODEL
            // ==========================================

            var model = new GoogleSearchConsoleVM();


            // ==========================================
            // SUMMARY
            // ==========================================

            var summaryRow = summary.Rows.FirstOrDefault();

            if (summaryRow != null)
            {
                model.TotalClicks = summaryRow.Clicks;

                model.TotalImpressions = summaryRow.Impressions;

                model.AverageCtr = summaryRow.Ctr * 100;

                model.AveragePosition = summaryRow.Position;
            }


            // ==========================================
            // QUERIES
            // ==========================================

            model.Queries = queries.Rows
                .Select(x => new SearchConsoleQueryVM
                {
                    Query = x.Keys.FirstOrDefault() ?? "",

                    Clicks = x.Clicks,

                    Impressions = x.Impressions,

                    Ctr = x.Ctr * 100,

                    Position = x.Position
                })
                .ToList();


            // ==========================================
            // PAGES
            // ==========================================

            model.Pages = pages.Rows
                .Select(x => new SearchConsolePageVM
                {
                    Page = x.Keys.FirstOrDefault() ?? "",

                    Clicks = x.Clicks,

                    Impressions = x.Impressions,

                    Ctr = x.Ctr * 100,

                    Position = x.Position
                })
                .ToList();


            // ==========================================
            // DATES
            // ==========================================

            model.Dates = dates.Rows
                .Select(x => new SearchConsoleDateVM
                {
                    Date = x.Keys.FirstOrDefault() ?? "",

                    Clicks = x.Clicks,

                    Impressions = x.Impressions,

                    Ctr = x.Ctr * 100,

                    Position = x.Position
                })
                .ToList();


            // ==========================================
            // COUNTRIES
            // ==========================================

            model.Countries = countries.Rows
                .Select(x => new SearchConsoleCountryVM
                {
                    Country = x.Keys.FirstOrDefault() ?? "",

                    Clicks = x.Clicks,

                    Impressions = x.Impressions,

                    Ctr = x.Ctr * 100,

                    Position = x.Position
                })
                .ToList();


            // ==========================================
            // DEVICES
            // ==========================================

            model.Devices = devices.Rows
                .Select(x => new SearchConsoleDeviceVM
                {
                    Device = x.Keys.FirstOrDefault() ?? "",

                    Clicks = x.Clicks,

                    Impressions = x.Impressions,

                    Ctr = x.Ctr * 100,

                    Position = x.Position
                })
                .ToList();


            return View(model);
        }
        [HttpGet]
        public IActionResult Connect()
        {
            var clientId = _configuration["GoogleSearchConsole:ClientId"];
            var redirectUri = _configuration["GoogleSearchConsole:RedirectUri"];

            var scope = "https://www.googleapis.com/auth/webmasters.readonly";

            var googleAuthUrl =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + Uri.EscapeDataString(clientId) +
                "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                "&response_type=code" +
                "&scope=" + Uri.EscapeDataString(scope) +
                "&access_type=offline" +
                "&prompt=consent";

            return Redirect(googleAuthUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Google authorization code not found.");
            }

            var clientId =
                _configuration["GoogleSearchConsole:ClientId"];

            var clientSecret =
                _configuration["GoogleSearchConsole:ClientSecret"];

            var redirectUri =
                _configuration["GoogleSearchConsole:RedirectUri"];

            var client = _httpClientFactory.CreateClient();

            var tokenRequest = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "redirect_uri", redirectUri! },
                { "grant_type", "authorization_code" }
            };

            var response = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest)
            );

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseContent);
            }

            var tokenResponse =
                JsonSerializer.Deserialize<GoogleTokenResponse>(
                    responseContent
                );

            if (tokenResponse == null)
            {
                return BadRequest("Unable to read Google token response.");
            }

            HttpContext.Session.SetString(
                "GoogleAccessToken",
                tokenResponse.AccessToken ?? ""
            );

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                HttpContext.Session.SetString(
                    "GoogleRefreshToken",
                    tokenResponse.RefreshToken
                );
            }

            return RedirectToAction("Index");
        }


        public class GoogleTokenResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("token_type")]
            public string? TokenType { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("scope")]
            public string? Scope { get; set; }
        }

        [HttpGet]
        public IActionResult Disconnect()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Sites()
        {
            var accessToken = HttpContext.Session.GetString(
                "GoogleAccessToken"
            );

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Connect");
            }

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    accessToken
                );

            var response = await client.GetAsync(
                "https://www.googleapis.com/webmasters/v3/sites"
            );

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseContent);
            }

            return Content(
                responseContent,
                "application/json"
            );
        }

        private async Task<SearchConsoleApiResponse> GetSearchConsoleData(
    string accessToken,
    string siteUrl,
    string startDate,
    string endDate,
    string dimension,
    int rowLimit)
        {
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken
                );

            object requestBody;

            if (string.IsNullOrEmpty(dimension))
            {
                requestBody = new
                {
                    startDate = startDate,
                    endDate = endDate,
                    type = "web",
                    rowLimit = rowLimit
                };
            }
            else
            {
                requestBody = new
                {
                    startDate = startDate,
                    endDate = endDate,
                    dimensions = new[]
                    {
                dimension
            },
                    type = "web",
                    rowLimit = rowLimit
                };
            }

            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var encodedSiteUrl = Uri.EscapeDataString(siteUrl);

            var apiUrl =
                "https://www.googleapis.com/webmasters/v3/sites/" +
                encodedSiteUrl +
                "/searchAnalytics/query";

            var response = await client.PostAsync(
                apiUrl,
                content
            );

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Search Console API Error: {responseContent}"
                );
            }

            return JsonSerializer.Deserialize<SearchConsoleApiResponse>(
                responseContent
            ) ?? new SearchConsoleApiResponse();
        }





        public void GoogleAds()
        {
            
        }
    }

}

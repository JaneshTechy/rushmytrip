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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class GoogleAdsController : Controller
    {
        // Google Ads REST API version — update this when Google sunsets old versions
        private const string API_VERSION = "v23";

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAdsController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // ==========================================
        // STEP 1: REDIRECT USER TO GOOGLE CONSENT SCREEN
        // ==========================================
        [HttpGet]
        public IActionResult Connect()
        {
            var clientId = _configuration["GoogleAds:ClientId"];
            var redirectUri = _configuration["GoogleAds:RedirectUri"];

            // IMPORTANT: Ads scope is different from Search Console scope
            var scope = "https://www.googleapis.com/auth/adwords";

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

        // ==========================================
        // STEP 2: EXCHANGE CODE FOR ACCESS + REFRESH TOKEN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Google authorization code not found.");
            }

            var clientId = _configuration["GoogleAds:ClientId"];
            var clientSecret = _configuration["GoogleAds:ClientSecret"];
            var redirectUri = _configuration["GoogleAds:RedirectUri"];

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

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseContent);
            }

            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent);

            if (tokenResponse == null)
            {
                return BadRequest("Unable to read Google token response.");
            }

            HttpContext.Session.SetString("GoogleAdsAccessToken", tokenResponse.AccessToken ?? "");

            // Refresh token only comes back the FIRST time (or with prompt=consent).
            // In production, save this to DB against the project/customer, not just Session,
            // because Session is lost when app restarts — and cron/background jobs need it.
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                HttpContext.Session.SetString("GoogleAdsRefreshToken", tokenResponse.RefreshToken);

                // TODO: persist tokenResponse.RefreshToken to DB here (per project/customer id)
            }

            return RedirectToAction("Index");
        }

        public class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }

            [JsonPropertyName("scope")]
            public string? Scope { get; set; }
        }

        // ==========================================
        // REFRESH ACCESS TOKEN USING REFRESH TOKEN
        // (access token expires every ~1 hour)
        // ==========================================
        private async Task<string> GetFreshAccessTokenAsync(string refreshToken)
        {
            var clientId = _configuration["GoogleAds:ClientId"];
            var clientSecret = _configuration["GoogleAds:ClientSecret"];

            var client = _httpClientFactory.CreateClient();

            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            var response = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest)
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to refresh Google Ads token: {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent);

            return tokenResponse?.AccessToken
                ?? throw new Exception("No access token returned on refresh.");
        }

        // ==========================================
        // CORE: RUN A GAQL QUERY AGAINST GOOGLE ADS REST API
        // ==========================================
        private async Task<GoogleAdsSearchResponse> RunGoogleAdsQuery(
            string accessToken,
            string customerId,
            string loginCustomerId,
            string developerToken,
            string gaqlQuery)
        {
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            client.DefaultRequestHeaders.Add("developer-token", developerToken);
            client.DefaultRequestHeaders.Add("login-customer-id", loginCustomerId);

            var requestBody = new { query = gaqlQuery };

            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // searchStream returns a JSON ARRAY of response batches, each with "results"
            var apiUrl =
                $"https://googleads.googleapis.com/{API_VERSION}/customers/{customerId}/googleAds:searchStream";

            var response = await client.PostAsync(apiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google Ads API Error: {responseContent}");
            }

            // searchStream returns: [ { "results": [...] }, { "results": [...] } ]
            var batches = JsonSerializer.Deserialize<List<GoogleAdsStreamBatch>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<GoogleAdsStreamBatch>();

            var merged = new GoogleAdsSearchResponse
            {
                Results = batches.SelectMany(b => b.Results ?? new List<GoogleAdsRow>()).ToList()
            };

            return merged;
        }

        // ==========================================
        // MAIN ACTION — FETCH EVERYTHING AND SAVE + DISPLAY
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accessToken = HttpContext.Session.GetString("GoogleAdsAccessToken");
            var refreshToken = HttpContext.Session.GetString("GoogleAdsRefreshToken");

            if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("Connect");
            }

            // Always refresh — access tokens are short-lived (~1hr) and this keeps it simple
            if (!string.IsNullOrEmpty(refreshToken))
            {
                accessToken = await GetFreshAccessTokenAsync(refreshToken);
                HttpContext.Session.SetString("GoogleAdsAccessToken", accessToken);
            }

            var developerToken = _configuration["GoogleAds:DeveloperToken"]!;
            var loginCustomerId = _configuration["GoogleAds:LoginCustomerId"]!.Replace("-", "");
            var customerId = _configuration["GoogleAds:CustomerId"]!.Replace("-", "");

            // ---- 1. Campaign-level summary (clicks, impressions, cost, ctr, cpc) ----
            var campaignQuery = @"
                SELECT
                    campaign.name,
                    campaign.status,
                    segments.date,
                    metrics.clicks,
                    metrics.impressions,
                    metrics.ctr,
                    metrics.average_cpc,
                    metrics.cost_micros
                FROM campaign
                WHERE segments.date DURING LAST_30_DAYS
                AND campaign.status != 'REMOVED'
                ORDER BY segments.date DESC";

            // ---- 2. Region-wise traffic ----
            var geoQuery = @"
                SELECT
                    campaign.name,
                    segments.geo_target_region,
                    metrics.clicks,
                    metrics.impressions,
                    metrics.cost_micros
                FROM geographic_view
                WHERE segments.date DURING LAST_30_DAYS";

            // ---- 3. Device-wise breakdown ----
            var deviceQuery = @"
                SELECT
                    segments.device,
                    metrics.clicks,
                    metrics.impressions,
                    metrics.cost_micros,
                    metrics.ctr
                FROM campaign
                WHERE segments.date DURING LAST_30_DAYS";

            var campaignTask = RunGoogleAdsQuery(accessToken, customerId, loginCustomerId, developerToken, campaignQuery);
            var geoTask = RunGoogleAdsQuery(accessToken, customerId, loginCustomerId, developerToken, geoQuery);
            var deviceTask = RunGoogleAdsQuery(accessToken, customerId, loginCustomerId, developerToken, deviceQuery);

            await Task.WhenAll(campaignTask, geoTask, deviceTask);

            var campaignData = await campaignTask;
            var geoData = await geoTask;
            var deviceData = await deviceTask;

            // ---- Build ViewModel ----
            var model = new GoogleAdsVM();

            model.CampaignStats = campaignData.Results.Select(r => new GoogleAdsCampaignStatVM
            {
                CampaignName = r.Campaign?.Name ?? "",
                Date = r.Segments?.Date ?? "",
                Clicks = r.Metrics?.Clicks ?? 0,
                Impressions = r.Metrics?.Impressions ?? 0,
                Ctr = (r.Metrics?.Ctr ?? 0) * 100,
                AverageCpc = (r.Metrics?.AverageCpc ?? 0) / 1_000_000.0,
                Cost = (r.Metrics?.CostMicros ?? 0) / 1_000_000.0
            }).ToList();

            model.RegionStats = geoData.Results.Select(r => new GoogleAdsRegionStatVM
            {
                CampaignName = r.Campaign?.Name ?? "",
                Region = r.Segments?.GeoTargetRegion ?? "",
                Clicks = r.Metrics?.Clicks ?? 0,
                Impressions = r.Metrics?.Impressions ?? 0,
                Cost = (r.Metrics?.CostMicros ?? 0) / 1_000_000.0
            }).ToList();

            model.DeviceStats = deviceData.Results.Select(r => new GoogleAdsDeviceStatVM
            {
                Device = r.Segments?.Device ?? "",
                Clicks = r.Metrics?.Clicks ?? 0,
                Impressions = r.Metrics?.Impressions ?? 0,
                Ctr = (r.Metrics?.Ctr ?? 0) * 100,
                Cost = (r.Metrics?.CostMicros ?? 0) / 1_000_000.0
            }).ToList();

            model.TotalClicks = model.CampaignStats.Sum(x => x.Clicks);
            model.TotalImpressions = model.CampaignStats.Sum(x => x.Impressions);
            model.TotalCost = model.CampaignStats.Sum(x => x.Cost);

            // ---- Save raw snapshot to disk (same pattern as Search Console) ----
            var completeApiResponse = new
            {
                CustomerId = customerId,
                FetchedAtUtc = DateTime.UtcNow,
                Campaigns = campaignData,
                Regions = geoData,
                Devices = deviceData
            };

            var jsonResponse = JsonSerializer.Serialize(
                completeApiResponse,
                new JsonSerializerOptions { WriteIndented = true }
            );

            var jsonFolderPath = Path.Combine(
                Directory.GetCurrentDirectory(), "App_Data", "GoogleAds");

            if (!Directory.Exists(jsonFolderPath))
            {
                Directory.CreateDirectory(jsonFolderPath);
            }

            var jsonFilePath = Path.Combine(jsonFolderPath, "GoogleAdsLatest.json");

            await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonResponse);

            return View(model);
        }

        [HttpGet]
        public IActionResult Disconnect()
        {
            HttpContext.Session.Remove("GoogleAdsAccessToken");
            HttpContext.Session.Remove("GoogleAdsRefreshToken");
            return RedirectToAction("Connect");
        }
    }

    // ==========================================
    // RAW API RESPONSE MODELS (searchStream shape)
    // ==========================================

    public class GoogleAdsStreamBatch
    {
        [JsonPropertyName("results")]
        public List<GoogleAdsRow>? Results { get; set; }
    }

    public class GoogleAdsSearchResponse
    {
        public List<GoogleAdsRow> Results { get; set; } = new List<GoogleAdsRow>();
    }

    public class GoogleAdsRow
    {
        [JsonPropertyName("campaign")]
        public GoogleAdsCampaign? Campaign { get; set; }

        [JsonPropertyName("segments")]
        public GoogleAdsSegments? Segments { get; set; }

        [JsonPropertyName("metrics")]
        public GoogleAdsMetrics? Metrics { get; set; }
    }

    public class GoogleAdsCampaign
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class GoogleAdsSegments
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("device")]
        public string? Device { get; set; }

        [JsonPropertyName("geoTargetRegion")]
        public string? GeoTargetRegion { get; set; }
    }

    public class GoogleAdsMetrics
    {
        [JsonPropertyName("clicks")]
        public long Clicks { get; set; }

        [JsonPropertyName("impressions")]
        public long Impressions { get; set; }

        [JsonPropertyName("ctr")]
        public double Ctr { get; set; }

        [JsonPropertyName("averageCpc")]
        public double AverageCpc { get; set; }

        [JsonPropertyName("costMicros")]
        public long CostMicros { get; set; }
    }
}

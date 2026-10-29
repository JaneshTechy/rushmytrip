using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TravelWeb.Models;

namespace TravelWeb.Services
{
    public class TrawickService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public TrawickService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;

        }

        public string GetToken()
        {
            try
            {
                string userName = _configuration["Trawick:UserName"];
                string password = _configuration["Trawick:Password"];
                string baseUrl = _configuration["Trawick:BaseUrl"];

                var request = new
                {
                    userName = userName,
                    passWord = password
                };

                var json = JsonConvert.SerializeObject(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
    _httpClient.PostAsync($"{baseUrl}/Account/authenticateV2", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    string error = response.Content.ReadAsStringAsync().Result;

                    throw new Exception($"Authentication Failed ({response.StatusCode}) : {error}");
                }

                string token = response.Content.ReadAsStringAsync().Result;

                token = token.Replace("\"", "");

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Trawick Authentication Error : " + ex.Message);
            }
        }
        public List<QuoteResponse> GetQuote(
     string token,
     List<object> travelers,
     DateTime startDate,
     DateTime endDate,
     string destinationCountry,
     string homeCountry)
        {
            try
            {
                string baseUrl = _configuration["Trawick:BaseUrl"];

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    travelers = travelers,
                    optionValues = new object[] { },

                    StartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    EndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),

                    DestinationCountry = destinationCountry,
                    HomeCountryCode = homeCountry,

                    TripCancellation = false,
                    StudentPlans = false,

                    productIds = new object[] { },

                    optionsOnly = false,
                    LoadOptionCost = true,
                    OnlyLoadIncludedOptions = true
                };

                var json = JsonConvert.SerializeObject(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    _httpClient.PostAsync($"{baseUrl}/Quote/quote", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    string error = response.Content.ReadAsStringAsync().Result;
                    throw new Exception($"Quote Failed ({response.StatusCode}) : {error}");
                }

                string result = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<QuoteResponse>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Trawick Quote Error : " + ex.Message);
            }
        }


        public List<ProductOptionResponse> GetProductOptions(string token, int productId)
        {
            try
            {
                string baseUrl = _configuration["Trawick:BaseUrl"];

                _httpClient.DefaultRequestHeaders.Clear();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    productId = productId
                };

                var jsonresult = JsonConvert.SerializeObject(request);

                var content = new StringContent(jsonresult, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    _httpClient.PostAsync($"{baseUrl}/Quote/productoptions", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    string error = response.Content.ReadAsStringAsync().Result;

                    throw new Exception($"Product Options Failed ({response.StatusCode}) : {error}");
                }

                string result = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<ProductOptionResponse>>(result);


            }
            catch (Exception ex)
            {
                throw new Exception("Trawick Product Options Error : " + ex.Message);
            }
        }

        public QuoteResponseDetail GetQuoteById(string token, int orderId)
        {
            try
            {
                string baseUrl = _configuration["Trawick:BaseUrl"];

                _httpClient.DefaultRequestHeaders.Clear();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    orderId = orderId
                };

                var json = JsonConvert.SerializeObject(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    _httpClient.PostAsync($"{baseUrl}/Quote/quotebyid", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    string error = response.Content.ReadAsStringAsync().Result;

                    throw new Exception($"QuoteById Failed ({response.StatusCode}) : {error}");
                }

                string result = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<QuoteResponseDetail>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Trawick QuoteById Error : " + ex.Message);
            }
        }


    }

}

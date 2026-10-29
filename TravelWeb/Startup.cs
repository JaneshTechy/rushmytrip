using FlightServices.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PayCliqMethod.Services;
using StackExchange.Redis;
using System;
using System.Net;
using System.Security.Authentication;
using TravelWeb.Datamodel;
using TravelWeb.Models;
using TravelWeb.Models.WebConfig;
using TravelWeb.Services;
using TravelWeb.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using TravelWeb.Interface;
using TravelWeb.Middleware;


namespace TravelWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //TravelWeb.Utilities.Common.WebSitePath = configuration.GetSection("AppSettings").GetSection("WebSitePath").Value?.ToString();
            TravelWeb.Utilities.Common.WebApiService = configuration.GetSection("AppSettings").GetSection("WebApiService").Value?.ToString();
            TravelWeb.Utilities.Common.ConnectionString = configuration.GetSection("AppSettings").GetSection("ConnectionString").Value?.ToString();

            TravelWeb.Utilities.Common.WebSiteDomain = configuration.GetSection("AppSettings").GetSection("WebSiteDomain").Value?.ToString();
            TravelWeb.Utilities.Common.EmailAddress = configuration.GetSection("AppSettings").GetSection("EmailAddress").Value?.ToString();
            TravelWeb.Utilities.Common.Password = configuration.GetSection("AppSettings").GetSection("Password").Value?.ToString();
            TravelWeb.Utilities.Common.Host = configuration.GetSection("AppSettings").GetSection("Host").Value?.ToString();
            TravelWeb.Utilities.Common.BusinessMailTo = configuration.GetSection("AppSettings").GetSection("BusinessMailTo").Value?.ToString();
            TravelWeb.Utilities.Common.BccMail = configuration.GetSection("AppSettings").GetSection("BccMail").Value?.ToString();

            // added by ritu 06Feb2021
            TravelWeb.Utilities.Common.TagLine = configuration.GetSection("AppSettings").GetSection("TagLine").Value?.ToString();
            TravelWeb.Utilities.Common.Destination = configuration.GetSection("AppSettings").GetSection("Destination").Value?.ToString();
            TravelWeb.Utilities.Common.BaggageLine = configuration.GetSection("AppSettings").GetSection("BaggageLine").Value?.ToString();
            TravelWeb.Utilities.Common.BaggageAllownce = configuration.GetSection("AppSettings").GetSection("BaggageAllownce").Value?.ToString();
            TravelWeb.Utilities.Common.MainCabinAllowed = configuration.GetSection("AppSettings").GetSection("MainCabinAllowed").Value?.ToString();
            TravelWeb.Utilities.Common.SeatPrefrenceAmount = configuration.GetSection("AppSettings").GetSection("SeatPrefrenceAmount").Value?.ToString();
        }

        public IConfiguration Configuration { get; }

        // Add services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default30", new CacheProfile { Duration = 30 });
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // 🔐 Redirect HTTP -> HTTPS (set your Kestrel HTTPS port here)
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 7070; // must match the HTTPS port you configured in Program.cs / launchSettings.json
            });

            //services.AddDistributedMemoryCache();
            
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.IsEssential = true;
                option.Cookie.SameSite = SameSiteMode.None;                // requires HTTPS
                option.Cookie.SecurePolicy = CookieSecurePolicy.Always;    // forces HTTPS
            });

            services.ConfigureApplicationCookie(o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                o.SlidingExpiration = true;
            });
            services.AddDbContext<RSFLY>(options =>
    options.UseSqlServer(Configuration.GetSection("AppSettings:ConnectionString").Value));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration =
            //        "redis-15610.c10.us-east-1-2.ec2.cloud.redislabs.com:15610," +
            //        "password=rDQMKmAZrLI8tdMHaQn0fIYmKCA6ENrr," +
            //        "ssl=True,abortConnect=False";

            //    options.InstanceName = "TravelWeb:";
            //});

            // ✅ Redis wrapper should be SCOPED

            services.Configure<PaymentSettings>(Configuration.GetSection("PaymentSettings"));

            services.AddControllersWithViews();
            services.AddConfiguration<Appsettings>(Configuration, "AppSettings");
            services.AddHttpContextAccessor();
            services.AddScoped<DeepLinkTrackerService>();
            services.AddScoped<FlightSearchService>();
            services.AddScoped<HotelSearchService>();
            services.AddScoped<FlightVerifierService>();

            services.AddScoped<ISeoService, SeoService>();
            services.AddHttpClient<ICardflexPaymentService, CardflexPaymentService>((provider, client) =>
            {
                var cfg = provider.GetRequiredService<IOptions<CliqPaymentConfig>>().Value;

                client.Timeout = TimeSpan.FromSeconds(cfg.TimeoutSeconds);
            });

            services.AddDbContext<RSFLY>(options =>
    options.UseSqlServer(
        Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }));

        }

        // Configure pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Common.Configure(
                app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(),
                app.ApplicationServices.GetRequiredService<IDistributedCache>(),
                env.WebRootPath);

            // Prefetch airport list so home autocomplete is fast on first keystroke
            try
            {
                Common.WarmAirportCache();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Airport cache warm-up failed: " + ex.Message);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // 🛡️ HSTS only outside Development
                app.UseExceptionHandler("/Home/Index");
                app.UseHsts();
            }

            // 🚀 Redirect early so static files & cookies go over HTTPS
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession();
            app.UseMiddleware<LocaleMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "home",
                    pattern: "",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
    name: "pages",
    pattern: "{slug:regex(^[^\\.]+$)}",
    defaults: new { controller = "Home", action = "Page" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });
        }
    }
}

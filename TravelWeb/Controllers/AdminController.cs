using AngleSharp.Dom;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Web.LayoutRenderers;
using RazorLight;
using SendGrid.Helpers.Mail;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using TravelWeb.Areas.Admin.Models;
using TravelWeb.Datamodel;
using TravelWeb.Helper;
using TravelWeb.Helpers;
using TravelWeb.Interface;
using TravelWeb.Models;

using TravelWeb.Utilities;
using static TravelWeb.Models.MetaResponseDto;

namespace TravelWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly RSFLY _context;
        private string ApiFilePath;
        private readonly ISeoService _seoService;
        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _serviceProvider;
        private readonly RazorLightEngine _engine;
        public readonly ILogger<DetailsController> _logger;

        public async Task<string> RenderAsync<T>(string viewPath, T model)
        {
            return await _engine.CompileRenderAsync(viewPath, model);
        }

        public AdminController(RSFLY context, ILogger<DetailsController> logger, ISeoService seoService, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _seoService = seoService;
            _env = env;
            _serviceProvider = serviceProvider;
            _engine = new RazorLightEngineBuilder().UseFileSystemProject(Path.Combine(env.ContentRootPath, "EmailTemplates")).UseMemoryCachingProvider().Build();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        //Login Related Methods
        //-------------------

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Dashboard");

            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            ViewBag.UserIP = ipAddress;

            return View();
        }

        //public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        //{
        //    var user = _context.AdminUsers.FirstOrDefault(u => u.UserEmail == email);

        //    if (user != null && PasswordHelper.Verify(password, user.PasswordHash))
        //    {
        //        if (user.IsLocked == true || user.IsActive == false)
        //        {
        //            Notify("Account is locked or disabled. Contact system admin.", "error");
        //            return View();
        //        }
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Email, user.UserEmail),
        //    new Claim(ClaimTypes.Role, user.RoleId.ToString()),
        //    new Claim("UserId", user.UserId.ToString())
        //};

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            new ClaimsPrincipal(claimsIdentity));
        //        user.LastLoginAt = DateTime.Now;
        //        _context.SaveChanges();

        //        Notify($"Welcome back, {user.UserName}!", "success");
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            return Redirect(returnUrl);
        //        }

        //        return RedirectToAction("Dashboard");
        //    }

        //    Notify("Invalid email or password. Please Contact To Admin.", "error");
        //    return View();
        //}

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            var user = _context.AdminUsers.FirstOrDefault(u => u.UserEmail == email);
            var RoleId = user.RoleId;
            if (user == null)
            {
                Notify("Invalid email or password. Please contact admin.", "error");
                return View();
            }

            if (!PasswordHelper.Verify(password, user.PasswordHash))
            {
                Notify("Invalid email or password. Please contact admin.", "error");
                return View();
            }

            if (user.IsLocked == true || user.IsActive == false)
            {
                Notify("Account is locked or disabled. Contact system admin.", "error");
                return View();
            }

            // 🔐 Get Current IP
            var currentIP = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(currentIP))
            {
                currentIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            // Normalize (in case proxy sends multiple IPs)
            if (!string.IsNullOrEmpty(currentIP) && currentIP.Contains(","))
            {
                currentIP = currentIP.Split(',')[0].Trim();
            }

            var allowedIPsRaw = user.AllowedIpaddress;

            bool isLocalhost = currentIP == "::1" || currentIP == "127.0.0.1" || RoleId == 3|| RoleId ==2;
            bool isDevelopment = HttpContext.Request.Host.Host.Contains("localhost");

            bool isAllowed = false;

            if (isDevelopment || isLocalhost)
            {
                isAllowed = true;
            }
            else if (!string.IsNullOrEmpty(allowedIPsRaw))
            {
                // 🔥 MULTIPLE IP SUPPORT
                var allowedIPs = allowedIPsRaw
                                    .Split(',')
                                    .Select(ip => ip.Trim())
                                    .ToList();

                if (allowedIPs.Contains(currentIP))
                {
                    isAllowed = true;
                }
            }

            if (!isAllowed)
            {
                Notify("Your credentials are correct, but your IP address is not authorized. Please contact the administrator.", "warning");
                return View();
            }

            // ✅ LOGIN PROCESS (unchanged)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.UserEmail),
        new Claim(ClaimTypes.Role, user.RoleId.ToString()),
        new Claim("UserId", user.UserId.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            user.LastLoginAt = DateTime.Now;
            _context.SaveChanges();

            Notify($"Welcome back, {user.UserName}!", "success");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        //Create User Funtionality Methods
        //-------------------


        public async Task<IActionResult> CreateUser(int? editId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Admin");
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Notify("Unauthorized. Please login again.", "error");
                    return RedirectToAction("Login", "Admin");
                }

                int currentUserId = int.Parse(userIdClaim);

                var currentUser = await _context.AdminUsers
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => new { u.UserId, u.RoleId })
                    .FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    Notify("User not found.", "error");
                    return RedirectToAction("Login", "Admin");
                }

                if (currentUser.RoleId != 1 && currentUser.RoleId != 2)
                {
                    Notify("You do not have permission to access this page.", "error");
                    return RedirectToAction("Dashboard");
                }

                var pageVm = new AdminUserPageVM();

                var rolesQuery = _context.AdminUserRoles.Where(r => r.IsActive == true);

                if (currentUser.RoleId == 2)
                {
                    rolesQuery = rolesQuery.Where(r => r.RoleId != 1);
                }

                ViewBag.Roles = new SelectList(
                    rolesQuery,
                    "RoleId",
                    "RoleName"
                );
                pageVm.Users = _context.AdminUsers
                    .Join(_context.AdminUserRoles,
                        u => u.RoleId,
                        r => r.RoleId,
                        (u, r) => new AdminUserVM
                        {
                            UserId = u.UserId,
                            UserName = u.UserName,
                            UserEmail = u.UserEmail,
                            UserPhone = u.UserPhone,
                            AllowedIPAddress = u.AllowedIpaddress,
                            RoleId = u.RoleId,
                            RoleName = r.RoleName,
                            LastLoginAt = u.LastLoginAt,
                            IsActive = u.IsActive ?? true,
                            IsLocked = u.IsLocked
                        }).ToList();

                if (editId == null)
                {
                    pageVm.FormUser = new AdminUserVM();
                    return View(pageVm);
                }

                var user = await _context.AdminUsers.FindAsync(editId);
                if (user == null)
                {
                    Notify("User not found.", "error");
                    return RedirectToAction("Dashboard");
                }

                if (user.RoleId == 1 && currentUser.RoleId != 1)
                {
                    Notify("Only Super Admin can modify another Super Admin.", "error");
                    return RedirectToAction("Dashboard");
                }
                pageVm.FormUser = new AdminUserVM
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    AllowedIPAddress = user.AllowedIpaddress,
                    RoleId = user.RoleId,
                    IsActive = user.IsActive ?? true,
                    IsLocked = user.IsLocked
                };

                return View(pageVm);
            }
            catch
            {
                Notify("An error occurred while loading the page.", "error");
                return RedirectToAction("Dashboard");
            }
        }
        [HttpPost]
        public IActionResult Save(AdminUserPageVM model)
        {
            try
            {
                var vm = model.FormUser;

                if (!ModelState.IsValid)
                {
                    Notify("Fill All the Details!", "Error");
                    return RedirectToAction(nameof(CreateUser));
                }

                if (vm.UserId == 0)
                {
                    var user = new AdminUser
                    {
                        UserName = vm.UserName,
                        UserEmail = vm.UserEmail,
                        PasswordHash = PasswordHelper.Hash(vm.Password),
                        UserPhone = vm.UserPhone,
                        AllowedIpaddress = vm.AllowedIPAddress,
                        RoleId = vm.RoleId,
                        IsActive = true,
                        IsLocked = false,
                        UserCreatedAt = DateTime.Now,
                        UserUpdatedAt = DateTime.Now
                    };
                    _context.AdminUsers.Add(user);
                }
                else
                {
                    var user = _context.AdminUsers.Find(vm.UserId);
                    if (user == null) return RedirectToAction(nameof(CreateUser));

                    user.UserName = vm.UserName;
                    user.UserEmail = vm.UserEmail;
                    user.UserPhone = vm.UserPhone;
                    user.AllowedIpaddress = vm.AllowedIPAddress;
                    user.RoleId = vm.RoleId;
                    user.IsActive = vm.IsActive;
                    user.IsLocked = vm.IsLocked;
                    user.UserUpdatedAt = DateTime.Now;

                    if (!string.IsNullOrEmpty(vm.Password))
                        user.PasswordHash = PasswordHelper.Hash(vm.Password);
                }

                _context.SaveChanges();
                Notify("User information has been saved successfully!", "success");
                return RedirectToAction("CreateUser");
            }
            catch (Exception ex)
            {
                Notify("Error: Could not save user. " + ex.Message, "error");
                return RedirectToAction("CreateUser");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _context.AdminUsers.Find(id);
                if (user != null)
                {
                    _context.AdminUsers.Remove(user);
                    _context.SaveChanges();
                }
                Notify("The user has been permanently removed.", "warning");
                return RedirectToAction("CreateUser");
            }
            catch (Exception ex)
            {
                Notify("Error: Could not save user. " + ex.Message, "error");
                return RedirectToAction("CreateUser");
            }
        }
        public IActionResult ToggleActive(int id)
        {
            var user = _context.AdminUsers.Find(id);
            if (user != null)
            {
                user.IsActive = !(user.IsActive ?? true);
                _context.SaveChanges();

                string status = (user.IsActive ?? true) ? "enabled" : "disabled";
                Notify($"User {user.UserName} has been {status}.", "success");
            }
            return RedirectToAction(nameof(CreateUser));
        }
        public IActionResult ToggleLock(int id)
        {
            var user = _context.AdminUsers.Find(id);
            if (user != null)
            {
                user.IsLocked = !user.IsLocked;
                _context.SaveChanges();

                // ADD THIS
                string lockStatus = user.IsLocked ? "locked" : "unlocked";
                Notify($"User account {lockStatus} successfully.", "warning");
            }
            return RedirectToAction(nameof(CreateUser));
        }

        //public async Task<IActionResult> TrackerDetails(string website = "rushmyflight.com", int pageNumber = 1, int pageSize = 100)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //        return RedirectToAction("Login", "Admin");

        //    var query = _context.DeepLinkTrackers.AsQueryable();


        //    var totalRecords = await query.CountAsync();

        //    var trackers = await query
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(x => new TrackerVM
        //        {
        //            Id = x.Id,
        //            IPAddress = x.Ipaddress,
        //            RootSource = x.RootSource,
        //            From = x.FromLocation,
        //            To = x.ToLocation,
        //            Airline = x.Airline,
        //            SubClass = x.SubClass,
        //            Depart = x.DepartureDate,
        //            Return = x.ReturnDate,
        //            TripType = x.TripType == "R" ? "ROUND" : "ONEWAY",
        //            ADT = x.Adults,
        //            CHD = x.Children,
        //            INF = x.Infants,
        //            CreatedDate = x.CreatedAt,
        //            TotalFare = x.PriceWithoutMarkup,
        //            Markup = x.Markup,
        //            TotalAfterMarkup = x.TotalAmount,
        //            Rule = x.Rule
        //        })
        //        .ToListAsync();

        //    return View(new TrackerPageVM
        //    {
        //        Trackers = trackers,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalRecords = totalRecords,
        //        Website = website
        //    });
        //}

        //    public async Task<IActionResult> TrackerDetails(
        //string filter = "U",   // U = Searches, C = Clicks
        //int pageNumber = 1,
        //int pageSize = 100)
        //    {
        //        if (!User.Identity.IsAuthenticated)
        //            return RedirectToAction("Login", "Admin");

        //        var query = _context.DeepLinkTrackers
        //                            .AsNoTracking()   // ⚡ Performance boost
        //                            .AsQueryable();

        //        // 🔥 Filter Logic (Same as Old SP)
        //        if (filter == "U") // Searches
        //        {
        //            query = query.Where(x => x.TotalAmount == null || x.TotalAmount == 0);
        //        }
        //        else if (filter == "C") // Clicks
        //        {
        //            query = query.Where(x => x.TotalAmount != null && x.TotalAmount > 0);
        //        }

        //        var totalRecords = await query.CountAsync();

        //        var trackers = await query
        //            .OrderByDescending(x => x.Id)
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(x => new TrackerVM
        //            {
        //                Id = x.Id,
        //                IPAddress = x.Ipaddress,
        //                RootSource = x.RootSource,
        //                From = x.FromLocation,
        //                To = x.ToLocation,
        //                Airline = x.Airline,
        //                SubClass = x.SubClass,
        //                Depart = x.DepartureDate,
        //                Return = x.ReturnDate,
        //                TripType = string.Equals(x.TripType, "R", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(x.TripType, "ROUND", StringComparison.OrdinalIgnoreCase)
        //? "ROUND"
        //: "ONEWAY",
        //                ADT = x.Adults,
        //                CHD = x.Children,
        //                INF = x.Infants,
        //                CreatedDate = x.CreatedAt,
        //                TotalFare = x.PriceWithoutMarkup,
        //                Markup = x.Markup,
        //                TotalAfterMarkup = x.TotalAmount,
        //                Rule = x.Rule
        //            })
        //            .ToListAsync();

        //        return View(new TrackerPageVM
        //        {
        //            Trackers = trackers,
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,
        //            TotalRecords = totalRecords,
        //            CurrentFilter = filter
        //        });
        //    }

        public async Task<IActionResult> TrackerDetails(
    string filter = "U",   // U = Searches, C = Clicks
    int pageNumber = 1,
    int pageSize = 100,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string rootSourceFilter = null,
    string fromLocationFilter = null,
    string toLocationFilter = null,
    string tripTypeFilter = null)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Admin");

            var query = _context.DeepLinkTrackers
                                .AsNoTracking()
                                .AsQueryable();

            // 🔥 Filter Logic (Same as Old SP)
            if (filter == "U") // Searches
            {
                query = query.Where(x => x.TotalAmount == null || x.TotalAmount == 0);
            }
            else if (filter == "C") // Clicks
            {
                query = query.Where(x => x.TotalAmount != null && x.TotalAmount > 0);
            }

            // 🔥 Date range filter (CreatedAt pr, aap DepartureDate pr bhi kar sakte ho agar wahi chahiye)
            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedAt <= endOfDay);
            }

            // 🔥 RootSource filter (FC / Trabber / FS)
            if (!string.IsNullOrWhiteSpace(rootSourceFilter))
            {
                query = query.Where(x => x.RootSource == rootSourceFilter);
            }

            // 🔥 From / To location filter
            if (!string.IsNullOrWhiteSpace(fromLocationFilter))
            {
                query = query.Where(x => x.FromLocation.Contains(fromLocationFilter));
            }
            if (!string.IsNullOrWhiteSpace(toLocationFilter))
            {
                query = query.Where(x => x.ToLocation.Contains(toLocationFilter));
            }

            // 🔥 TripType filter (ROUND / ONEWAY)
            if (!string.IsNullOrWhiteSpace(tripTypeFilter))
            {
                if (tripTypeFilter == "ROUND")
                {
                    query = query.Where(x =>
                        x.TripType == "R" || x.TripType.ToUpper() == "ROUND");
                }
                else if (tripTypeFilter == "ONEWAY")
                {
                    query = query.Where(x =>
                        x.TripType != "R" && x.TripType.ToUpper() != "ROUND");
                }
            }

            var totalRecords = await query.CountAsync();

            var trackers = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TrackerVM
                {
                    Id = x.Id,
                    IPAddress = x.Ipaddress,
                    RootSource = x.RootSource,
                    From = x.FromLocation,
                    To = x.ToLocation,
                    Airline = x.Airline,
                    SubClass = x.SubClass,
                    Depart = x.DepartureDate,
                    Return = x.ReturnDate,
                    TripType = string.Equals(x.TripType, "R", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(x.TripType, "ROUND", StringComparison.OrdinalIgnoreCase)
                               ? "ROUND"
                               : "ONEWAY",
                    ADT = x.Adults,
                    CHD = x.Children,
                    INF = x.Infants,
                    CreatedDate = x.CreatedAt,
                    TotalFare = x.PriceWithoutMarkup,
                    Markup = x.Markup,
                    TotalAfterMarkup = x.TotalAmount,
                    Rule = x.Rule
                })
                .ToListAsync();

            return View(new TrackerPageVM
            {
                Trackers = trackers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                CurrentFilter = filter,
                FromDate = fromDate,
                ToDate = toDate,
                RootSourceFilter = rootSourceFilter,
                FromLocationFilter = fromLocationFilter,
                ToLocationFilter = toLocationFilter,
                TripTypeFilter = tripTypeFilter
            });
        }


        public async Task<IActionResult> TrackerView(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var tracker = await _context.DeepLinkTrackers
                                        .AsNoTracking()
                                        .Where(x => x.Id == id)
                                        .Select(x => new TrackerDetailVM
                                        {
                                            Id = x.Id,
                                            SearchId = x.SearchId,
                                            ParentSearchId = x.ParentSearchId,
                                            Source = x.Source,
                                            RootSource = x.RootSource,
                                            Stage = x.Stage,
                                            PreviousStage = x.PreviousStage,
                                            FromLocation = x.FromLocation,
                                            ToLocation = x.ToLocation,
                                            DepartureDate = x.DepartureDate,
                                            ReturnDate = x.ReturnDate,
                                            TripType = x.TripType == "R" ? "ROUND" : "ONEWAY",
                                            Adults = x.Adults,
                                            Children = x.Children,
                                            Infants = x.Infants,
                                            CabinClass = x.CabinClass,
                                            IPAddress = x.Ipaddress,
                                            UserAgent = x.UserAgent,
                                            BookingReference = x.BookingReference,
                                            CreatedAt = x.CreatedAt,
                                            UpdatedAt = x.UpdatedAt,
                                            PriceWithoutMarkup = x.PriceWithoutMarkup,
                                            Markup = x.Markup,
                                            Rule = x.Rule,
                                            Airline = x.Airline,
                                            SubClass = x.SubClass,
                                            ModifiedBy = x.ModifiedBy,
                                            SubDetailStage = x.SubDetailStage,
                                            TotalAmount = x.TotalAmount
                                        })
                                        .FirstOrDefaultAsync();

            if (tracker == null)
                return NotFound();

            return View(tracker);
        }

        //Booking Related Methods
        //-------------------

        //public async Task<IActionResult> BookingDeatils(int pageNumber = 1, int pageSize = 10)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Admin");
        //    }
        //    var totalRecords = await _context.FlightBookings.CountAsync();

        //    var bookings = await _context.FlightBookings
        //        .OrderByDescending(x => x.BookingDate)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(f => new FlightBookingVM
        //        {
        //            BookingId = f.BookingId,
        //            BookingNumber = f.BookingNumber,
        //            BookingDate = f.BookingDate,
        //            FromDeparture = f.FromDeparture,
        //            ToDestination = f.ToDestination,
        //            DepartDate = f.DepartDate,
        //            ReturnDate = f.ReturnDate,
        //            FlightWay = f.FlightWay,
        //            Airline = f.Airline,
        //            FlightClass = f.FlightClass,
        //            TotalPassesngers = f.Adult + f.Child + f.Infant,
        //            Status = f.Status
        //        }).ToListAsync();

        //    return View(new BookingPageVM
        //    {
        //        FlightBookings = bookings,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalRecords = totalRecords
        //    });
        //}
        public async Task<IActionResult> BookingDeatils(
    int pageNumber = 1,
    int pageSize = 50,
    string bookingNumber = "")
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Admin");
                }

                // 🔹 Get current user ID from cookie claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return RedirectToAction("Login", "Admin");

                int currentUserId = int.Parse(userIdClaim);

                // 🔹 Get current user's role from DB
                var currentUser = await _context.AdminUsers
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => new { u.UserId, u.RoleId })
                    .FirstOrDefaultAsync();

                if (currentUser == null)
                    return RedirectToAction("Login", "Admin");

                // 🔹 Base query
                var query = _context.FlightBookings.Where(f => f.DelInd != true).AsQueryable();

                // 🔹 Filter for Sales Agent
                if (currentUser.RoleId == 4) // 4 = Sales Agent
                {
                    query = query.Where(f => f.AgentId == currentUserId);
                }
                if (!string.IsNullOrWhiteSpace(bookingNumber))
                {
                    query = query.Where(f =>
                        (f.BookingNumber != null && f.BookingNumber.Contains(bookingNumber)) ||
                        (f.ContactPerson != null && f.ContactPerson.Contains(bookingNumber)) ||
                        (f.PaxFullName != null && f.PaxFullName.Contains(bookingNumber)||
                        (f.ContactEmail != null && f.ContactEmail.Contains(bookingNumber)))
                    );
                }
                ViewBag.BookingNumber = bookingNumber;
                var totalRecords = await query.CountAsync();

                var bookings = await query
                    .OrderByDescending(f => f.BookingDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => new FlightBookingVM
                    {
                        BookingId = f.BookingId,
                        BookingNumber = f.BookingNumber,
                        BookingDate = f.BookingDate,
                        FromDeparture = f.FromDeparture,
                        ToDestination = f.ToDestination,
                        DepartDate = f.DepartDate,
                        ReturnDate = f.ReturnDate,
                        FlightWay = f.FlightWay,
                        OutboundAirline = ExtractAirline(f.FlightDetailsOutbound),
                        InboundAirline = ExtractAirline(f.FlightDetailsInbound),
                        FlightStatus = f.Status ?? "Pending",
                        Airline = f.Airline,
                        FlightClass = f.FlightClass,
                        TotalPassesngers = f.Adult + f.Child + f.Infant,
                        Status = f.IsStatus,
                        PaxFullName = f.PaxFullName,

                        Agent = _context.AdminUsers
                                    .Where(u => u.UserId == f.AgentId)
                                    .Select(u => u.UserName)
                                    .FirstOrDefault()
                    })
                    .ToListAsync();

                return View(new BookingPageVM
                {
                    FlightBookings = bookings,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static string ExtractAirline(string flightDetails)
        {
            if (string.IsNullOrEmpty(flightDetails))
                return "N/A";

            var part = flightDetails
                .Split(',')
                .FirstOrDefault(x => x.Trim().StartsWith("Airline ="));

            return part != null
                ? part.Replace("Airline =", "").Trim()
                : "N/A";
        }
        [HttpGet]
        public async Task<IActionResult> GetPassengers(int bookingId)
        {
            var data = await _context.BookingPaxDetails
                .Where(p => p.BookingId == bookingId)
                .Select(p => new
                {
                    p.PaxId,
                    p.PaxTitle,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
                    p.PaxType,
                    p.Gender,
                    p.Dob
                })
                .ToListAsync();

            var orderedData = data
                .OrderBy(x => x.PaxType == "Adult" ? 1 :
                              x.PaxType == "Child" ? 2 : 3)
                .ThenBy(x =>
                {
                    var numberPart = x.PaxTitle?.Split(' ').LastOrDefault();
                    return int.TryParse(numberPart, out int n) ? n : 999;
                })
                .ToList();

            return Json(orderedData);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetSegments(int bookingId)
        {
            var segments = await _context.BookingFlghtDetails
                .Where(f => f.BookingId == bookingId)
                .OrderBy(f => f.DepartDate)
                .Select(f => new
                {
                    f.FlightNo,
                    f.DepDate,
                    f.ArrDate,
                    f.Dcode,
                    f.Gcode,
                    f.Airline,
                    f.DepartOn,
                    f.ArriveOn,
                    f.FlightType,
                    f.BoundClass
                })
                .ToListAsync();

            var outbound = segments.Where(x => x.FlightType == "OutBound").ToList();
            var inbound = segments.Where(x => x.FlightType == "InBound").ToList();

            return Json(new
            {
                OutboundStopCount = outbound.Count <= 1 ? 0 : outbound.Count - 1,
                InboundStopCount = inbound.Count <= 1 ? 0 : inbound.Count - 1,
                OutboundSegments = outbound,
                InboundSegments = inbound
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetCardDetails(long bookingId)
        {

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                Notify("Unauthorized. Please login.", "error");
                return RedirectToAction("Login", "Admin");
            }

            int currentUserId = int.Parse(userIdClaim);

            // 🔹 Get current user's role
            var currentUser = await _context.AdminUsers
                .Where(u => u.UserId == currentUserId)
                .Select(u => new { u.UserId, u.RoleId })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                Notify("User not found.", "error");
                return RedirectToAction("Login", "Admin");
            }


            // 🔹 Only Admin (2) and Super Admin (1) allowed
            if (currentUser.RoleId != 1 && currentUser.RoleId != 2 && currentUser.RoleId != 3&& currentUser.RoleId != 4)
            {
                Notify("You do not have permission to access card details.", "error");
                return RedirectToAction("BookingDeatils"); // redirect back to booking list
            }

            // 🔹 Get card details
            var address = await _context.FlightBookings
                .Where(x => x.BookingId == bookingId)
                .Select(x => new
                {
                    x.BillAddress1,
                    x.BillAddress2,
                    x.BillCity,
                    x.BillState,
                    x.BillPostalCode,
                    x.BillCountry
                })
                .FirstOrDefaultAsync();

            //var cards = await _context.BookingsCardDetails
            //    .Where(c => c.BookingsId == bookingId)
            //    .Select(c => new
            //    {
            //        CardName = c.FirstName + " " + c.LastName,
            //        c.CardType,
            //        c.Cvc,
            //        c.CardNumber,
            //        c.ExpiryDate,
            //        c.ExpiryYear
            //    })
            //    .ToListAsync();

            var cardsRaw = await _context.BookingsCardDetails.Where(c => c.BookingsId == bookingId).ToListAsync();

            var cards = cardsRaw.Select(c => new
            {
                CardName = c.FirstName + " " + c.LastName,
                c.CardType,
                CardNumber = (currentUser.RoleId == 1 || currentUser.RoleId == 2)
        ? c.CardNumber
        : MaskCard(c.CardNumber),
                c.ExpiryDate,
                c.ExpiryYear,
                c.Cvc

            }).ToList();
            // 🔹 Send to view (or as JSON if you still want ajax rendering)
            return Json(new
            {
                success = true,
                data = new { address, cards }
            });
        }
        private static string MaskCard(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Length < 4)
                return "****";

            return "**** **** **** " + number[^4..];
        }
        public async Task<IActionResult> GetPriceDetails(long bookingId)
        {
            var data = await _context.FlightBookings
                .Where(x => x.BookingId == bookingId)
                .Select(x => new
                {
                    x.Adult,
                    x.Child,
                    x.Infant,
                    x.AdultFare,
                    x.AdutlTax,
                    x.AdultMarkup,
                    x.ChildFare,
                    x.ChildTax,
                    x.ChildMarkup,
                    x.InfantFare,
                    x.InfantTax,
                    x.InfantMarkup,
                    x.Nett,
                })
                .FirstOrDefaultAsync();

            return Json(data);
        }
        public async Task<IActionResult> GetContactDetails(long bookingId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Admin");

            int currentUserId = int.Parse(userIdClaim);

            // Get current user's role from DB
            var currentUser = await _context.AdminUsers
                .Where(u => u.UserId == currentUserId)
                .Select(u => new { u.UserId, u.RoleId, u.UserName })
                .FirstOrDefaultAsync();

            if (currentUser == null)
                return RedirectToAction("Login", "Admin");

            // Base query
            var query = _context.FlightBookings.AsQueryable();

            // Restrict Sales Agent to only their bookings
            //if (currentUser.RoleId == 4) // 4 = Sales Agent
            //{
            //    query = query.Where(f => f.AgentId == currentUserId);
            //}

            // Fetch booking data
            var data = await query
                .Where(x => x.BookingId == bookingId)
                .Select(x => new
                {
                    x.ContactPerson,
                    x.ContactEmail,
                    x.PaxFullName,
                    x.Comments,
                    Agent = currentUser.UserName // assign the current agent's name
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return Json(new { success = false, message = "Booking not found or access denied." });
            }

            return Json(new { success = true, data });
        }
        [HttpGet]
        public async Task<IActionResult> AssignBookings(int pageNumber = 1, int pageSize = 10)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Admin");
            }

            // 🔹 Get current user's ID from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                Notify("Unauthorized. Please login.", "error");
                return RedirectToAction("Login", "Admin");
            }

            int currentUserId = int.Parse(userIdClaim);

            // 🔹 Get current user's role
            var currentUser = await _context.AdminUsers
                .Where(u => u.UserId == currentUserId)
                .Select(u => new { u.UserId, u.RoleId })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                Notify("User not found.", "error");
                return RedirectToAction("Login", "Admin");
            }

            // 🔹 Only Admin (RoleId = 2) or Super Admin (RoleId = 1) allowed
            if (currentUser.RoleId != 1 && currentUser.RoleId != 2 && currentUser.RoleId != 3)
            {
                Notify("You do not have permission to access this page.", "error");
                return RedirectToAction("Dashboard");
            }

            var totalRecords = await _context.FlightBookings.CountAsync();

            // 🔹 Get bookings with assigned agent name
            var list = await _context.FlightBookings
                .OrderByDescending(x => x.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AssignBookingVM
                {
                    BookingId = x.BookingId,
                    BookingNumber = x.BookingNumber,
                    BookingDate = x.BookingDate,
                    FromDeparture = x.FromDeparture,
                    ToDestination = x.ToDestination,
                    Status = x.Status,
                    AgentId = x.AgentId,
                    AgentName = _context.AdminUsers
                        .Where(u => u.UserId == x.AgentId)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // 🔹 Get active roles dynamically
            var roles = await _context.AdminUserRoles
                .Where(r => r.IsActive == true && r.RoleId != 1 && r.RoleId != 2)
                .Select(r => new RoleVM
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();

            return View(new AssignBookingPageVM
            {
                Bookings = list,
                Roles = roles,          // 👈 send roles to view
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            });
        }
        // Assign Bookings to the Users
        [HttpGet]
        public async Task<IActionResult> GetAssignableUsers(int roleId)
        {
            var users = await _context.AdminUsers
                .Where(x => x.RoleId == roleId && x.IsActive == true && x.IsLocked == false)
                .Select(x => new
                {
                    x.UserId,
                    x.UserName,
                    x.UserEmail
                })
                .ToListAsync();

            return Json(users);
        }
        [HttpPost]
        public async Task<IActionResult> AssignBooking(long bookingId, int userId)
        {
            var booking = await _context.FlightBookings
                .FirstOrDefaultAsync(x => x.BookingId == bookingId);

            if (booking == null)
                return Json(new { success = false, message = "Booking not found" });

            booking.AgentId = userId;
            booking.AssignDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Booking assigned successfully" });
        }


        // Popup Code
        //-------------------

        private void Notify(string message, string type = "success")
        {
            TempData.Remove("Notification");
            TempData["Notification"] = Newtonsoft.Json.JsonConvert.SerializeObject(
                new { Message = message, Type = type }
            );
        }


        public IActionResult APiResponseFlight()
        {
            var list = new List<FlightApiCacheVM>();

            var root = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "api-cache"
            );

            if (!Directory.Exists(root))
                return View(list);

            var files = Directory.GetFiles(root, "*.json");
            var fromTime = DateTime.Now.AddHours(-2);

            foreach (var file in files)
            {
                var info = new FileInfo(file);

                if (info.LastWriteTime < fromTime || info.LastWriteTime.Date != DateTime.Today)
                    continue;

                var name = Path.GetFileNameWithoutExtension(file);
                var parts = name.Split('_');

                // bos_jfk_20260207_ow_7_0_0_1
                var from = parts[0].ToUpper();
                var to = parts[1].ToUpper();

                bool isOneWay = name.Contains("_ow_");

                string depDate = DateTime.ParseExact(parts[2], "yyyyMMdd", null)
                                 .ToString("yyyy-MM-dd");

                string travelDates = isOneWay
            ? depDate
            : depDate + " → " +
              DateTime.ParseExact(parts[3], "yyyyMMdd", null)
                      .ToString("yyyy-MM-dd");

                int pax =
                    int.Parse(parts[^4]) +
                    int.Parse(parts[^3]) +
                    int.Parse(parts[^2]);

                list.Add(new FlightApiCacheVM
                {
                    FileName = name,
                    Route = $"{from} → {to}",
                    JourneyType = isOneWay ? "One Way" : "Round Trip",
                    TravelDates = travelDates,
                    Pax = pax,
                    SearchTime = info.LastWriteTime
                });
            }

            list = list
                .OrderByDescending(x => x.SearchTime)
                .ToList();

            return View(list);
        }


        public IActionResult ViewFlightApiResponse(string file, FlightFilterVM filter)
        {
            ApiFilePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "api-cache",
        file + ".json"
    );
            var allFlights = LoadFlightsFromFile();

            PrepareDropdowns(allFlights, filter);

            int totalRecords = allFlights.Count;

            var pagedData = allFlights
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToList();

            // 🔹 ViewBag for pagination
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageNumber = filter.PageNumber;
            ViewBag.PageSize = filter.PageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize);


            var filteredFlights = ApplyFilters(allFlights, filter).ToList();

            ViewBag.TotalCount = allFlights.Count;
            ViewBag.FilteredCount = filteredFlights.Count;
            ViewBag.FileName = file;

            return View(filteredFlights);
        }




        private List<ApiFlightData> LoadFlightsFromFile()
        {
            if (!System.IO.File.Exists(ApiFilePath))
                return new List<ApiFlightData>();

            var json = System.IO.File.ReadAllText(ApiFilePath);
            var response = JsonConvert.DeserializeObject<ApiResponse>(json);

            return response != null && response.data != null
                ? response.data
                : new List<ApiFlightData>();
        }
        private void PrepareDropdowns(
            List<ApiFlightData> allFlights,
            FlightFilterVM filter)
        {
            // ---------- AIRLINES ----------
            var airlines = allFlights
                .Where(x => x.validatingAirlineCodes != null)
                .SelectMany(x => x.validatingAirlineCodes)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var airlineList = new List<SelectListItem>();
            airlineList.Add(new SelectListItem
            {
                Text = "All Airlines",
                Value = "",
                Selected = string.IsNullOrEmpty(filter.Airline)
            });

            foreach (var a in airlines)
            {
                airlineList.Add(new SelectListItem
                {
                    Text = a,
                    Value = a,
                    Selected = filter.Airline == a
                });
            }

            ViewBag.Airlines = airlineList;

            // ---------- STOPS ----------
            var stopsList = new List<SelectListItem>();

            stopsList.Add(new SelectListItem
            {
                Text = "All",
                Value = "",
                Selected = !filter.Stops.HasValue
            });

            stopsList.Add(new SelectListItem
            {
                Text = "Non-Stop",
                Value = "0",
                Selected = filter.Stops == 0
            });

            stopsList.Add(new SelectListItem
            {
                Text = "1 Stop",
                Value = "1",
                Selected = filter.Stops == 1
            });

            stopsList.Add(new SelectListItem
            {
                Text = "2+ Stops",
                Value = "2",
                Selected = filter.Stops == 2
            });

            ViewBag.Stops = stopsList;
        }
        private IEnumerable<ApiFlightData> ApplyFilters(
            List<ApiFlightData> allFlights,
            FlightFilterVM filter)
        {
            IEnumerable<ApiFlightData> query = allFlights;

            // Airline
            if (!string.IsNullOrEmpty(filter.Airline))
            {
                query = query.Where(x =>
                    x.validatingAirlineCodes != null &&
                    x.validatingAirlineCodes.Contains(filter.Airline));
            }

            // Depart time
            if (filter.DepartFrom.HasValue && filter.DepartTo.HasValue)
            {
                query = query.Where(x =>
                {
                    var seg = x.itineraries.First().segments.First();
                    var t = DateTime.Parse(seg.departure.at).TimeOfDay;
                    return t >= filter.DepartFrom.Value &&
                           t <= filter.DepartTo.Value;
                });
            }

            if (filter.ArriveFrom.HasValue && filter.ArriveTo.HasValue)
            {
                query = query.Where(x =>
                {
                    var seg = x.itineraries.First().segments.Last();
                    var t = DateTime.Parse(seg.arrival.at).TimeOfDay;
                    return t >= filter.ArriveFrom.Value &&
                           t <= filter.ArriveTo.Value;
                });
            }

            if (filter.Stops.HasValue)
            {
                query = query.Where(x =>
                {
                    var segs = x.itineraries.First().segments;
                    return (segs.Count - 1) == filter.Stops.Value;
                });
            }

            return query;
        }



        public IActionResult SearchMetaResponses(int pageNumber = 1, int pageSize = 20)
        {
            using var db = new RSFLY();

            var query = db.FlightSearchCaches
                          .OrderByDescending(x => x.CreatedAt);

            int totalRecords = query.Count();

            var data = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages =
                (int)Math.Ceiling((double)totalRecords / pageSize);

            return View(data);
        }

        public IActionResult ViewSearchMetaJson(string searchId)
        {
            var folder = AplicationHelper.GetSearchFolder(searchId);
            var filePath = Path.Combine(folder, "search_.txt");

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var json = System.IO.File.ReadAllText(filePath);

            ViewBag.SearchId = searchId;
            ViewBag.Json = json;

            return View();
        }
        public IActionResult ViewMetaApiResponse(
    string searchId,
    string airline,
    int? stops)
        {
            // 🔹 Load META RESPONSE JSON
            var folder = AplicationHelper.GetSearchFolder(searchId);
            var filePath = Path.Combine(folder, "meta_response.txt");
            // ⬆️ yahan wahi file name rakho jisme META RESPONSE save hota hai

            if (!System.IO.File.Exists(filePath))
                return NotFound("Meta response file not found");

            var json = System.IO.File.ReadAllText(filePath);
            var response = JsonConvert.DeserializeObject<MetaSearchResponse>(json);

            var allFlights = response.result.airlines;

            // 🔹 Airline filter dropdown
            var airlines = allFlights
                .SelectMany(x => x.segments)
                .SelectMany(x => x.oSeg)
                .SelectMany(x => x.iSeg)
                .Select(x => x.airlineNm)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Airlines = airlines;
            ViewBag.SearchId = searchId;
            ViewBag.SelectedAirline = airline;
            ViewBag.SelectedStops = stops;

            // 🔹 Apply filters
            IEnumerable<MetaAirlineResult> query = allFlights;

            if (!string.IsNullOrEmpty(airline))
                query = query.Where(x =>
                    x.segments.Any(s =>
                        s.oSeg.Any(o =>
                            o.iSeg.Any(i => i.airlineNm == airline))));

            if (stops.HasValue)
                query = query.Where(x => x.stop == stops.Value);

            return View(query.ToList());
        }


        [HttpGet]
        public IActionResult MetaSearchResponse(
    string fileName,
    string airline,
    int? stops,
    string cabin)
        {
            FlightsResult model = null;
            MetaFlightSearchResponse metaResp = null;
            var folder = Path.Combine(
                  Directory.GetCurrentDirectory(),
                  "wwwroot",
                  "api-cache"
              );

            var path = Path.Combine(folder, "MetaApiresponse.json");

            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                model = JsonConvert.DeserializeObject<FlightsResult>(json);

                metaResp = MetaFlightController.MapToMetaResponse(model);

                metaResp.Airlines = metaResp.Airlines.OrderBy(x => x.TotalAmt).ToList();
                metaResp.Airlines = metaResp.Airlines
            .OrderBy(x => x.TotalAmt)
            .ToList();

                // 🔹 Step 4: Filters
                if (!string.IsNullOrEmpty(airline))
                {
                    metaResp.Airlines = metaResp.Airlines
                        .Where(x =>
                            x.Segments.Any(s =>
                                s.OSeg.Any(o =>
                                    o.ISeg.Any(i => i.AirlineNm == airline))))
                        .ToList();
                }

                if (stops.HasValue)
                {
                    metaResp.Airlines = metaResp.Airlines
                        .Where(x => x.Stop == stops.Value)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(cabin))
                {
                    metaResp.Airlines = metaResp.Airlines
                        .Where(x => x.CabinClass == cabin)
                        .ToList();
                }

                // 🔹 Dropdowns
                ViewBag.Airlines = metaResp.Airlines
                    .SelectMany(x => x.Segments)
                    .SelectMany(x => x.OSeg)
                    .SelectMany(x => x.ISeg)
                    .Select(x => x.AirlineNm)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }


            ViewBag.FileName = fileName;
            ViewBag.SelectedAirline = airline;
            ViewBag.SelectedStops = stops;
            ViewBag.SelectedCabin = cabin;

            return View(metaResp);

        }

        [HttpPost]
        public async Task<IActionResult> UploadMetaJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction("MetaSearchResponse");

            var folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "api-cache",
                "MetaApiresponse"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}.json";
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);

            }

            return RedirectToAction("MetaSearchResponse", new { fileName });
        }

        public IActionResult BlockRulesView(string campId, int? editId)
        {
            campId ??= "Rush_FS";

            var vm = new BlockRulesPageVM();

            var filePath = AplicationHelper.GetBlockRulesFile(campId);

            List<BlockAirlineRule> rules = new List<BlockAirlineRule>();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        rules = JsonConvert.DeserializeObject<List<BlockAirlineRule>>(json);
                    }
                    catch
                    {
                        rules = new List<BlockAirlineRule>();
                    }
                }
            }

            vm.Rules = rules;

            vm.FormRule = editId.HasValue
                ? rules.FirstOrDefault(x => x.RuleId == editId.Value)
                : new BlockAirlineRule { Status = true, CampId = campId };

            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveBlockRule(BlockRulesPageVM postData, IFormFile excelFile)
        {
            if (!ModelState.IsValid)
                return View("BlockRulesView", postData);

            try
            {
                /* =========================================================
                   ✅ EXCEL UPLOAD MODE
                ========================================================= */
                if (excelFile != null && excelFile.Length > 0)
                {
                    /* ✅ DELETE OLD DATA FIRST */
                    _context.BlockAirlineRules.RemoveRange(_context.BlockAirlineRules);
                    _context.SaveChanges();

                    using (var stream = excelFile.OpenReadStream())
                    using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                    {
                        var ws = workbook.Worksheet(1);
                        var rows = ws.RowsUsed().Skip(1); // Skip Header

                        foreach (var row in rows)
                        {
                            /* ✅ Skip Empty Rows */
                            if (string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()))
                                continue;

                            /* ✅ SAFE PARSING */
                            int.TryParse(row.Cell(8).GetValue<string>(), out int advanceDays);
                            DateTime.TryParse(row.Cell(9).GetValue<string>(), out DateTime dateFrom);
                            DateTime.TryParse(row.Cell(10).GetValue<string>(), out DateTime dateTo);

                            var entity = new TravelWeb.Datamodel.BlockAirlineRule
                            {
                                Origin = row.Cell(2).GetValue<string>(),
                                Destination = row.Cell(3).GetValue<string>(),
                                CabinClassOb = row.Cell(4).GetValue<string>(),
                                CabinClassIb = row.Cell(5).GetValue<string>(),
                                TimeOb = row.Cell(6).GetValue<string>(),
                                TimeIb = row.Cell(7).GetValue<string>(),

                                AdvanceDays = advanceDays,

                                DateFrom = row.Cell(9).GetValue<DateTime?>(),
                                DateTo = row.Cell(10).GetValue<DateTime?>(),

                                BlockDates = row.Cell(11).GetValue<string>(),
                                Status = row.Cell(12).GetValue<string>() == "Active",

                                Company = row.Cell(14).GetValue<string>(),
                                CampId = row.Cell(15).GetValue<string>(),
                                UpdatedBy = row.Cell(16).GetValue<string>(),
                                AirlineCodes = row.Cell(18).GetValue<string>(),

                                CreatedAt = DateTime.Now
                            };

                            _context.BlockAirlineRules.Add(entity);
                        }
                    }
                    _context.SaveChanges();

                    AplicationHelper.SaveBlockRulesToFile(postData.FormRule.CampId, _context);

                    TempData["Success"] = "Excel uploaded & database refreshed successfully.";
                    return RedirectToAction("BlockRulesView");
                }


                var vm = postData.FormRule;

                if (vm.RuleId == 0)
                {
                    var entity = new TravelWeb.Datamodel.BlockAirlineRule
                    {
                        Origin = vm.Origin,
                        Destination = vm.Destination,
                        CabinClassOb = vm.CabinClassOb,
                        CabinClassIb = vm.CabinClassIb,
                        TimeOb = vm.TimeOb,
                        TimeIb = vm.TimeIb,
                        AdvanceDays = vm.AdvanceDays,
                        DateFrom = vm.DateFrom,
                        DateTo = vm.DateTo,
                        BlockDates = vm.BlockDates,
                        Status = vm.Status,
                        Company = vm.Company,
                        CampId = vm.CampId,
                        UpdatedBy = vm.UpdatedBy,
                        AirlineCodes = vm.AirlineCodes,

                        CreatedAt = DateTime.Now
                    };

                    _context.BlockAirlineRules.Add(entity);
                }
                else
                {
                    var entity = _context.BlockAirlineRules
                                         .FirstOrDefault(x => x.RuleId == vm.RuleId);

                    if (entity == null)
                        return NotFound();

                    entity.Origin = vm.Origin;
                    entity.Destination = vm.Destination;
                    entity.CabinClassOb = vm.CabinClassOb;
                    entity.CabinClassIb = vm.CabinClassIb;
                    entity.TimeOb = vm.TimeOb;
                    entity.TimeIb = vm.TimeIb;
                    entity.AdvanceDays = vm.AdvanceDays;
                    entity.DateFrom = vm.DateFrom;
                    entity.DateTo = vm.DateTo;
                    entity.BlockDates = vm.BlockDates;
                    entity.Status = vm.Status;
                    entity.Company = vm.Company;
                    entity.CampId = vm.CampId;
                    entity.UpdatedBy = vm.UpdatedBy;
                    entity.AirlineCodes = vm.AirlineCodes;

                    entity.UpdatedAt = DateTime.Now;
                }

                _context.SaveChanges();
                AplicationHelper.SaveBlockRulesToFile(postData.FormRule.CampId, _context);

                TempData["Success"] = "Rule saved successfully.";
                return RedirectToAction("BlockRulesView");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong while saving.";
                return RedirectToAction("BlockRulesView");
            }
        }



        public IActionResult DownloadExcel()
        {
            var rules = _context.BlockAirlineRules.ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Block Rules");

                /* ✅ HEADERS */
                ws.Cell(1, 1).Value = "RuleId";
                ws.Cell(1, 2).Value = "Origin";
                ws.Cell(1, 3).Value = "Destination";
                ws.Cell(1, 4).Value = "CabinClassOB";
                ws.Cell(1, 5).Value = "CabinClassIB";
                ws.Cell(1, 6).Value = "TimeOB";
                ws.Cell(1, 7).Value = "TimeIB";
                ws.Cell(1, 8).Value = "AdvanceDays";
                ws.Cell(1, 9).Value = "DateFrom";
                ws.Cell(1, 10).Value = "DateTo";
                ws.Cell(1, 11).Value = "BlockDates";
                ws.Cell(1, 12).Value = "Status";
                ws.Cell(1, 13).Value = "CreatedAt";
                ws.Cell(1, 14).Value = "Company";
                ws.Cell(1, 15).Value = "CampID";
                ws.Cell(1, 16).Value = "UpdatedBy";
                ws.Cell(1, 17).Value = "UpdatedAt";
                ws.Cell(1, 18).Value = "AirlineCodes";

                /* ✅ DATA */
                int row = 2;

                foreach (var r in rules)
                {
                    ws.Cell(row, 1).Value = r.RuleId;
                    ws.Cell(row, 2).Value = r.Origin;
                    ws.Cell(row, 3).Value = r.Destination;
                    ws.Cell(row, 4).Value = r.CabinClassOb;
                    ws.Cell(row, 5).Value = r.CabinClassIb;
                    ws.Cell(row, 6).Value = r.TimeOb;
                    ws.Cell(row, 7).Value = r.TimeIb;
                    ws.Cell(row, 8).Value = r.AdvanceDays;
                    ws.Cell(row, 9).Value = r.DateFrom;
                    ws.Cell(row, 9).Style.DateFormat.Format = "yyyy-MM-dd";

                    // DateTo
                    ws.Cell(row, 10).Value = r.DateTo;
                    ws.Cell(row, 10).Style.DateFormat.Format = "yyyy-MM-dd";

                    ws.Cell(row, 11).Value = r.BlockDates;
                    ws.Cell(row, 12).Value = r.Status ?? false;
                    ws.Cell(row, 13).Value = r.CreatedAt;
                    ws.Cell(row, 13).Style.DateFormat.Format = "yyyy-MM-dd";
                    ws.Cell(row, 14).Value = r.Company;
                    ws.Cell(row, 15).Value = r.CampId;
                    ws.Cell(row, 16).Value = r.UpdatedBy;
                    if (r.UpdatedAt != null)
                    {
                        ws.Cell(row, 17).Value = r.UpdatedAt;
                        ws.Cell(row, 17).Style.DateFormat.Format = "yyyy-MM-dd";
                    }
                    ws.Cell(row, 18).Value = r.AirlineCodes;

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BlockRules.xlsx"
                    );
                }
            }
        }

        public IActionResult DownloadAllowExcel()
        {
            var routes = _context.AirlineRoutes.ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Allow Rules");

                /* HEADERS */
                ws.Cell(1, 1).Value = "RoutesId";
                ws.Cell(1, 2).Value = "Origin";
                ws.Cell(1, 3).Value = "Destination";
                ws.Cell(1, 4).Value = "Status";
                ws.Cell(1, 5).Value = "UpdatedBy";
                ws.Cell(1, 6).Value = "Company";
                ws.Cell(1, 7).Value = "CampID";
                ws.Cell(1, 8).Value = "CreatedAt";
                ws.Cell(1, 9).Value = "UpdatedAt";

                /* DATA */
                int row = 2;

                foreach (var r in routes)
                {
                    ws.Cell(row, 1).Value = r.RoutesId;
                    ws.Cell(row, 2).Value = r.Origin;
                    ws.Cell(row, 3).Value = r.Destination;
                    ws.Cell(row, 4).Value = r.Status ?? false;
                    ws.Cell(row, 5).Value = r.UpdatedBy;
                    ws.Cell(row, 6).Value = r.Company;
                    ws.Cell(row, 7).Value = r.CampId;
                    ws.Cell(row, 8).Value = r.CreatedAt;
                    ws.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd";
                    if (r.UpdatedAt != null)
                    {
                        ws.Cell(row, 9).Value = r.UpdatedAt;
                        ws.Cell(row, 9).Style.DateFormat.Format = "yyyy-MM-dd";
                    }

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AllowRules.xlsx"
                    );
                }
            }
        }

        public IActionResult DeleteRule(int id)
        {
            try
            {
                var rule = _context.BlockAirlineRules
                                   .FirstOrDefault(x => x.RuleId == id);

                if (rule == null)
                {
                    AplicationHelper.SaveBlockRulesToFile("Rush_FS", _context);
                    TempData["Error"] = "Rule not found.";
                    return RedirectToAction("BlockRulesView");
                }

                var campId = rule.CampId;

                _context.BlockAirlineRules.Remove(rule);
                _context.SaveChanges();
                AplicationHelper.SaveBlockRulesToFile(campId, _context);


                TempData["Success"] = "Rule deleted successfully.";
            }
            catch
            {
                TempData["Error"] = "Something went wrong while deleting the rule.";
            }

            return RedirectToAction("BlockRulesView");
        }


        //public IActionResult AllowRulesView(int? editId)
        //{
        //    var dbRules = _context.AirlineRoutes.ToList();

        //    var rules = dbRules.Select(x => new AllowAirlineRuleDto
        //    {
        //        RoutesId = x.RoutesId,
        //        Status = x.Status ?? true,
        //        Company = x.Company,
        //        CampID = x.CampId,
        //        UpdatedBy = x.UpdatedBy,
        //        CreatedAt = x.CreatedAt,
        //        UpdatedAt = x.UpdatedAt,

        //        Origins = new HashSet<string> { x.Origin },
        //        Destinations = new HashSet<string> { x.Destination }
        //    }).ToList();

        //    var vm = new AllowRulesPageVM
        //    {
        //        Rules = rules,
        //        FormRule = new AllowAirlineRuleDto { Status = true }
        //    };


        //    if (editId.HasValue)
        //    {
        //        var rule = rules.FirstOrDefault(x => x.RoutesId == editId.Value);

        //        if (rule != null)
        //            vm.FormRule = rule;
        //    }

        //    return View(vm);
        //}


        public IActionResult AllowRulesView(int? editId, AllowRulesPageVM vm, string OriginsText, string DestinationsText)
        {
            var dbRules = _context.AirlineRoutes.ToList();

            var rules = dbRules.Select(x => new AllowAirlineRuleDto
            {
                RoutesId = x.RoutesId,
                Status = x.Status ?? true,
                Company = x.Company,
                CampID = x.CampId,
                UpdatedBy = x.UpdatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

                Origins = new HashSet<string> { x.Origin },
                Destinations = new HashSet<string> { x.Destination }
            }).ToList();

            // ✅ FILTER
            var originSet = ToSet(OriginsText);
            var destSet = ToSet(DestinationsText);

            if (originSet.Any())
            {
                rules = rules.Where(x =>
                    x.Origins.Any(o => originSet.Contains(OriginsText))
                ).ToList();
            }

            if (destSet.Any())
            {
                rules = rules.Where(x =>
                    x.Destinations.Any(d => destSet.Contains(DestinationsText))
                ).ToList();
            }

            //if (!string.IsNullOrWhiteSpace(vm?.FormRule?.Company))
            //{
            //    rules = rules.Where(x =>
            //        (x.Company ?? "").Contains(vm.FormRule.Company)
            //    ).ToList();
            //}

            if (!string.IsNullOrWhiteSpace(vm?.FormRule?.CampID))
            {
                rules = rules.Where(x =>
                    (x.CampID ?? "").Contains(vm.FormRule.CampID)
                ).ToList();
            }

            var formRule = vm?.FormRule ?? new AllowAirlineRuleDto { Status = true };

            if (editId.HasValue)
            {
                var rule = rules.FirstOrDefault(x => x.RoutesId == editId.Value);
                if (rule != null)
                    formRule = rule;
            }

            return View(new AllowRulesPageVM
            {
                Rules = rules,
                FormRule = formRule
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllowRulesViewCopy(int? copyId)
        {

            if (copyId == null || copyId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid route id."
                });
            }

            var existingRoute = _context.AirlineRoutes
                .FirstOrDefault(x => x.RoutesId == copyId.Value);

            if (existingRoute == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Record not found."
                });
            }

            var newRoute = new Datamodel.AirlineRoute
            {
                Origin = existingRoute.Origin,
                Destination = existingRoute.Destination,
                Status = existingRoute.Status,
                UpdatedBy = existingRoute.UpdatedBy,
                Company = existingRoute.Company,
                CampId = existingRoute.CampId,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.AirlineRoutes.Add(newRoute);
            _context.SaveChanges();

            var dbRules = _context.AirlineRoutes.ToList();

            var rules = dbRules.Select(x => new AllowAirlineRuleDto
            {
                RoutesId = x.RoutesId,
                Status = x.Status ?? true,
                Company = x.Company,
                CampID = x.CampId,
                UpdatedBy = x.UpdatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

                Origins = new HashSet<string> { x.Origin },
                Destinations = new HashSet<string> { x.Destination }
            }).ToList();

            var vm = new AllowRulesPageVM
            {
                Rules = rules,
                FormRule = new AllowAirlineRuleDto { Status = true }
            };


            if (copyId.HasValue)
            {
                var rule = rules.FirstOrDefault(x => x.RoutesId == copyId.Value);

                if (rule != null)
                    vm.FormRule = rule;
            }

            return View("AllowRulesView", vm);
        }

        //    [HttpPost]
        //    public IActionResult SaveAllowRule(
        //AllowRulesPageVM postData,
        //IFormFile excelFile,
        //string OriginsText,
        //string DestinationsText)
        //    {
        //        try
        //        {

        //            if (excelFile != null && excelFile.Length > 0)
        //            {
        //                var extension = Path.GetExtension(excelFile.FileName);

        //                if (extension != ".xlsx")
        //                {
        //                    TempData["Error"] = "Only .xlsx Excel files allowed.";
        //                    return RedirectToAction("AllowRulesView");
        //                }

        //                using var transaction = _context.Database.BeginTransaction(); 

        //                try
        //                {
        //                    List<TravelWeb.Datamodel.AirlineRoute> parsedRules = new List<TravelWeb.Datamodel.AirlineRoute>();

        //                    using (var stream = excelFile.OpenReadStream())
        //                    using (var workbook = new XLWorkbook(stream))
        //                    {
        //                        var ws = workbook.Worksheet(1);
        //                        var rows = ws.RowsUsed().Skip(1);

        //                        foreach (var row in rows)
        //                        {
        //                            if (string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()))
        //                                continue;

        //                            parsedRules.Add(new TravelWeb.Datamodel.AirlineRoute
        //                            {
        //                                Origin = row.Cell(2).GetValue<string>(),
        //                                Destination = row.Cell(3).GetValue<string>(),

        //                                Status = GetBoolSafe(row.Cell(4)),

        //                                Company = row.Cell(6).GetValue<string>(),
        //                                CampId = row.Cell(7).GetValue<string>(),
        //                                UpdatedBy = row.Cell(5).GetValue<string>(),

        //                                // ✅ DATE FORMAT SAFE
        //                                CreatedAt = DateTime.Now
        //                            });
        //                        }
        //                    }

        //                    if (!parsedRules.Any())
        //                    {
        //                        TempData["Error"] = "Excel empty or invalid.";
        //                        return RedirectToAction("AllowRulesView");
        //                    }

        //                    var oldData = _context.AirlineRoutes.ToList();
        //                    _context.AirlineRoutes.RemoveRange(oldData);

        //                    // ✅ INSERT
        //                    _context.AirlineRoutes.AddRange(parsedRules);

        //                    _context.SaveChanges();

        //                    transaction.Commit(); 
        //                    _context.Database.ExecuteSqlRawAsync("DELETE FROM AirlineRoutes");

        //                    // ✅ FILE REFRESH
        //                    RefreshAllowRulesFile(parsedRules.First().CampId);

        //                    TempData["Success"] = "Excel uploaded successfully.";
        //                }
        //                catch
        //                {
        //                    transaction.Rollback(); // ❌ NO DATA LOSS

        //                    TempData["Error"] = "Excel file corrupted or invalid.";
        //                }

        //                return RedirectToAction("AllowRulesView");
        //            }


        //            var vm = postData.FormRule;

        //            vm.Origins = OriginsText?
        //                .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                .Select(x => x.Trim())
        //                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        //            vm.Destinations = DestinationsText?
        //                .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                .Select(x => x.Trim())
        //                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        //            if (vm.Origins == null || vm.Destinations == null ||
        //                !vm.Origins.Any() || !vm.Destinations.Any())
        //            {
        //                TempData["Error"] = "Origin & Destination required.";
        //                return RedirectToAction("AllowRulesView");
        //            }

        //            if (vm.RoutesId > 0)
        //            {
        //                var existing = _context.AirlineRoutes
        //                                       .Where(x => x.RoutesId == vm.RoutesId);

        //                _context.AirlineRoutes.RemoveRange(existing);
        //                _context.SaveChanges();
        //            }
        //            var entity = new TravelWeb.Datamodel.AirlineRoute
        //            {
        //                Origin = OriginsText,
        //                Destination = DestinationsText,
        //                Status = vm.Status,
        //                Company = vm.Company,
        //                CampId = vm.CampID,
        //                UpdatedBy = vm.UpdatedBy,
        //                CreatedAt = DateTime.Now
        //            };
        //            _context.AirlineRoutes.Add(entity);
        //            ///* ✅ MULTI INSERT */
        //            //foreach (var origin in vm.Origins)
        //            //{
        //            //    foreach (var destination in vm.Destinations)
        //            //    {
        //            //        var entity = new TravelWeb.Datamodel.AirlineRoute
        //            //        {
        //            //            Origin = OriginsText,
        //            //            Destination = DestinationsText,
        //            //            Status = vm.Status,
        //            //            Company = vm.Company,
        //            //            CampId = vm.CampID,
        //            //            UpdatedBy = vm.UpdatedBy,
        //            //            CreatedAt = DateTime.Now
        //            //        };

        //            //        _context.AirlineRoutes.Add(entity);
        //            //    }
        //            //}

        //            _context.SaveChanges();

        //            RefreshAllowRulesFile(vm.CampID);

        //            TempData["Success"] = "Allow rule saved successfully.";
        //            return RedirectToAction("AllowRulesView");
        //        }
        //        catch
        //        {
        //            TempData["Error"] = "Something went wrong.";
        //            return RedirectToAction("AllowRulesView");
        //        }
        //    }

        [HttpPost]
        public IActionResult SaveAllowRule(
    AllowRulesPageVM postData,
    IFormFile excelFile,
    string OriginsText,
    string DestinationsText)
        {
            try
            {
                if (excelFile != null && excelFile.Length > 0)
                {
                    var extension = Path.GetExtension(excelFile.FileName);

                    if (extension != ".xlsx")
                    {
                        TempData["Error"] = "Only .xlsx Excel files allowed.";
                        return RedirectToAction("AllowRulesView");
                    }

                    var parsedRules = new List<TravelWeb.Datamodel.AirlineRoute>();

                    using (var stream = excelFile.OpenReadStream())
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var ws = workbook.Worksheet(1);
                        var rows = ws.RowsUsed().Skip(1);

                        foreach (var row in rows)
                        {
                            if (string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()))
                                continue;

                            parsedRules.Add(new TravelWeb.Datamodel.AirlineRoute
                            {
                                Origin = row.Cell(2).GetValue<string>(),
                                Destination = row.Cell(3).GetValue<string>(),
                                Status = GetBoolSafe(row.Cell(4)),
                                Company = row.Cell(6).GetValue<string>(),
                                CampId = row.Cell(7).GetValue<string>(),
                                UpdatedBy = row.Cell(5).GetValue<string>(),
                                CreatedAt = DateTime.Now
                            });
                        }
                    }

                    if (!parsedRules.Any())
                    {
                        TempData["Error"] = "Excel empty or invalid.";
                        return RedirectToAction("AllowRulesView");
                    }

                    var oldData = _context.AirlineRoutes.ToList();
                    _context.AirlineRoutes.RemoveRange(oldData);

                    _context.AirlineRoutes.AddRange(parsedRules);
                    _context.SaveChanges();

                    RefreshAllowRulesFile(parsedRules.First().CampId);

                    TempData["Success"] = "Excel uploaded successfully.";
                    return RedirectToAction("AllowRulesView");
                }

                var vm = postData.FormRule;

                if (vm.RoutesId > 0)
                {
                    var existing = _context.AirlineRoutes
                                           .Where(x => x.RoutesId == vm.RoutesId)
                                           .ToList();

                    _context.AirlineRoutes.RemoveRange(existing);
                }

                var entity = new TravelWeb.Datamodel.AirlineRoute
                {
                    Origin = OriginsText,
                    Destination = DestinationsText,
                    Status = vm.Status,
                    Company = vm.Company,
                    CampId = vm.CampID,
                    UpdatedBy = vm.UpdatedBy,
                    CreatedAt = DateTime.Now
                };

                _context.AirlineRoutes.Add(entity);
                _context.SaveChanges();

                RefreshAllowRulesFile(vm.CampID);

                TempData["Success"] = "Allow rule saved successfully.";
                return RedirectToAction("AllowRulesView");
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Error SaveAllowRule" + ex.Message);
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("AllowRulesView");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAllowRule(int id)
        {
            try
            {
                var rule = _context.AirlineRoutes
                               .FirstOrDefault(x => x.RoutesId == id);

                if (rule != null)
                {
                    var campId = rule.CampId;

                    _context.AirlineRoutes.Remove(rule);
                    _context.SaveChanges();
                    AplicationHelper.Log(" DeleteAllowRule  ID - " + id);
                    RefreshAllowRulesFile(campId);
                }

            }
            catch (Exception ex)
            {

                AplicationHelper.Log("Error DeleteAllowRule " + ex.Message);
            }


            return RedirectToAction("AllowRulesView");
        }

        private void RefreshAllowRulesFile(string campId)
        {
            try
            {
                if (campId != null)
                {
                    var dbRules = _context.AirlineRoutes
                                                      .Where(x => x.CampId == campId)
                                                      .ToList();

                    var rules = dbRules.Select(x => new AirlineRoute
                    {
                        RoutesId = x.RoutesId,
                        Status = x.Status ?? true,
                        Company = x.Company,
                        CampId = x.CampId,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,

                        Origin = x.Origin,
                        Destination = x.Destination
                    }).ToList();

                    AplicationHelper.SaveAllowRulesToFile(rules, campId);
                }
            }
            catch (Exception ex)
            {

                AplicationHelper.Log("Error RefreshAllowRulesFile : " + ex.Message);

            }


        }




        //public IActionResult MarkupRulesView(int? editId, int? copyId)
        //{
        //    var dbRules = _context.MarkupRules.ToList();
        //    var rules = MapToDto(dbRules);

        //    MarkupRuleDto formRule;

        //    if (editId.HasValue)
        //        formRule = rules.FirstOrDefault(x => x.RuleId == editId.Value);

        //    else if (copyId.HasValue)
        //    {
        //        var rule = rules.FirstOrDefault(x => x.RuleId == copyId.Value);

        //        formRule = new MarkupRuleDto
        //        {
        //            RuleName = rule.RuleName,
        //            Origins = rule.Origins,
        //            Destinations = rule.Destinations,
        //            Airlines = rule.Airlines,
        //            CabinClasses = rule.CabinClasses,
        //            JourneyType = rule.JourneyType,
        //            MarkType = rule.MarkType,
        //            MarkUp = rule.MarkUp,
        //            Status = rule.Status,
        //            Company = rule.Company,
        //            CampId = rule.CampId,
        //            MinimumAmountRange = rule.MinimumAmountRange,
        //            PAirlines = rule.PAirlines,
        //            RAirlines =  rule.RAirlines,
        //            ReplaceAirline =  rule.ReplaceAirline,
        //            InboundTime = rule.InboundTime,
        //            OutboundTime = rule.OutboundTime,
        //            TravelDateFrom =  rule.TravelDateFrom,
        //            TravelDateTo = rule.TravelDateTo,
        //            SaleDateFrom =  rule.SaleDateFrom,
        //            SaleDateTo = rule.SaleDateTo,
        //            PaxType =  rule.PaxType,
        //            PaxCount =  rule.PaxCount,
        //            Stops =  rule.Stops,
        //            AdvanceDays = rule.AdvanceDays,
        //            ClassIb  = rule.ClassIb,
        //            ClassOb =  rule.ClassOb,
        //            CabinClass = rule.CabinClass
        //        };
        //    }
        //    else
        //        formRule = new MarkupRuleDto { Status = true };

        //    return View(new MarkupRulesPageVM
        //    {
        //        Rules = rules,
        //        FormRule = formRule
        //    });
        //}

        public IActionResult MarkupRulesView(int? editId, int? copyId, MarkupRulesPageVM vm, string OriginsText,
     string DestinationsText,
     string AirlinesText,
     string CabinText,
     string AmountText)
        {


            var searchModel = new MarkupRuleDto();
            searchModel = vm.FormRule;
            var dbRules = _context.MarkupRules.ToList();
            var rules = MapToDto(dbRules);


            if (searchModel != null)
            {
                var originSet = ToSet(OriginsText);
                var destSet = ToSet(DestinationsText);
                var airlineSet = ToSet(AirlinesText);

                // ✅ RuleName
                if (!string.IsNullOrWhiteSpace(searchModel.RuleName))
                    rules = rules.Where(x => (x.RuleName ?? "")
                        .ToLower()
                        .Contains(searchModel.RuleName.ToLower()))
                        .ToList();

                // ✅ Origin (multi match)
                if (originSet.Any())
                    rules = rules.Where(x =>
                        x.Origins != null &&
                        x.Origins.Any(o => originSet.Contains(o.ToUpper()))
                    ).ToList();

                // ✅ Destination
                if (destSet.Any())
                    rules = rules.Where(x =>
                        x.Destinations != null &&
                        x.Destinations.Any(d => destSet.Contains(d.ToUpper()))
                    ).ToList();

                // ✅ Airlines
                if (airlineSet.Any())
                    rules = rules.Where(x =>
                        x.Airlines != null &&
                        x.Airlines.Any(a => airlineSet.Contains(a.ToUpper()))
                    ).ToList();

                // ✅ Cabin Class
                if (!string.IsNullOrWhiteSpace(CabinText))
                    rules = rules.Where(x =>
                        (x.CabinClass ?? "").Equals(CabinText, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                // ✅ Company
                if (!string.IsNullOrWhiteSpace(searchModel.Company))
                    rules = rules.Where(x =>
                        (x.Company ?? "").Contains(searchModel.Company)
                    ).ToList();

                // ✅ CampId
                if (!string.IsNullOrWhiteSpace(searchModel.CampId))
                    rules = rules.Where(x =>
                        (x.CampId ?? "").Contains(searchModel.CampId)
                    ).ToList();

                // ✅ PaxCount
                if (searchModel.PaxCount.HasValue)
                    rules = rules.Where(x => x.PaxCount == searchModel.PaxCount).ToList();

                // ✅ Stops
                if (searchModel.Stops.HasValue && searchModel.Stops != -1)
                    rules = rules.Where(x => x.Stops == searchModel.Stops).ToList();

                // ✅ JourneyType
                if (!string.IsNullOrWhiteSpace(searchModel.JourneyType))
                    rules = rules.Where(x => x.JourneyType == searchModel.JourneyType).ToList();

                // ✅ Date Range (Travel)
                if (searchModel.TravelDateFrom.HasValue)
                    rules = rules.Where(x => x.TravelDateFrom >= searchModel.TravelDateFrom).ToList();

                if (searchModel.TravelDateTo.HasValue)
                    rules = rules.Where(x => x.TravelDateTo <= searchModel.TravelDateTo).ToList();

                // ✅ Amount Range (200-400)
                if (!string.IsNullOrWhiteSpace(AmountText))
                {
                    var parts = AmountText.Split('-');

                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var min) &&
                        decimal.TryParse(parts[1], out var max))
                    {
                        rules = rules.Where(x =>
                            x.MinimumAmountRange.min >= min &&
                            x.MinimumAmountRange.max <= max
                        ).ToList();
                    }
                }
            }

            MarkupRuleDto formRule;

            if (editId.HasValue)
            {

                formRule = rules.FirstOrDefault(x => x.RuleId == editId.Value);
            }

            else if (copyId.HasValue)
            {
                formRule = rules.FirstOrDefault(x => x.RuleId == copyId.Value);
                var existingRule = _context.MarkupRules
          .FirstOrDefault(x => x.RuleId == copyId.Value);

                if (existingRule != null)
                {
                    var newRule = new MarkupRule
                    {
                        RuleName = existingRule.RuleName + " - Copy", // optional
                        Origin = existingRule.Origin,
                        Destination = existingRule.Destination,
                        Airline = existingRule.Airline,
                        CabinClass = existingRule.CabinClass,
                        JourneyType = existingRule.JourneyType,
                        ClassOb = existingRule.ClassOb,
                        ClassIb = existingRule.ClassIb,
                        MarkType = existingRule.MarkType,
                        MarkUp = existingRule.MarkUp,
                        Status = existingRule.Status,
                        MinDays = existingRule.MinDays,
                        MaxDays = existingRule.MaxDays,
                        AdvanceDays = existingRule.AdvanceDays,
                        MarkApplicable2 = existingRule.MarkApplicable2,
                        Rairline = existingRule.Rairline,
                        Pairline = existingRule.Pairline,
                        Stops = existingRule.Stops,
                        Ttime = existingRule.Ttime,
                        RestrictedClass = existingRule.RestrictedClass,
                        MinimumAmount = existingRule.MinimumAmount,
                        PaxCount = existingRule.PaxCount,
                        PaxType = existingRule.PaxType,
                        TravelDateFrom = existingRule.TravelDateFrom,
                        TravelDateTo = existingRule.TravelDateTo,
                        SaleDateFrom = existingRule.SaleDateFrom,
                        SaleDateTo = existingRule.SaleDateTo,
                        BlockOdateFrom = existingRule.BlockOdateFrom,
                        BlockOdateTo = existingRule.BlockOdateTo,
                        Company = existingRule.Company,
                        CampId = existingRule.CampId,
                        UpdateBy = existingRule.UpdateBy,
                        InboundTime = existingRule.InboundTime,
                        OutboundTime = existingRule.OutboundTime
                    };

                    // ✅ Insert new record
                    _context.MarkupRules.Add(newRule);
                    _context.SaveChanges();

                    // ✅ Latest record ko DTO me convert karke bind karo
                    formRule = MapToDto(new List<MarkupRule> { newRule }).FirstOrDefault();
                }

            }
            else
                formRule = vm.FormRule ?? new MarkupRuleDto { Status = true };


            var Json = JsonConvert.SerializeObject(formRule);

            return View(new MarkupRulesPageVM
            {
                Rules = rules,
                FormRule = formRule
            });
        }

        private HashSet<string> ToSet(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new HashSet<string>();

            return input.Split(',')
                        .Select(x => x.Trim().ToUpper())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToHashSet();
        }

        public async Task<IActionResult> DownloadMarkupExcel()
        {
            var data = await _context.MarkupRules
                                     .AsNoTracking()
                                     .OrderBy(x => x.RuleId) // optional (same order as DB)
                                     .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("MarkupRules");

                // ✅ FIXED COLUMN ORDER (IMPORTANT)
                string[] columns = new[]
                {
            "RuleId","RuleName","Origin","Destination","Airline","CabinClass",
            "JourneyType","ClassOB","ClassIB","MarkType","MarkUp","Status",
            "MinDays","MaxDays","AdvanceDays","MarkApplicable2","RAirline",
            "PAirline","Stops","TTime","RestrictedClass","MinimumAmount",
            "PaxCount","PaxType","TravelDateFrom","TravelDateTo",
            "SaleDateFrom","SaleDateTo","BlockODateFrom","BlockODateTo",
            "Company","CampId","UpdateBy","InboundTime","OutboundTime"
        };

                // ✅ HEADER
                for (int i = 0; i < columns.Length; i++)
                {
                    ws.Cell(1, i + 1).Value = columns[i];
                }

                ws.Range(1, 1, 1, columns.Length).Style.Font.Bold = true;

                int row = 2;

                foreach (var x in data)
                {
                    ws.Cell(row, 1).Value = x.RuleId;
                    ws.Cell(row, 2).Value = x.RuleName ?? "";
                    ws.Cell(row, 3).Value = x.Origin ?? "";
                    ws.Cell(row, 4).Value = x.Destination ?? "";
                    ws.Cell(row, 5).Value = x.Airline ?? "";
                    ws.Cell(row, 6).Value = x.CabinClass ?? "";
                    ws.Cell(row, 7).Value = x.JourneyType ?? "";
                    ws.Cell(row, 8).Value = x.ClassOb ?? "";
                    ws.Cell(row, 9).Value = x.ClassIb ?? "";
                    ws.Cell(row, 10).Value = x.MarkType ?? "";
                    ws.Cell(row, 11).Value = x.MarkUp ?? 0;

                    // ✅ TRUE / FALSE (EXACT)
                    ws.Cell(row, 12).Value = x.Status.HasValue ? x.Status.Value : false;

                    ws.Cell(row, 13).Value = x.MinDays ?? 0;
                    ws.Cell(row, 14).Value = x.MaxDays ?? 0;
                    ws.Cell(row, 15).Value = x.AdvanceDays ?? 0;
                    ws.Cell(row, 16).Value = x.MarkApplicable2 ?? 0;
                    ws.Cell(row, 17).Value = x.Rairline ?? "";
                    ws.Cell(row, 18).Value = x.Pairline ?? "";
                    ws.Cell(row, 19).Value = x.Stops ?? 0;
                    ws.Cell(row, 20).Value = x.Ttime ?? 0;
                    ws.Cell(row, 21).Value = x.RestrictedClass ?? "";
                    ws.Cell(row, 22).Value = x.MinimumAmount ?? "";
                    ws.Cell(row, 23).Value = x.PaxCount ?? 0;
                    ws.Cell(row, 24).Value = x.PaxType ?? "";

                    // ✅ DATE FORMAT (MATCH YOUR SAMPLE)
                    SetDate(ws.Cell(row, 25), x.TravelDateFrom);
                    SetDate(ws.Cell(row, 26), x.TravelDateTo);
                    SetDate(ws.Cell(row, 27), x.SaleDateFrom);
                    SetDate(ws.Cell(row, 28), x.SaleDateTo);
                    SetDate(ws.Cell(row, 29), x.BlockOdateFrom);
                    SetDate(ws.Cell(row, 30), x.BlockOdateTo);

                    ws.Cell(row, 31).Value = x.Company ?? "";
                    ws.Cell(row, 32).Value = x.CampId ?? "";
                    ws.Cell(row, 33).Value = x.UpdateBy ?? "";
                    ws.Cell(row, 34).Value = x.InboundTime ?? 0;
                    ws.Cell(row, 35).Value = x.OutboundTime ?? 0;

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "MarkupRules.xlsx"
                    );
                }
            }
        }

        private void SetDate(IXLCell cell, DateTime? date)
        {
            if (date.HasValue)
            {
                cell.Value = date.Value;
                cell.Style.DateFormat.Format = "yyyy-MM-dd"; // EXACT LIKE YOUR DATA
            }
            else
            {
                cell.Value = "";
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMarkupRule(
    MarkupRulesPageVM postData,
    IFormFile excelFile,
    string OriginsText,
    string DestinationsText,
    string AirlinesText,
    string CabinText,
    string AmountText)
        {
            AplicationHelper.Log("SaveMarkupRule started");

            try
            {
                if (excelFile != null && excelFile.Length > 0)
                {
                    AplicationHelper.Log("Excel upload detected");
                    await UploadMarkupExcelV1(excelFile);
                }

                var vm = postData.FormRule;

                vm.CabinClass = string.IsNullOrWhiteSpace(CabinText) ? null : vm.CabinClass;
                vm.PaxType = string.IsNullOrWhiteSpace(vm.PaxType) ? "Any" : vm.PaxType;
                vm.Stops = vm.Stops ?? -1;

                vm.Origins = SplitToSet(OriginsText);
                vm.Destinations = SplitToSet(DestinationsText);
                vm.Airlines = SplitToSet(AirlinesText);
                vm.CabinClasses = SplitToSet(CabinText);
                vm.MinimumAmountRange = ParseAmountRange(AmountText);

                if (vm.RuleId == 0)
                {
                    AplicationHelper.Log("Creating new rule");

                    var entity = new MarkupRule
                    {
                        RuleName = vm.RuleName,
                        Origin = string.Join(",", vm.Origins),
                        Destination = string.Join(",", vm.Destinations),
                        Airline = string.Join(",", vm.Airlines),
                        CabinClass = string.Join(",", vm.CabinClasses),

                        ClassOb = vm.ClassOb,
                        ClassIb = vm.ClassIb,

                        JourneyType = vm.JourneyType,
                        MarkType = vm.MarkType,
                        MarkUp = vm.MarkUp,
                        Status = vm.Status,

                        AdvanceDays = vm.AdvanceDays,
                        Stops = vm.Stops,

                        MinimumAmount = AmountText,

                        PaxCount = vm.PaxCount,
                        PaxType = vm.PaxType,

                        TravelDateFrom = vm.TravelDateFrom,
                        TravelDateTo = vm.TravelDateTo,
                        SaleDateFrom = vm.SaleDateFrom,
                        SaleDateTo = vm.SaleDateTo,
                        Rairline = vm.ReplaceAirline,
                        Pairline = vm.ParallelAirline,
                        Company = vm.Company,
                        CampId = vm.CampId,
                        UpdateBy = vm.UpdatedBy,
                        InboundTime = vm.InboundTime,
                        OutboundTime = vm.OutboundTime,
                    };

                    _context.MarkupRules.Add(entity);
                }
                else
                {
                    AplicationHelper.Log($"Updating rule ID: {vm.RuleId}");

                    var entity = _context.MarkupRules
                                         .FirstOrDefault(x => x.RuleId == vm.RuleId);

                    if (entity == null)
                    {
                        AplicationHelper.Log("Rule not found");
                        return NotFound();
                    }

                    entity.RuleName = vm.RuleName;
                    entity.Origin = string.Join(",", vm.Origins);
                    entity.Destination = string.Join(",", vm.Destinations);
                    entity.Airline = string.Join(",", vm.Airlines);
                    entity.CabinClass = string.Join(",", vm.CabinClasses);

                    entity.ClassOb = vm.ClassOb;
                    entity.ClassIb = vm.ClassIb;

                    entity.JourneyType = vm.JourneyType;
                    entity.MarkType = vm.MarkType;
                    entity.MarkUp = vm.MarkUp;
                    entity.Status = vm.Status;

                    entity.AdvanceDays = vm.AdvanceDays;
                    entity.Stops = vm.Stops;

                    entity.MinimumAmount = AmountText;
                    entity.PaxCount = vm.PaxCount;
                    entity.PaxType = vm.PaxType;
                    entity.SaleDateFrom = vm.SaleDateFrom;
                    entity.SaleDateTo = vm.SaleDateTo;
                    entity.TravelDateFrom = vm.TravelDateFrom;
                    entity.TravelDateTo = vm.TravelDateTo;
                    entity.Rairline = vm.ReplaceAirline;
                    entity.Pairline = vm.ParallelAirline;
                    entity.Company = vm.Company;
                    entity.CampId = vm.CampId;
                    entity.InboundTime = vm.InboundTime;
                    entity.OutboundTime = vm.OutboundTime;
                }

                _context.SaveChanges();

                AplicationHelper.Log("Database changes saved");
                if (postData != null)
                {
                    RefreshMarkupRulesFileV2(postData.FormRule.CampId);
                }


                AplicationHelper.Log("Markup rules file refreshed");

                TempData["Success"] = "Markup rule saved successfully.";

                AplicationHelper.Log("SaveMarkupRule success");

                return RedirectToAction("MarkupRulesView");
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Error: " + ex.Message);

                TempData["Error"] = "Something went wrong.";

                return RedirectToAction("MarkupRulesView");
            }
        }

        public async Task<IActionResult> UploadMarkupExcel(IFormFile excelFile)
        {
            AplicationHelper.Log("Upload started");

            if (excelFile == null || excelFile.Length == 0)
            {
                AplicationHelper.Log("File not found");
                TempData["Error"] = "File not found.";
                return RedirectToAction("MarkupRulesView");
            }

            if (Path.GetExtension(excelFile.FileName) != ".xlsx")
            {
                AplicationHelper.Log("Invalid file format");
                TempData["Error"] = "Only .xlsx Excel files allowed.";
                return RedirectToAction("MarkupRulesView");
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSFLY>();

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                List<MarkupRule> parsedRules = new List<MarkupRule>();

                using (var stream = excelFile.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var ws = workbook.Worksheet(1);
                    var rows = ws.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        var ruleName = GetString(row.Cell(2));

                        if (string.IsNullOrWhiteSpace(ruleName) || ruleName.ToLower() == "null")
                            continue;

                        parsedRules.Add(new MarkupRule
                        {
                            RuleName = GetString(row.Cell(2)),
                            Origin = GetString(row.Cell(3)),
                            Destination = GetString(row.Cell(4)),
                            Airline = GetString(row.Cell(5)),
                            CabinClass = GetString(row.Cell(6)),
                            JourneyType = GetString(row.Cell(7)),
                            ClassOb = GetString(row.Cell(8)),
                            ClassIb = GetString(row.Cell(9)),
                            MarkType = GetString(row.Cell(10)),
                            MarkUp = GetDecimalSafe(row.Cell(11)),
                            Status = GetBoolSafe(row.Cell(12)),
                            MinDays = GetIntSafe(row.Cell(13)),
                            MaxDays = GetIntSafe(row.Cell(14)),
                            AdvanceDays = GetIntSafe(row.Cell(15)),
                            MarkApplicable2 = GetDecimalSafe(row.Cell(16)),
                            Rairline = GetString(row.Cell(17)),
                            Pairline = GetString(row.Cell(18)),
                            Stops = GetIntSafe(row.Cell(19)),
                            Ttime = GetIntSafe(row.Cell(20)),
                            RestrictedClass = GetString(row.Cell(21)),
                            MinimumAmount = GetString(row.Cell(22)),
                            PaxCount = GetIntSafe(row.Cell(23)),
                            PaxType = GetString(row.Cell(24)),
                            TravelDateFrom = GetDateSafe(row.Cell(25)),
                            TravelDateTo = GetDateSafe(row.Cell(26)),
                            SaleDateFrom = GetDateSafe(row.Cell(27)),
                            SaleDateTo = GetDateSafe(row.Cell(28)),
                            BlockOdateFrom = GetDateSafe(row.Cell(29)),
                            BlockOdateTo = GetDateSafe(row.Cell(30)),
                            Company = GetString(row.Cell(31)),
                            CampId = GetString(row.Cell(32)),
                            UpdateBy = GetString(row.Cell(33)),
                            InboundTime = GetIntSafe(row.Cell(34)),
                            OutboundTime = GetIntSafe(row.Cell(35))
                        });
                    }
                }

                AplicationHelper.Log($"Parsed Rules Count: {parsedRules.Count}");

                if (!parsedRules.Any())
                {
                    AplicationHelper.Log("Excel empty");
                    TempData["Error"] = "Excel is empty.";
                    return RedirectToAction("MarkupRulesView");
                }

                parsedRules = parsedRules.Where(x => x != null && !string.IsNullOrWhiteSpace(x.RuleName)).ToList();

                var campId = parsedRules.First().CampId;

                if (string.IsNullOrWhiteSpace(campId))
                {
                    AplicationHelper.Log("CampId missing");
                    TempData["Error"] = "CampId missing.";
                    return RedirectToAction("MarkupRulesView");
                }

                AplicationHelper.Log("Deleting old data");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM MarkupRules");

                context.MarkupRules.AddRange(parsedRules);

                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                AplicationHelper.Log("Data saved successfully");

                await context.Database.ExecuteSqlRawAsync("DELETE FROM MarkupRules WHERE CampId IS NULL");

                RefreshMarkupRulesFileV1("Rush_FS");
                //RefreshMarkupRulesFile("Rush");

                AplicationHelper.Log("File refreshed");

                TempData["Success"] = "Uploaded successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                AplicationHelper.Log("Error: " + ex.Message);
                TempData["Error"] = "Upload failed";
            }

            AplicationHelper.Log("Upload finished");

            return RedirectToAction("MarkupRulesView");
        }

        public async Task<IActionResult> UploadMarkupExcelV1(IFormFile excelFile)
        {
            AplicationHelper.Log("Upload started");

            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "File not found.";
                return RedirectToAction("MarkupRulesView");
            }

            if (Path.GetExtension(excelFile.FileName) != ".xlsx")
            {
                TempData["Error"] = "Only .xlsx Excel files allowed.";
                return RedirectToAction("MarkupRulesView");
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSFLY>();

            try
            {
                List<MarkupRule> parsedRules = new List<MarkupRule>();

                using (var stream = excelFile.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var ws = workbook.Worksheet(1);
                    var rows = ws.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        var ruleName = GetString(row.Cell(2));

                        if (string.IsNullOrWhiteSpace(ruleName) || ruleName.ToLower() == "null")
                            continue;

                        parsedRules.Add(new MarkupRule
                        {
                            RuleName = GetString(row.Cell(2)),
                            Origin = GetString(row.Cell(3)),
                            Destination = GetString(row.Cell(4)),
                            Airline = GetString(row.Cell(5)),
                            CabinClass = GetString(row.Cell(6)),
                            JourneyType = GetString(row.Cell(7)),
                            ClassOb = GetString(row.Cell(8)),
                            ClassIb = GetString(row.Cell(9)),
                            MarkType = GetString(row.Cell(10)),
                            MarkUp = GetDecimalSafe(row.Cell(11)),
                            Status = GetBoolSafe(row.Cell(12)),
                            MinDays = GetIntSafe(row.Cell(13)),
                            MaxDays = GetIntSafe(row.Cell(14)),
                            AdvanceDays = GetIntSafe(row.Cell(15)),
                            MarkApplicable2 = GetDecimalSafe(row.Cell(16)),
                            Rairline = GetString(row.Cell(17)),
                            Pairline = GetString(row.Cell(18)),
                            Stops = GetIntSafe(row.Cell(19)),
                            Ttime = GetIntSafe(row.Cell(20)),
                            RestrictedClass = GetString(row.Cell(21)),
                            MinimumAmount = GetString(row.Cell(22)),
                            PaxCount = GetIntSafe(row.Cell(23)),
                            PaxType = GetString(row.Cell(24)),
                            TravelDateFrom = GetDateSafe(row.Cell(25)),
                            TravelDateTo = GetDateSafe(row.Cell(26)),
                            SaleDateFrom = GetDateSafe(row.Cell(27)),
                            SaleDateTo = GetDateSafe(row.Cell(28)),
                            BlockOdateFrom = GetDateSafe(row.Cell(29)),
                            BlockOdateTo = GetDateSafe(row.Cell(30)),
                            Company = GetString(row.Cell(31)),
                            CampId = GetString(row.Cell(32)),
                            UpdateBy = GetString(row.Cell(33)),
                            InboundTime = GetIntSafe(row.Cell(34)),
                            OutboundTime = GetIntSafe(row.Cell(35))
                        });
                    }
                }

                AplicationHelper.Log($"Parsed Rules Count: {parsedRules.Count}");

                if (!parsedRules.Any())
                {
                    TempData["Error"] = "Excel is empty.";
                    return RedirectToAction("MarkupRulesView");
                }

                var oldData = context.MarkupRules.ToList();
                context.MarkupRules.RemoveRange(oldData);
                await context.SaveChangesAsync();

                await context.MarkupRules.AddRangeAsync(parsedRules);
                await context.SaveChangesAsync();

                AplicationHelper.Log("Data saved successfully");

                RefreshMarkupRulesFileV1("Rush_FS");

                TempData["Success"] = "Uploaded successfully";
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Error: " + ex.Message);
                TempData["Error"] = "Upload failed";
            }

            AplicationHelper.Log("Upload finished");

            return RedirectToAction("MarkupRulesView");
        }
        private string GetString(IXLCell cell)
        {
            return cell?.GetValue<string>()?.Trim();
        }
        private decimal? GetDecimalSafe(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number)
            {
                try
                {
                    return cell.GetValue<decimal>();
                }
                catch
                {
                    return null;
                }
            }

            var val = cell.GetValue<string>()?.Trim();

            if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            return null;
        }
        private int? GetIntSafe(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number)
                return (int)cell.GetDouble();

            if (int.TryParse(cell.GetValue<string>(), out int result))
                return result;

            return null;
        }

        private bool? GetBoolSafe(IXLCell cell)
        {
            var val = cell.GetValue<string>()?.Trim().ToLower();

            if (val == "active" || val == "true" || val == "1")
                return true;

            if (val == "inactive" || val == "false" || val == "0")
                return false;

            return null;
        }
        private DateTime? GetDateSafe(IXLCell cell)
        {
            if (DateTime.TryParse(cell.GetValue<string>(), out DateTime result))
                return result;

            return null;
        }
        private void RefreshMarkupRulesFile(string campId)
        {
            AplicationHelper.Log("RefreshMarkupRulesFile started");
            try
            {
                if (campId != null)
                {
                    AplicationHelper.Log($"CampId: {campId}");



                    var dbRules = _context.MarkupRules
                                         .Where(x => x.CampId == campId)
                                         .AsNoTracking()
                                         .ToList();

                    AplicationHelper.Log($"Rules fetched from DB: {dbRules.Count}");

                    var rules = MapToDto(dbRules);

                    AplicationHelper.Log("Mapping completed");

                    AplicationHelper.SaveMarkupRulesToFile(rules, campId);

                    AplicationHelper.Log("File saved successfully");

                }
                else
                {
                    AplicationHelper.Log("CampId is null - skipped");
                }

                AplicationHelper.Log("RefreshMarkupRulesFile finished");

            }
            catch (Exception ex)
            {

                AplicationHelper.Log("RefreshMarkupRulesFile - Error" + ex.Message);
            }
            
        }
        private void RefreshMarkupRulesFileV1(string campId)
        {
            if(campId!=null&& campId == "Rush")
            {
                campId = "Rush";
            }
            else
            {
                campId = "Rush_FS";

            }
            AplicationHelper.Log("RefreshMarkupRulesFileV1 started");

            if (campId != null)
            {
                AplicationHelper.Log($"CampId: {campId}");




                var dbRules = _context.MarkupRules
                                     .Where(x => x.CampId == campId)
                                     .AsNoTracking()
                                     .ToList();

                AplicationHelper.Log($"Rules fetched from DB: {dbRules.Count}");

                var rules = MapToDto(dbRules);

                AplicationHelper.Log($"Mapping completed Rules Count {rules.Count}");

                AplicationHelper.SaveMarkupRulesToFileV1(rules, campId);

                AplicationHelper.Log("File saved successfully");

            }
            else
            {
                AplicationHelper.Log("CampId is null - skipped");
            }

            AplicationHelper.Log("RefreshMarkupRulesFile finished");
        }


        private void RefreshMarkupRulesFileV2(string campId)
        {
            //if (campId != null && campId == "Rush")
            //{
            //    campId = "Rush";
            //}
            //else
            //{
            //    campId = "Rush_FS";

            //}
            AplicationHelper.Log("RefreshMarkupRulesFileV1 started");

            if (campId != null)
            {
                AplicationHelper.Log($"CampId: {campId}");




                var dbRules = _context.MarkupRules
                                     .Where(x => x.CampId == campId)
                                     .AsNoTracking()
                                     .ToList();

                AplicationHelper.Log($"Rules fetched from DB: {dbRules.Count}");

                var rules = MapToDto(dbRules);

                AplicationHelper.Log($"Mapping completed Rules Count {rules.Count}");

                AplicationHelper.SaveMarkupRulesToFileV1(rules, campId);

                AplicationHelper.Log("File saved successfully");

            }
            else
            {
                AplicationHelper.Log("CampId is null - skipped");
            }

            AplicationHelper.Log("RefreshMarkupRulesFile finished");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMarkupRule(int id)
        {
            var rule = _context.MarkupRules.FirstOrDefault(x => x.RuleId == id);
            AplicationHelper.Log("DeleteMarkupRule RuleName " + rule.RuleName + "_id" + id.ToString());
            if (rule != null)
            {
                var campId = rule.CampId;

                _context.MarkupRules.Remove(rule);
                _context.SaveChanges();

                RefreshMarkupRulesFile(campId);
            }

            return RedirectToAction("MarkupRulesView");
        }


        private List<MarkupRuleDto> MapToDto(List<MarkupRule> dbRules)
        {
            return dbRules.Select(x => new MarkupRuleDto
            {
                RuleId = x.RuleId,
                RuleName = x.RuleName,

                /* ✅ MULTI VALUE PARSING */
                Origins = SplitToSet(x.Origin),
                Destinations = SplitToSet(x.Destination),
                Airlines = SplitToSet(x.Airline),
                CabinClasses = SplitToSet(x.CabinClass),
                CabinClass = x.CabinClass,
                JourneyType = x.JourneyType,
                MarkType = x.MarkType,

                MarkUp = x.MarkUp ?? 0,

                Status = x.Status ?? true,

                MinDays = x.MinDays,
                MaxDays = x.MaxDays,
                AdvanceDays = x.AdvanceDays,

                MarkApplicable2 = x.MarkApplicable2,

                /* ✅ AIRLINE SETS */
                RAirlines = SplitToSet(x.Rairline),
                PAirlines = SplitToSet(x.Pairline),

                Stops = x.Stops,

                /* ✅ RESTRICTED CLASS */
                RestrictedClass = SplitToSet(x.RestrictedClass),

                /* ✅ RANGE PARSE */
                MinimumAmountRange = ParseAmountRange(x.MinimumAmount),

                PaxCount = x.PaxCount,
                PaxType = string.IsNullOrWhiteSpace(x.PaxType) ? "ANY" : x.PaxType,

                TravelDateFrom = x.TravelDateFrom,
                TravelDateTo = x.TravelDateTo,
                ClassOb = x.ClassOb,
                ClassIb = x.ClassIb,
                BlockODateFrom = x.BlockOdateFrom,
                BlockODateTo = x.BlockOdateTo,
                Company = x.Company,
                CampId = x.CampId,
                UpdatedBy = x.UpdateBy,
                ReplaceAirline = x.Rairline,
                InboundTime = x.InboundTime ?? 0,
                OutboundTime = x.OutboundTime ?? 0,
                SaleDateFrom = x.SaleDateFrom,
                SaleDateTo = x.SaleDateTo,

            }).ToList();
        }
        private static HashSet<string> SplitToSet(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim().ToUpper())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static (decimal? min, decimal? max) ParseAmountRange(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (null, null);

            var parts = value.Split('-', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1 && decimal.TryParse(parts[0], out decimal single))
                return (single, single);

            if (parts.Length == 2 &&
                decimal.TryParse(parts[0], out decimal min) &&
                decimal.TryParse(parts[1], out decimal max))
                return (min, max);

            return (null, null);
        }



        [HttpPost]
        public IActionResult SendMessage(int bookingId, string message, int SenderId, int SenderRole)
        {
            try
            {
                SenderId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value; ;

                var entity = new BookingConversation
                {
                    BookingId = bookingId,
                    Message = message,
                    SenderId = SenderId,
                    SenderName = User.Identity.Name,
                    SenderRole = role,
                    CreatedDate = DateTime.Now
                };

                _context.BookingConversations.Add(entity);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        public IActionResult GetConversation(int bookingId)
        {
            var data = _context.BookingConversations
                .Where(x => x.BookingId == bookingId)
                .OrderBy(x => x.CreatedDate)
                .Select(x => new
                {
                    x.Message,
                    x.SenderName,
                    x.SenderRole,
                    Time = x.CreatedDate.ToString("dd MMM yyyy HH:mm")
                })
                .ToList();

            return Json(data);
        }

        [HttpGet]
        public IActionResult SeoPages(int? websiteId)
        {
            var model = new SeoPagesVM();

            model.Websites = _context.Websites
               .Select(w => new WebsiteDropdownVM
               {
                   Id = w.Id,
                   Name = w.Name
               })
               .ToList();

            if (websiteId.HasValue)
            {
                model.SelectedWebsiteId = websiteId;

                model.Pages = (
                    from p in _context.Pages
                    join s in _context.SeoEntities
                        on p.Id equals s.ReferenceId into seoGroup
                    from s in seoGroup.DefaultIfEmpty()   // LEFT JOIN

                    where p.WebsiteId == websiteId && p.Delind == false

                    select new SeoPageRowVM
                    {
                        Id = p.Id,
                        PageKey = p.PageKey,
                        Title = p.Title,
                        ViewName = p.ViewName,
                        IsActive = (bool)p.IsActive,
                        Slug = s != null ? s.Slug : null
                    }
                ).ToList();
            }
            else
            {
                model.Pages = new List<SeoPageRowVM>();
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult DeleteSeoPage(int id)
        {
            var page = _context.Pages.FirstOrDefault(x => x.Id == id);

            if (page == null)
                return NotFound();

            page.Delind = true;   // Soft Delete
            _context.SaveChanges();

            return Ok();
        }

        //[HttpGet]
        //public IActionResult PageSeoEditor(string pageKey)
        //{
        //    var model = _seoService.GetPageSeoEditorData(pageKey);

        //    if (model == null)
        //        return NotFound();

        //    return View(model);
        //}
        [HttpGet]
        public IActionResult PageSeoEditor(string pageKey, int id)
        {
            var websites = _context.Websites
                .Select(w => new WesbiteVM
                {
                    Id = w.Id,
                    Name = w.Name,
                    Domain = w.Domain
                })
                .ToList();

            if (string.IsNullOrEmpty(pageKey))
            {
                var model = new PageSeoEditorVM
                {
                    Page = new PageModel(),
                    Seo = new SeoEntities(),
                    Websites = websites   // ✅ Correct
                };

                return View(model);
            }

            var editModel = _seoService.GetPageSeoEditorData(pageKey, id);

            if (editModel == null)
                return NotFound();

            editModel.Websites = websites;   // ✅ Important

            return View(editModel);
        }

        [HttpPost]
        public IActionResult PageSeoEditor(PageSeoEditorVM model)
        {
            if (model.Page.FeatureImageFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "img/pages");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() +
                                  Path.GetExtension(model.Page.FeatureImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Page.FeatureImageFile.CopyTo(stream);
                }

                model.Page.FeatureImageUrl = "/img/pages/" + fileName;
            }

            _seoService.SavePageSeoEditorData(model);

            Notify("Page & SEO updated successfully", "success");

            return RedirectToAction("PageSeoEditor", new { pageKey = model.Page.PageKey });
        }


        //public IActionResult BlogList(int? websiteId)
        //{
        //    var model = new BlogListVM();

        //    model.Websites = _context.Websites.ToList();
        //    model.SelectedWebsiteId = websiteId;

        //    if (websiteId.HasValue)
        //    {
        //        model.Blogs = _context.BlogPosts
        //            .Where(x => x.WebsiteId == websiteId && !x.IsDeleted)
        //            .OrderByDescending(x => x.CreatedDate)
        //            .ToList();
        //    }
        //    else
        //    {
        //        model.Blogs = new List<BlogPost>();
        //    }

        //    return View(model);
        //}

        public async Task<IActionResult> BlogList(int? websiteId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Admin");

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Notify("Unauthorized. Please login again.", "error");
                    return RedirectToAction("Login", "Admin");
                }

                int currentUserId = int.Parse(userIdClaim);

                var currentUser = await _context.AdminUsers
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => new { u.UserId, u.RoleId })
                    .FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    Notify("User not found.", "error");
                    return RedirectToAction("Login", "Admin");
                }

                // ✅ ROLE RESTRICTION 🔥
                if (currentUser.RoleId != 1 &&
                    currentUser.RoleId != 2 &&
                    currentUser.RoleId != 8)
                {
                    Notify("You do not have permission to access Blog Management.", "error");
                    return RedirectToAction("Dashboard");
                }

                var model = new BlogListVM();

                model.Websites = await _context.Websites.ToListAsync();
                model.SelectedWebsiteId = websiteId;

                if (websiteId.HasValue)
                {
                    model.Blogs = await _context.BlogPosts
                        .Where(x => x.WebsiteId == websiteId && !x.IsDeleted)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToListAsync();
                    model.Domain = await _context.Websites
    .Where(x => x.Id == websiteId)
    .Select(x => x.Domain)
    .FirstOrDefaultAsync();
                }
                else
                {
                    model.Blogs = new List<BlogPost>();
                }

                return View(model);
            }
            catch
            {
                Notify("An error occurred while loading Blog Management.", "error");
                return RedirectToAction("Dashboard");
            }
        }
        public IActionResult CreateBlog(int websiteId)
        {
            var model = new BlogVM();
            model.Blog = new BlogPost();
            model.Blog.WebsiteId = websiteId;

            return View("CreateEditBlog", model);
        }
        [HttpPost]
        public IActionResult CreateBlog(BlogVM vm, IFormFile FeatureImageFile)
        {
            var model = vm.Blog;
            if (FeatureImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(FeatureImageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "img/blogs", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    FeatureImageFile.CopyTo(stream);
                }

                model.FeatureImageUrl = "/img/blogs/" + fileName;
            }

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = "Admin";
            model.IsDeleted = false;
            //model.Slug = model.Slug.Trim().ToLower().Replace(" ", "-");
            _context.BlogPosts.Add(model);
            _context.SaveChanges();

            // EXISTING FAQS
            var existingFaqs = _context.FaqMasters
                .Where(x => x.ReferenceType == "Blog"
                         && x.ReferenceId == (int)model.BlogPostId
                         && !x.Delind)
                .ToList();


            // SOFT DELETE REMOVED FAQS
            foreach (var oldFaq in existingFaqs)
            {
                if (!vm.FAQs.Any(x => x.FaqId == oldFaq.FaqId))
                {
                    oldFaq.Delind = true;
                    oldFaq.UpdatedDate = DateTime.Now;
                    oldFaq.UpdatedBy = "Admin";
                }
            }

            if (vm.FAQs.Count > 0){
                foreach (var faq in vm.FAQs)
                {
                    if (string.IsNullOrWhiteSpace(faq.Question) &&
                        string.IsNullOrWhiteSpace(faq.Answer))
                        continue;

                    if (faq.FaqId > 0)
                    {
                        var existingFaq = existingFaqs
                            .FirstOrDefault(x => x.FaqId == faq.FaqId);

                        if (existingFaq != null)
                        {
                            existingFaq.Question = faq.Question;
                            existingFaq.Answer = faq.Answer;
                            existingFaq.SortOrder = faq.SortOrder;
                            existingFaq.UpdatedDate = DateTime.Now;
                            existingFaq.UpdatedBy = "Admin";
                        }
                    }
                    else
                    {
                        _context.FaqMasters.Add(new FaqMaster
                        {
                            ReferenceType = "Blog",
                            ReferenceId = (int)model.BlogPostId,
                            Question = faq.Question,
                            Answer = faq.Answer,
                            SortOrder = faq.SortOrder,
                            IsActive = true,
                            Delind = false,
                            CreatedDate = DateTime.Now,
                            CreatedBy = "Admin"
                        });
                    }
                   

                }
                _context.SaveChanges();
            }
            // ADD / UPDATE FAQS
            
            return RedirectToAction("BlogList", new { websiteId = model.WebsiteId });
        }
        //public IActionResult EditBlog(int id)
        //{
        //    var blog = _context.BlogPosts.FirstOrDefault(x => x.BlogPostId == id);

        //    if (blog == null)
        //        return NotFound();

        //    return View("CreateEditBlog", blog);
        //}
        public IActionResult EditBlog(int id)
        {
            var blog = _context.BlogPosts.FirstOrDefault(x => x.BlogPostId == id);

            if (blog == null)
                return NotFound();

            var vm = new BlogVM
            {
                Blog = blog,
                FAQs = _context.FaqMasters
                    .Where(x => x.ReferenceType == "Blog"
                             && x.ReferenceId == id
                             && !x.Delind)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new FaqVM
                    {
                        FaqId = x.FaqId,
                        Question = x.Question,
                        Answer = x.Answer,
                        SortOrder = x.SortOrder ?? 0
                    }).ToList()
            };

            return View("CreateEditBlog", vm);
        }
        //[HttpPost]
        //public IActionResult EditBlog(BlogPost model, IFormFile FeatureImageFile)
        //{
        //    var blog = _context.BlogPosts.FirstOrDefault(x => x.BlogPostId == model.BlogPostId);

        //    if (blog == null)
        //        return NotFound();

        //    if (FeatureImageFile != null)
        //    {
        //        var fileName = Guid.NewGuid() + Path.GetExtension(FeatureImageFile.FileName);
        //        var path = Path.Combine(_env.WebRootPath, "img/blogs", fileName);

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            FeatureImageFile.CopyTo(stream);
        //        }

        //        blog.FeatureImageUrl = "/img/blogs/" + fileName;
        //    }

        //    blog.Title = model.Title;
        //    blog.SubHeading = model.SubHeading;
        //    blog.Content = model.Content;
        //    blog.MetaTitle = model.MetaTitle;
        //    blog.MetaDescription = model.MetaDescription;
        //    blog.MetaKeywords = model.MetaKeywords;
        //    blog.Slug = model.Slug.Trim().ToLower().Replace(" ", "-");
        //    blog.Status = model.Status;

        //    blog.EditedDate = DateTime.Now;
        //    blog.EditedBy = "Admin";

        //    _context.SaveChanges();
        //    Notify("Blog updated successfully", "success");
        //    return RedirectToAction("EditBlog", new { id = blog.BlogPostId });

        //    //return RedirectToAction("BlogList", new { websiteId = blog.WebsiteId });
        //}

        [HttpPost]
        public IActionResult EditBlog(BlogVM model, IFormFile FeatureImageFile)
        {
            var blog = _context.BlogPosts
                .FirstOrDefault(x => x.BlogPostId == model.Blog.BlogPostId);

            if (blog == null)
                return NotFound();

            if (FeatureImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(FeatureImageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "img/blogs", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    FeatureImageFile.CopyTo(stream);
                }

                blog.FeatureImageUrl = "/img/blogs/" + fileName;
            }

            // BLOG UPDATE
            blog.Title = model.Blog.Title;
            blog.SubHeading = model.Blog.SubHeading;
            blog.Content = model.Blog.Content;
            blog.MetaTitle = model.Blog.MetaTitle;
            blog.MetaDescription = model.Blog.MetaDescription;
            blog.MetaKeywords = model.Blog.MetaKeywords;
            blog.Slug = model.Blog.Slug.Trim().ToLower().Replace(" ", "-");
            blog.Status = model.Blog.Status;
            blog.EditedDate = DateTime.Now;
            blog.EditedBy = "Admin";


            // EXISTING FAQS
            var existingFaqs = _context.FaqMasters
                .Where(x => x.ReferenceType == "Blog"
                         && x.ReferenceId == (int)blog.BlogPostId
                         && !x.Delind)
                .ToList();


            // SOFT DELETE REMOVED FAQS
            foreach (var oldFaq in existingFaqs)
            {
                if (!model.FAQs.Any(x => x.FaqId == oldFaq.FaqId))
                {
                    oldFaq.Delind = true;
                    oldFaq.UpdatedDate = DateTime.Now;
                    oldFaq.UpdatedBy = "Admin";
                }
            }


            // ADD / UPDATE FAQS
            foreach (var faq in model.FAQs)
            {
                if (string.IsNullOrWhiteSpace(faq.Question) &&
                    string.IsNullOrWhiteSpace(faq.Answer))
                    continue;

                if (faq.FaqId > 0)
                {
                    var existingFaq = existingFaqs
                        .FirstOrDefault(x => x.FaqId == faq.FaqId);

                    if (existingFaq != null)
                    {
                        existingFaq.Question = faq.Question;
                        existingFaq.Answer = faq.Answer;
                        existingFaq.SortOrder = faq.SortOrder;
                        existingFaq.UpdatedDate = DateTime.Now;
                        existingFaq.UpdatedBy = "Admin";
                    }
                }
                else
                {
                    _context.FaqMasters.Add(new FaqMaster
                    {
                        ReferenceType = "Blog",
                        ReferenceId = (int)blog.BlogPostId,
                        Question = faq.Question,
                        Answer = faq.Answer,
                        SortOrder = faq.SortOrder,
                        IsActive = true,
                        Delind = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin"
                    });
                }
            }

            _context.SaveChanges();

            Notify("Blog updated successfully", "success");

            return RedirectToAction("EditBlog", new { id = blog.BlogPostId });
        }

        public IActionResult ManageCampaign(int? editId)
        {
            var vm = new CampaignPageVM();

            vm.Campaigns = _context.TblCampaigns
                .Select(x => new CampaignDto
                {
                    Id = x.Id,
                    Status = (bool)x.Status,
                    PaxCount = (int)x.PaxCount,
                    CampID = x.CampId,
                    Company = x.Company,
                    UpdatedBy = x.UpdatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ExtraMarkup = x.ExtraMarkup

                }).ToList();

            if (editId.HasValue)
            {
                vm.Form = vm.Campaigns.FirstOrDefault(x => x.Id == editId);
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveCampaign(CampaignPageVM model)
        {
            if (model?.Form == null)
                return RedirectToAction("ManageCampaign");

            var form = model.Form;


            if (form.Id > 0)
            {
                var existing = _context.TblCampaigns.FirstOrDefault(x => x.Id == form.Id);

                if (existing != null)
                {
                    existing.Status = form.Status;
                    existing.PaxCount = form.PaxCount;
                    existing.CampId = form.CampID;
                    existing.Company = form.Company;
                    existing.UpdatedBy = form.UpdatedBy;
                    existing.UpdatedAt = DateTime.Now;
                    existing.ExtraMarkup = form.ExtraMarkup;
                    _context.SaveChanges();
                }
            }
            else
            {
                // 🔹 INSERT CASE
                var entity = new Datamodel.TblCampaign
                {
                    Status = form.Status,
                    PaxCount = form.PaxCount,
                    CampId = form.CampID,
                    Company = form.Company,
                    UpdatedBy = form.UpdatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ExtraMarkup = form.ExtraMarkup
                };

                _context.TblCampaigns.Add(entity);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageCampaign");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCampaign(int id)
        {
            var data = _context.TblCampaigns.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {
                _context.TblCampaigns.Remove(data);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageCampaign");
        }

        [HttpPost]
        public IActionResult SendCancellationEmail(int bookingId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Admin");
                }

                // 🔹 Get current user ID from cookie claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return RedirectToAction("Login", "Admin");

                int currentUserId = int.Parse(userIdClaim);

                // 🔹 Get current user's role from DB
                var currentUser =  _context.AdminUsers
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => new { u.UserId, u.RoleId })
                    .FirstOrDefault();

                if(currentUser.RoleId!=1 && currentUser.RoleId!=2 && currentUser.RoleId != 3)
                {
                    return Json(new { success = false, message = "Not Authorized" });
                }
                var booking = _context.FlightBookings
                                      .FirstOrDefault(x => x.BookingId == bookingId);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found" });
                }
                else
                {
                    var result = sendCancellationEmailtoCustomer(bookingId);

                    if (result)
                    {
                        booking.IsStatus = "Cancelled";
                        _context.SaveChanges();
                    }

                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public bool sendCancellationEmailtoCustomer(int BookingID)
        {
            try
            {
                var GetBookingDetails = _context.FlightBookings.Where(x => x.BookingId == BookingID).FirstOrDefault();
                var Flights = _context.BookingFlghtDetails.Where(x => x.BookingId == BookingID).OrderBy(x => x.BookingId).ToList();
                List<FlightsSegment> InBoundFlights = new List<FlightsSegment>();
                List<FlightsSegment> OutBoundFlights = new List<FlightsSegment>();
                foreach (var item in Flights)
                {
                    FlightsSegment fs = new FlightsSegment();
                    fs.Airline = Common.GetAirline(item.Airline);
                    if (item.DepDate != null)
                    {
                        fs.DepartureDate = (DateTime)item.DepDate;
                    }
                    if (item.ArrDate != null)
                    {
                        fs.ArrivalDate = (DateTime)item.ArrDate;
                    }
                    fs.FromAirport = Common.GetAirport(item.Dcode);
                    fs.ToAirport = Common.GetAirport(item.Gcode);
                    if (item.DepDate != null)
                    {
                        fs.DepDate = item.DepartOn + "," + fs.DepartureDate.DayOfWeek + " " + fs.DepartureDate.ToString("dd MMM yyyy");
                    }
                    if (item.ArrDate != null)
                    {
                        fs.ArrDate = item.ArriveOn + "," + fs.ArrivalDate.DayOfWeek + " " + fs.ArrivalDate.ToString("dd MMM yyyy");
                    }
                    fs.FlightNo = item.FlightNo;
                    if (item.ArrDate != null)
                    {
                        TimeSpan ts = (DateTime)item.ArrDate - (DateTime)item.DepDate;
                        fs.TotalTime = new DateTime(ts.Ticks).ToString("HH:mm");
                    }
                    if (item.FlightType == "InBound")
                        InBoundFlights.Add(fs);
                    else
                        OutBoundFlights.Add(fs);
                }
                var GetPaxDetails = _context.BookingPaxDetails.Where(x => x.BookingId == BookingID /* && x.Delind == false */).ToList();
                EmailbookingModel be = new EmailbookingModel();
                be.InBoundFlights = InBoundFlights;
                be.OutBoundFlights = OutBoundFlights;
                be.FlightBooking = GetBookingDetails;
                be.BookingPaxDetail = GetPaxDetails;
                be.FromLocation = Common.GetAirport(GetBookingDetails.FromDeparture);
                be.ToLocation = Common.GetAirport(GetBookingDetails.ToDestination);

                if (InBoundFlights.Count > 0)
                {
                    DateTime Departure = InBoundFlights.Select(x => x.DepartureDate).First();
                    DateTime Arrival = InBoundFlights.Select(x => x.ArrivalDate).Last();
                    TimeSpan tse = Arrival - Departure;
                    be.ToTime = new DateTime(tse.Ticks).ToString("HH:mm");
                }
                if (OutBoundFlights.Count > 0)
                {

                    DateTime Departure = OutBoundFlights.Select(x => x.DepartureDate).First();
                    DateTime Arrival = OutBoundFlights.Select(x => x.ArrivalDate).Last();
                    TimeSpan tsemo = Arrival - Departure;
                    be.FromTime = new DateTime(tsemo.Ticks).ToString("HH:mm");
                }
                var value = RenderInviteView(be);
                var Body = value.Result.Value.ToString();

                var tos = new List<EmailAddress>();
                tos.Add(new EmailAddress(TravelWeb.Utilities.Common.BccMail.ToString()));
                tos.Add(new EmailAddress(GetBookingDetails.ContactEmail));
                var from = new EmailAddress("care@rushmyflight.com", "RushMyFlight");
                var subject = "Cancellation Details";
                var htmlContent = Body;

                var message = new System.Net.Mail.MailMessage();
                message.Subject = "Cancellation Details";
                message.Body = Body;
                var EmailId = TravelWeb.Utilities.Common.EmailAddress;
                message.From = new MailAddress(EmailId, "RushMyFlight");
                message.Bcc.Add(TravelWeb.Utilities.Common.BccMail.ToString());
                message.To.Add(GetBookingDetails.ContactEmail);
                message.IsBodyHtml = true;
                SmtpClient smtp = smtp = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Host = TravelWeb.Utilities.Common.Host,
                    Port = 587,
                    UseDefaultCredentials = true
                };
                smtp.Credentials = new NetworkCredential(TravelWeb.Utilities.Common.EmailAddress, TravelWeb.Utilities.Common.Password);
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString() + ex.StackTrace.ToString());
                return false;
            }

        }
        public async Task<JsonResult> RenderInviteView(EmailbookingModel be)
        {
            var jsion = JsonConvert.SerializeObject(be);
            var htmls = await RenderAsync("CancellationMail.cshtml", be);
            return Json(htmls);
        }


        [HttpPost]
        public IActionResult UpdateBookingStatus(int bookingId, string status)
        {
            try
            {
                var booking = _context.FlightBookings
                                      .FirstOrDefault(x => x.BookingId == bookingId);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found" });
                }

                // ✅ Update status
                booking.Status = status;


                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [Authorize]
        public async Task<IActionResult> DeepLinkTracker(int page = 1, int pageSize = 50)
        {
            var allowedSizes = new[] { 50, 100, 200, 300, 500 };
            if (!allowedSizes.Contains(pageSize))
                pageSize = 50;

            var query = _context.DeepLinkTrackers.AsNoTracking();

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DeepLinkTrackerListItemVM
                {
                    Id = x.Id,
                    SearchId = x.SearchId,
                    Source = x.Source,
                    FromLocation = x.FromLocation,
                    ToLocation = x.ToLocation,
                    DepartureDate = x.DepartureDate,
                    ReturnDate = x.ReturnDate,
                    TripType = x.TripType,
                    Adults = x.Adults,
                    Children = x.Children,
                    Infants = x.Infants,
                    Airline = x.Airline,
                    Stops = x.Stops,
                    TotalAmount = x.TotalAmount,
                    Markup = x.Markup,
                    Stage = x.Stage
                })
                .ToListAsync();

            // ⭐ LAST 3 DAYS FS COUNT
            var last3Days = DateTime.UtcNow.AddDays(-3);

            var fsCount = await _context.DeepLinkTrackers
                .Where(x => x.Source == "FS" && x.CreatedAt >= last3Days)
                .CountAsync();

            var vm = new DeepLinkTrackerListVM
            {
                Records = data,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Last3DaysFSCount = fsCount
            };

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            var data = await _context.DeepLinkTrackers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
                return NotFound();

            return Json(data); // return full object
        }

        [HttpPost]
        public IActionResult DeleteBooking(int bookingId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Admin");
                }

                // 🔹 Get current user ID from cookie claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return RedirectToAction("Login", "Admin");

                int currentUserId = int.Parse(userIdClaim);

                // 🔹 Get current user's role from DB
                var currentUser = _context.AdminUsers
                    .Where(u => u.UserId == currentUserId)
                    .Select(u => new { u.UserId, u.RoleId })
                    .FirstOrDefault();

                if (currentUser.RoleId != 1 && currentUser.RoleId != 2 && currentUser.RoleId != 3)
                {
                    return Json(new { success = false, message = "Not Authorized" });
                }
                var booking = _context.FlightBookings
                                     .FirstOrDefault(x => x.BookingId == bookingId);

                if (booking == null)
                    return Json(new { success = false, message = "Booking not found" });

                booking.DelInd = true;


                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ExportTrackerDetails(
    string filter = "U",
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string rootSourceFilter = null,
    string fromLocationFilter = null,
    string toLocationFilter = null,
    string tripTypeFilter = null)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Admin");

            var query = _context.DeepLinkTrackers
                                .AsNoTracking()
                                .AsQueryable();

            if (filter == "U")
            {
                query = query.Where(x => x.TotalAmount == null || x.TotalAmount == 0);
            }
            else if (filter == "C")
            {
                query = query.Where(x => x.TotalAmount != null && x.TotalAmount > 0);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedAt <= endOfDay);
            }
            if (!string.IsNullOrWhiteSpace(rootSourceFilter))
            {
                query = query.Where(x => x.RootSource == rootSourceFilter);
            }
            if (!string.IsNullOrWhiteSpace(fromLocationFilter))
            {
                query = query.Where(x => x.FromLocation.Contains(fromLocationFilter));
            }
            if (!string.IsNullOrWhiteSpace(toLocationFilter))
            {
                query = query.Where(x => x.ToLocation.Contains(toLocationFilter));
            }
            if (!string.IsNullOrWhiteSpace(tripTypeFilter))
            {
                if (tripTypeFilter == "ROUND")
                {
                    query = query.Where(x => x.TripType == "R" || x.TripType.ToUpper() == "ROUND");
                }
                else if (tripTypeFilter == "ONEWAY")
                {
                    query = query.Where(x => x.TripType != "R" && x.TripType.ToUpper() != "ROUND");
                }
            }

            var records = await query
                .OrderByDescending(x => x.Id)
                .Select(x => new TrackerVM
                {
                    Id = x.Id,
                    IPAddress = x.Ipaddress,
                    RootSource = x.RootSource,
                    From = x.FromLocation,
                    To = x.ToLocation,
                    Airline = x.Airline,
                    SubClass = x.SubClass,
                    Depart = x.DepartureDate,
                    Return = x.ReturnDate,
                    TripType = string.Equals(x.TripType, "R", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(x.TripType, "ROUND", StringComparison.OrdinalIgnoreCase)
                               ? "ROUND"
                               : "ONEWAY",
                    ADT = x.Adults,
                    CHD = x.Children,
                    INF = x.Infants,
                    CreatedDate = x.CreatedAt,
                    TotalFare = x.PriceWithoutMarkup,
                    Markup = x.Markup,
                    TotalAfterMarkup = x.TotalAmount,
                    Rule = x.Rule
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("TrackerDetails");

            // Header row
            string[] headers = {
        "IP", "Root", "From", "To", "Airline", "Sub-Class",
        "Depart", "Return", "Trip", "ADT", "CHD", "INF",
        "Created", "Fare", "Markup", "After Markup", "Rule"
    };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
                sheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Data rows
            int row = 2;
            foreach (var t in records)
            {
                sheet.Cell(row, 1).Value = t.IPAddress;
                sheet.Cell(row, 2).Value = t.RootSource;
                sheet.Cell(row, 3).Value = t.From;
                sheet.Cell(row, 4).Value = t.To;
                sheet.Cell(row, 5).Value = t.Airline;
                sheet.Cell(row, 6).Value = t.SubClass;
                sheet.Cell(row, 7).Value = t.Depart?.ToString("dd MMM yyyy") ?? "";
                sheet.Cell(row, 8).Value = t.Return?.ToString("dd MMM yyyy") ?? "";
                sheet.Cell(row, 9).Value = t.TripType;
                sheet.Cell(row, 10).Value = t.ADT;
                sheet.Cell(row, 11).Value = t.CHD;
                sheet.Cell(row, 12).Value = t.INF;
                sheet.Cell(row, 13).Value = t.CreatedDate?.ToString("yyyy-MM-dd HH:mm") ?? "";
                sheet.Cell(row, 14).Value = t.TotalFare;
                sheet.Cell(row, 15).Value = t.Markup;
                sheet.Cell(row, 16).Value = t.TotalAfterMarkup;
                sheet.Cell(row, 17).Value = t.Rule;
                row++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"TrackerDetails_{filter}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

    }


}
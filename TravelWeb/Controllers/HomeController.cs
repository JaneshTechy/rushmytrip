using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TravelWeb.Datamodel;
using TravelWeb.Interface;
using TravelWeb.Models;
using TravelWeb.Utilities;


namespace TravelWeb.Controllers
{

    public class HomeController : Controller
    {
        private readonly ISeoService _seoService;
        private readonly RSFLY _context;
        private readonly ICompositeViewEngine _viewEngine;
        public HomeController(ISeoService seoService, RSFLY context, ICompositeViewEngine viewEngine)
        {
            _seoService = seoService;
            _context = context;
            _viewEngine = viewEngine;
        }
        public IActionResult Index()
        {
            var model = new HomeViewModel();
            var domain = HttpContext.Request.Host.Value;
            domain = "rushmyflight.com";
            model.Seo = _seoService.GetSeo("Home", null,domain);

            
            return View("IndexNew", model);

        }

        //[Route("privacy-policy")]
        //[Route("{folder}/privacy-policy")]
        //public IActionResult PrivacyPolicy()
        //{
        //    return View();
        //}
        //[Route("about-us")]
        //[Route("{folder}/about-us")]
        //public IActionResult AboutUs()
        //{
        //    var model = new AboutUsViewModel();

        //    var slug = HttpContext.Request.Path.Value;

        //    // ✅ Normalize BEFORE DB Call
        //    if (slug.EndsWith("/about-us"))
        //        slug = "/about-us";

        //    model.Seo = _seoService.GetSeoBySlug(slug);

        //    return View(model);
        //}

        //[Route("contact-us")]
        //[Route("{folder}/contact-us")]
        //public IActionResult ContactUs()
        //{
        //    return View();
        //}
        //[Route("terms-conditions")]
        //[Route("{folder}/terms-conditions")]
        //public IActionResult TermsConditions()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult GetContactDetail(Contact contactDetails)
        {
            var message = new System.Net.Mail.MailMessage();
            message.Subject = "ContactUs Form Details";
            //message.Body = stringBuilder.ToString() + stringBuilder3.ToString() + stringBuilder4.ToString() + stringBuilder2.ToString();
            message.Body = "User Name: " + contactDetails.UserName + "<br>" + "User Message: " + contactDetails.UserMessage + "<br>" + "send by user: " + contactDetails.UserEmail;
            var EmailId = TravelWeb.Utilities.Common.EmailAddress;
            message.From = new MailAddress(EmailId);
            message.To.Add(EmailId);



            foreach (var BccAddress in TravelWeb.Utilities.Common.BccMail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.Bcc.Add(BccAddress);
            }
            // message.Bcc.Add(TravelWeb.Utilities.Common.BccMail.ToString());
            message.IsBodyHtml = true;
            SmtpClient smtp =
            smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                Host = TravelWeb.Utilities.Common.Host,
                Port = 587,
                UseDefaultCredentials = false
            };
            smtp.Credentials = new NetworkCredential(TravelWeb.Utilities.Common.EmailAddress, TravelWeb.Utilities.Common.Password);
            //Task.Factory.StartNew(() => smtp.Send(message));
            try
            {
                smtp.Send(message);
                return Json("Thank you for contacting us, we will get back to you as soon as possible.");
            }
            catch (Exception ex)
            {
                return Json("Something went wrong!");
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult SaveApiResponse(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
                return Content("No file selected");

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/Data/APiResponseFlights.txt"
            );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                jsonFile.CopyTo(stream);
            }

            return RedirectToAction("APiResponseFlight");
        }
        private string ApiFilePath
        {
            get
            {   
                return Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/Data/APiResponseFlights.txt");
            }
        }

        //public IActionResult Page(string slug)
        //{

        //    var domain = HttpContext.Request.Host.Value;
        //    domain = "rushmyflight.com";
        //    var page = _seoService.GetPageBySlug(domain, slug);

        //    if (page == null)
        //        return NotFound();

        //    ViewBag.Seo = _seoService.GetSeoData();

        //    return View(page.ViewName, page);
        //}

        public IActionResult Page(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var domain = HttpContext.Request.Host.Value;
            domain = "rushmyflight.com";

            var page = _seoService.GetPageBySlug(domain, slug);

            if (page == null)
                return NotFound();

            // SEO Bind
            ViewBag.Seo = _seoService.GetSeoData(page.Id);


            
            if (page.ViewName != null&& page.ViewName != "")
            {
                var viewResult = _viewEngine.FindView(ControllerContext, page.ViewName, false);
                return View(viewResult.ViewName, page);
            }
                //        return NotFound();

                // Otherwise open generic dynamic view
                return View("DynamicPage", page);
        }
        [Route("blog")]

        public IActionResult Blog(int page = 1, int pageSize = 6)
        {
            int WebsiteId = 1;

            var totalBlogs = _context.BlogPosts
                .Where(x => !x.IsDeleted && x.Status != null && x.Status.ToLower() != "draft" && x.WebsiteId == WebsiteId)
                .Count();

            var blogs = _context.BlogPosts
                .Where(x => !x.IsDeleted && x.Status != null && x.Status.ToLower() != "draft" && x.WebsiteId == WebsiteId)
                .OrderByDescending(x => x.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBlogs / pageSize);
            ViewBag.CurrentPage = page;

            return View(blogs);
        }

        [HttpGet]
        public IActionResult LoadBlogs(int skip = 0, int take = 5, string? search = "")
        {
            int WebsiteId = 1;

            var query = _context.BlogPosts
                .Where(x => !x.IsDeleted && x.Status != null && x.Status.ToLower() != "draft" && x.WebsiteId == WebsiteId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }

            var blogs = query
                .OrderByDescending(x => x.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToList();

            return PartialView("_BlogListPartial", blogs);
        }
        //[Route("blog/{slug}")]
        //public IActionResult BlogDetails(string slug)
        //{
        //    var blog = _context.BlogPosts
        //        .FirstOrDefault(x => x.Slug == slug
        //                            && !x.IsDeleted
        //                            && x.Status != null
        //                            && x.Status.ToLower() != "draft");
        //    if (blog == null) return NotFound();

        //    return View(blog);
        //}
        
        [Route("blog/{slug}")]
public IActionResult BlogDetails(string slug)
{
    var BlogModel = new BlogDetailModel();
    var WebsiteId = 1;

    BlogModel.RecentPosts = _context.BlogPosts
        .Where(x => !x.IsDeleted &&
                    x.Status != null &&
                    x.Status.ToLower() != "draft" &&
                    x.WebsiteId == WebsiteId)
        .OrderByDescending(x => x.CreatedDate)
        .Take(5)
        .ToList();

    BlogModel.blog = _context.BlogPosts
        .FirstOrDefault(x => x.Slug == slug &&
                             !x.IsDeleted &&
                             x.Status != null &&
                             x.Status.ToLower() != "draft" &&
                             x.WebsiteId == WebsiteId);

    if (BlogModel.blog == null)
        return NotFound();

    BlogModel.FAQs = _context.FaqMasters
        .Where(x => x.ReferenceType == "Blog"
                 && x.ReferenceId == (int)BlogModel.blog.BlogPostId
                 && x.IsActive == true
                 && x.Delind!=true)
        .OrderBy(x => x.SortOrder)
        .ToList();

    return View(BlogModel);
}
    }

}

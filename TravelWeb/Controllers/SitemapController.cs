using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using TravelWeb.Datamodel;

[Route("sitemap.xml")]
public class SitemapController : Controller
{
    private readonly RSFLY _context;
    private readonly IWebHostEnvironment _env;

    public SitemapController(IWebHostEnvironment env, RSFLY context)
    {
        _env = env;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var sitemap = new StringBuilder();

        sitemap.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sitemap.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        // 🔹 Static pages
        AddUrl(sitemap, baseUrl, "/", "daily", "1.0");
        AddUrl(sitemap, baseUrl, "/about-us", "weekly", "0.8");
        AddUrl(sitemap, baseUrl, "/disclaimer ", "weekly", "0.8");
        AddUrl(sitemap, baseUrl, "/contact-us", "weekly", "0.8");
        AddUrl(sitemap, baseUrl, "/privacy-policy", "weekly", "0.8");
        AddUrl(sitemap, baseUrl, "/terms-and-conditions", "weekly", "0.8");
        AddUrl(sitemap, baseUrl, "/blog", "weekly", "0.8");

        
        var WebsiteId = 1;
        var totalBlogs = _context.BlogPosts
            .Where(x => !x.IsDeleted && x.Status != null && x.Status.ToLower() != "draft" 
             && x.WebsiteId == WebsiteId).ToList();

            foreach (var blog in totalBlogs)
            {
                AddUrl(
                    sitemap,
                    baseUrl,
                    $"/blog/{blog.Slug}",
                    "weekly",
                    "0.7",
                    blog.CreatedDate
                );
            }

        sitemap.AppendLine("</urlset>");

        return Content(sitemap.ToString(), "application/xml");
    }

    private void AddUrl(
        StringBuilder sb,
        string baseUrl,
        string path,
        string changeFreq,
        string priority,
        DateTime? lastMod = null)
    {
        sb.AppendLine("<url>");
        sb.AppendLine($"<loc>{baseUrl}{path}</loc>");
        sb.AppendLine($"<lastmod>{(lastMod ?? DateTime.UtcNow):yyyy-MM-dd}</lastmod>");
        sb.AppendLine($"<changefreq>{changeFreq}</changefreq>");
        sb.AppendLine($"<priority>{priority}</priority>");
        sb.AppendLine("</url>");
    }
}
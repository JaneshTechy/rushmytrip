using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelWeb.Datamodel;
using TravelWeb.Interface;
using TravelWeb.Models;
using PageModel = TravelWeb.Models.PageModel;

namespace TravelWeb.Services
{
    public class SeoService : ISeoService
    {
        private readonly RSFLY _context;
        private readonly ILogger<SeoService> _logger;
        private TravelWeb.Models.SeoEntities _seoData;

        public SeoService(RSFLY context, ILogger<SeoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public SeoEntities GetSeo(string pageType, int? referenceId = null,string domain= null)
        {
            try
            {
                if (domain == null)
                {
                    return _context.SeoEntities
                    .AsNoTracking()
                    .Where(x =>
                                x.PageType == pageType &&
                                x.Delind == false&& x.ReferenceId== referenceId)
                    .Select(x => new SeoEntities
                    {
                        MetaTitle = x.MetaTitle,
                        MetaDescription = x.MetaDescription,
                        MetaKeywords = x.MetaKeywords,
                        CanonicalUrl = x.CanonicalUrl,
                        OgTitle = x.OgTitle,
                        OgType = x.OgType,
                        OgDescription = x.OgDescription,
                        OgImageUrl = x.OgImageUrl,
                        Robots = x.Robots,
                        SchemaJson = x.SchemaJson,
                        Slug = x.Slug,
                        
                    })
                    .FirstOrDefault();
                }
                else
                {
                    var website = _context.Websites
                                  .AsNoTracking()
                                  .FirstOrDefault(w => w.Domain == domain);

                    var Pagesinfo = _context.Pages.Where(x => x.WebsiteId == website.Id && x.PageKey == "Home").FirstOrDefault();



                    return _context.SeoEntities
                        .AsNoTracking()
                        .Where(x =>
                                    x.ReferenceId == Pagesinfo.Id &&
                                    x.Delind == false)
                        .Select(x => new SeoEntities
                        {
                            MetaTitle = x.MetaTitle,
                            MetaDescription = x.MetaDescription,
                            MetaKeywords = x.MetaKeywords,
                            CanonicalUrl = x.CanonicalUrl,
                            OgTitle = x.OgTitle,
                            OgType = x.OgType,
                            OgDescription = x.OgDescription,
                            OgImageUrl = x.OgImageUrl,
                            Robots = x.Robots,
                            SchemaJson = x.SchemaJson,
                            Slug = x.Slug,
                            AltImageText = Pagesinfo.AltImageText
                        })
                        .FirstOrDefault();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SEO Fetch Failed");

                return GetDefaultSeo();
            }
        }

        private SeoEntities GetDefaultSeo()
        {
            return new SeoEntities
            {
                MetaTitle = "RushMyFlight",
                MetaDescription = "Find and book the best flight deals.",
                Robots = "noindex, nofollow"
            };
        }

        public SeoEntities GetSeoBySlug(string slug)
        {
            return _context.SeoEntities
                .AsNoTracking()
                .Where(x => x.Slug == slug && x.Delind == false)
                .Select(x => new SeoEntities
                {
                    MetaTitle = x.MetaTitle,
                    MetaDescription = x.MetaDescription,
                    CanonicalUrl = x.CanonicalUrl,
                    OgTitle = x.OgTitle,
                    OgDescription = x.OgDescription,
                    OgImageUrl = x.OgImageUrl,
                    Robots = x.Robots,
                    SchemaJson = x.SchemaJson
                })
                .FirstOrDefault();
        }

        public Models.PageModel GetPageBySlug(string domain, string slug)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                    return null;

                slug = slug.Trim().ToLower();

                var website = _context.Websites
                    .AsNoTracking()
                    .FirstOrDefault(w => w.Domain == domain);

                if (website == null)
                    return null;

                var formattedSlug = slug.StartsWith("/") ? slug : "/" + slug;

                var seo = _context.SeoEntities
                    .AsNoTracking()
                    .Where(s => s.WebsiteId == website.Id &&
                                s.Slug.ToLower() == slug &&
                                s.Delind == false)
                    .FirstOrDefault();

                if (seo == null)
                    return null;

                var page = _context.Pages
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == seo.ReferenceId &&
                                         p.WebsiteId == website.Id &&
                                         p.IsActive == true &&
                                         p.Delind == false);

                if (page == null)
                    return null;

                return new Models.PageModel
                {
                    Id = page.Id,
                    Title = page.Title,
                    Content = page.Content,
                    ViewName = page.ViewName,
                    WebsiteId = page.WebsiteId,
                    Slug = slug,
                    PageKey = page.PageKey,
                    IsActive = (bool)page.IsActive,
                    Delind = (bool)page.Delind,
                    FeatureImageUrl = page.FeatureImageUrl,
                    UpdatedOn = page.UpdatedOn,
                    AltImageText = page.AltImageText,
                    FAQs = _context.FaqMasters
        .AsNoTracking()
        .Where(x => x.ReferenceType == "Page"
                 && x.ReferenceId == page.Id
                 && x.IsActive == true
                 && x.Delind != true)
        .OrderBy(x => x.SortOrder)
        .Select(x => new FaqVM
        {
            FaqId = x.FaqId,
            Question = x.Question,
            Answer = x.Answer,
            SortOrder = x.SortOrder ?? 0
        })
        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Page Resolution Failed");
                return null;
            }
        }
        //public TravelWeb.Models.SeoEntities GetSeoData()
        //{
        //    return _seoData;
        //}
        public TravelWeb.Models.SeoEntities GetSeoData(int referenceId)
        {
            return _context.SeoEntities
                .AsNoTracking()
                .Where(x => x.ReferenceId == referenceId && x.Delind == false)
                .Select(x => new SeoEntities
                {
                    MetaTitle = x.MetaTitle,
                    MetaDescription = x.MetaDescription,
                    MetaKeywords = x.MetaKeywords,
                    CanonicalUrl = x.CanonicalUrl,
                    OgTitle = x.OgTitle,
                    OgType = x.OgType,
                    OgDescription = x.OgDescription,
                    OgImageUrl = x.OgImageUrl,
                    Robots = x.Robots,
                    SchemaJson = x.SchemaJson
                })
                .FirstOrDefault();
        }

        public PageSeoEditorVM GetPageSeoEditorData(string pageKey , int id)
        {
            var page = _context.Pages
                .Where(p => p.PageKey == pageKey && p.Id ==id && p.Delind == false)
                .Select(p => new PageModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    PageKey = p.PageKey,
                    WebsiteId = p.WebsiteId,
                    ViewName = p.ViewName
                })
                .FirstOrDefault();

            if (page == null)
                return null;

            var seo = GetSeo("Page", page.Id);
            var faqs = _context.FaqMasters
       .Where(x => x.ReferenceType == "Page"
                && x.ReferenceId == page.Id
                && x.Delind != true)
       .OrderBy(x => x.SortOrder)
       .Select(x => new FaqVM
       {
           FaqId = x.FaqId,
           Question = x.Question,
           Answer = x.Answer,
           SortOrder = x.SortOrder ?? 0
       })
       .ToList();
            return new PageSeoEditorVM
            {
                Page = page,
                Seo = seo ?? new SeoEntities(),
                FAQs = faqs
            };
        }

        //public void SavePageSeoEditorData(PageSeoEditorVM model)
        //{
        //    var page = _context.Pages.FirstOrDefault(p => p.Id == model.Page.Id);

        //    if (page == null)
        //        throw new Exception("Page not found");


        //    page.Title = model.Page.Title;
        //    page.Content = model.Page.Content;
        //    page.UpdatedOn = DateTime.Now;


        //    var seo = _context.SeoEntities
        //        .FirstOrDefault(s => s.PageType == "Page"
        //                          && s.ReferenceId == page.Id
        //                          && s.Delind == false);

        //    if (seo == null)
        //    {
        //        seo = new TravelWeb.Datamodel.SeoEntity
        //        {
        //            PageType = "Page",
        //            ReferenceId = page.Id,
        //            CreatedOn = DateTime.Now
        //        };

        //        _context.SeoEntities.Add(seo);
        //    }

        //    seo.Slug = model.Seo.Slug;
        //    seo.MetaTitle = model.Seo.MetaTitle;
        //    seo.MetaDescription = model.Seo.MetaDescription;
        //    seo.MetaKeywords = model.Seo.MetaKeywords;
        //    seo.CanonicalUrl = model.Seo.CanonicalUrl;
        //    seo.Robots = model.Seo.Robots;
        //    seo.UpdatedOn = DateTime.Now;

        //    _context.SaveChanges();
        //}

        public void SavePageSeoEditorData(PageSeoEditorVM model)
        { var Json  = JsonConvert.SerializeObject(model);
            try
            {
                TravelWeb.Datamodel.Page page;

                // =============================
                // CREATE MODE
                // =============================
                if (model.Page.Id == 0)
                {
                    page = new TravelWeb.Datamodel.Page
                    {
                        Title = model.Page.Title,
                        Content = model.Page.Content,
                        PageKey = model.Page.Title,
                        WebsiteId = model.Page.WebsiteId,
                        AltImageText = model.Page.AltImageText,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        FeatureImageUrl = model.Page.FeatureImageUrl,
                        IsActive = true
                    };

                    _context.Pages.Add(page);
                    _context.SaveChanges(); // Important: ID generate hone ke liye
                }
                else
                {
                    // =============================
                    // UPDATE MODE
                    // =============================
                    page = _context.Pages.FirstOrDefault(p => p.Id == model.Page.Id);

                    if (page == null)
                        throw new Exception("Page not found");

                    page.Title = model.Page.Title;
                    page.Content = model.Page.Content;
                    page.AltImageText = model.Page.AltImageText;

                    page.UpdatedOn = DateTime.Now;
                    page.FeatureImageUrl = model.Page.FeatureImageUrl;
                    _context.SaveChanges();
                }

                // =============================
                // SEO SECTION (Common for both)
                // =============================
                var seo = _context.SeoEntities
                    .FirstOrDefault(s => s.PageType == "Page"
                                      && s.ReferenceId == page.Id
                                      && s.Delind == false);

                if (seo == null)
                {
                    seo = new TravelWeb.Datamodel.SeoEntity
                    {
                        PageType = "Page",
                        ReferenceId = page.Id,
                        CreatedOn = DateTime.Now,
                        MetaDescription = model.Seo.MetaDescription,
                        MetaKeywords = model.Seo.MetaKeywords,
                        MetaTitle = model.Seo.MetaTitle,
                        WebsiteId = model.Page.WebsiteId,
                        Slug = model.Seo.Slug.Trim().ToLower().Replace(" ", "-"),
                    };

                    _context.SeoEntities.Add(seo);
                }

                seo.Slug = model.Seo.Slug.Trim().ToLower().Replace(" ", "-");
                seo.MetaTitle = model.Seo.MetaTitle;
                seo.MetaDescription = model.Seo.MetaDescription;
                seo.MetaKeywords = model.Seo.MetaKeywords;
                seo.CanonicalUrl = model.Seo.CanonicalUrl;
                seo.Robots = model.Seo.Robots;
                seo.UpdatedOn = DateTime.Now;

                model.FAQs ??= new List<FaqVM>();

                var existingFaqs = _context.FaqMasters
                    .Where(x => x.ReferenceType == "Page"
                             && x.ReferenceId == page.Id
                             && x.Delind != true)
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
                            ReferenceType = "Page",
                            ReferenceId = page.Id,
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
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

    }
}

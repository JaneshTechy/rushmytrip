using TravelWeb.Datamodel;
using TravelWeb.Models;

namespace TravelWeb.Interface
{
    public interface ISeoService
    {
        SeoEntities GetSeo(string pageType, int? referenceId = null,string domain = null);
        SeoEntities GetSeoBySlug(string Slug);
        PageModel GetPageBySlug(string domain, string slug);
        TravelWeb.Models.SeoEntities GetSeoData(int Id);
        PageSeoEditorVM GetPageSeoEditorData(string pageKey , int id);

        void SavePageSeoEditorData(PageSeoEditorVM model);
    }
}

using EPiServer.Core;
using EpiLastic.Wrappers;
using EpiLastic.Models;
using EPiServer.Web;
using System.Linq;
using EPiServer.Security;

namespace EpiLastic.Indexing.Services
{
    public interface IPageHelper
    {
        bool PageShouldBeIndexed(PageData page);

        bool PageShouldBeDeleted(PageData page);
    }

    public class PageHelper : IPageHelper
    {
        private readonly IDateTimeWrapper _dateTime;
        private readonly SiteDefinition _siteDefinition;

        public PageHelper(IDateTimeWrapper dateTime, SiteDefinition siteDefinition)
        {
            _dateTime = dateTime;
            _siteDefinition = siteDefinition;
        }

        public bool PageShouldBeDeleted(PageData page)
        {
            if (page.ParentLink.ID == _siteDefinition.WasteBasket.ID)
                return true;
            if (page is ISearchablePage && ((ISearchablePage)page).ExcludeFromSearch)
                return true;
            return false;
        }

        public bool PageShouldBeIndexed(PageData page)
        {
            if (!(page is ISearchablePage))
                return false;
            if (((ISearchablePage)page).ExcludeFromSearch)
                return false;
            if (!IsPublished(page))
                return false;
            if (!HasEveryoneReadAccess(page))
                return false;

            return true;
        }

        private bool HasEveryoneReadAccess(PageData page)
        {
            var result = page.ACL.Entries.Where(x => x.Name == "Everyone" && x.Access == AccessLevel.Read);
            if (result != null && result.Count() > 0)
                return true;

            return false;
        }

        private bool IsPublished(PageData page)
        {
            if (page.PendingPublish)
            {
                return false;
            }
            if (page.Status != VersionStatus.Published)
            {
                return false;
            }
            if (page.StartPublish > _dateTime.Now)
            {
                return false;
            }
            if (page.StopPublish < _dateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}

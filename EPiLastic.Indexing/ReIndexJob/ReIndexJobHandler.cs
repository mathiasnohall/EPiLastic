using EPiServer;
using EPiServer.DataAbstraction;
using EpiLastic.Helpers;
using EpiLastic.Wrappers;
using EPiServer.Web;
using EPiServer.Core;
using EpiLastic.Models;
using System.Linq;
using EpiLastic.Indexing.Services;

namespace EpiLastic.Indexing.ReIndexJob
{
    public interface IReIndexJobHandler
    {
        int ReIndex();
    }

    public class ReIndexJobHandler : IReIndexJobHandler
    {
        private readonly IDateTimeWrapper _dateTime;
        private readonly IContentLoader _contentLoader;
        private readonly ILanguageBranchRepository _languageBranches;
        private readonly IIndexClient _indexClient;
        private readonly IPageHelper _pageHelper;
        private readonly IIndexingHandler _indexHandler;
        private readonly SiteDefinition _siteDefinition;    

        public ReIndexJobHandler(
            IDateTimeWrapper dateTime,
            IContentLoader contentLoader,
            ILanguageBranchRepository languageBranchRepository,
            IIndexClient indexClient,
            IPageHelper pageHelper,
            IIndexingHandler indexingHandler,
            SiteDefinition siteDefinition
            )
        {
            _dateTime = dateTime;
            _contentLoader = contentLoader;
            _languageBranches = languageBranchRepository;
            _indexClient = indexClient;
            _pageHelper = pageHelper;
            _indexHandler = indexingHandler;
            _siteDefinition = siteDefinition;
        }

        public int ReIndex()
        {
            int indexedPages = 0;
            var result = _indexClient.Ping();
            if (result.OriginalException != null)
                throw result.OriginalException;

            var enabledLanguages = _languageBranches.ListEnabled();
            var timeStamp = _dateTime.Now;

            foreach(var enabledLangue in enabledLanguages)
            {
                var language = enabledLangue.Culture.TwoLetterISOLanguageName;
                var indexName = "epilastic_" + language +  "_" + timeStamp.ToString("yyyyMMdd\\_hhmmss");
                _indexClient.CreateIndex(indexName);

                var contentReferences = _contentLoader.GetDescendents(_siteDefinition.RootPage);
                var contents = _contentLoader.GetItems(contentReferences, enabledLangue.Culture);
                var pages = contents.Where(x => x is PageData).Cast<PageData>();

                foreach(var page in pages)
                {
                    if(_pageHelper.PageShouldBeIndexed(page))
                    {
                        var mappedPage = _indexHandler.IndexPage((ISearchablePage)page, language);
                        _indexClient.IndexDuringReindex(mappedPage, indexName);
                        indexedPages++;
                    }
                }

                _indexClient.SwapIndexes(indexName, IndexAlias.GetAlias(language));
            }

            return indexedPages;
        }
    }
}

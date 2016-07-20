using EPiServer;
using EPiServer.Core;
using EPiLastic.Models;
using System.Threading.Tasks;
using EPiLastic.Indexing.Services;

namespace EPiLastic.Indexing.EventHandling
{
    public interface IEPiServerEventHandler
    {
        Task MovedContent(ContentEventArgs e);

        Task PublishedContent(ContentEventArgs e);
    }

    public class EPiServerEventHandler : IEPiServerEventHandler
    {
        private readonly IIndexClient _indexClient;
        private readonly IPageHelper _pageHelper;
        private readonly IIndexingHandler _indexingHandler;

        public EPiServerEventHandler(IIndexClient indexClient, IPageHelper pageHelper, IIndexingHandler indexingHandler)
        {
            _indexClient = indexClient;
            _pageHelper = pageHelper;
            _indexingHandler = indexingHandler;
        }

        public async Task MovedContent(ContentEventArgs e)
        {
            if (e.Content is PageData)
            {
                var page = e.Content as PageData;
                foreach(var cultureInfo in page.ExistingLanguages)
                if (_pageHelper.PageShouldBeIndexed(page))
                {
                    var mappedPage = _indexingHandler.IndexPage((ISearchablePage)page, cultureInfo.TwoLetterISOLanguageName);
                    await _indexClient.IndexAsyncUsingAlias(mappedPage, cultureInfo.TwoLetterISOLanguageName);
                }
                else if (_pageHelper.PageShouldBeDeleted(page))
                {
                    await _indexClient.DeleteAsync(e.Content.ContentGuid, cultureInfo.TwoLetterISOLanguageName);
                }
            }
        }

        public async Task PublishedContent(ContentEventArgs e)
        {
            string language = (e.Content as ILocale).Language.TwoLetterISOLanguageName;

            if (_pageHelper.PageShouldBeIndexed(e.Content as PageData))
            {
                var mappedPage = _indexingHandler.IndexPage((ISearchablePage)e.Content, language);
                await _indexClient.IndexAsyncUsingAlias(mappedPage, language);
            }
            else if (_pageHelper.PageShouldBeDeleted(e.Content as PageData))
            {
                await _indexClient.DeleteAsync(e.Content.ContentGuid, language);
            }
            else if (e.Content is ISearchableBlock)
            {
                var mappedPages = _indexingHandler.IndexBlock(e.Content, language);
                foreach (var mappedPage in mappedPages)
                {
                    await _indexClient.IndexAsyncUsingAlias(mappedPage, language);
                }
            }
        }
    }
}

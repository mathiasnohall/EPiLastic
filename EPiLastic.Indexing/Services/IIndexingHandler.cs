using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EpiLastic.Models;
using System.Collections.Generic;
using System.Linq;

namespace EpiLastic.Indexing.Services
{
    public interface IIndexingHandler
    {
        Page IndexPage(ISearchablePage page, string language);

        IEnumerable<Page> IndexBlock(IContent block, string language);
    }

    public class IndexingHandler : IIndexingHandler
    {
        private readonly IContentLoader _contentLoader;
        private readonly IContentSoftLinkRepository _contentSoftLinkRepository;
        private readonly IPageHelper _pageHelper;

        public IndexingHandler(IContentLoader contentLoader, IContentSoftLinkRepository contentSoftLinkRepository, IPageHelper pageHelper)
        {
            _contentLoader = contentLoader;
            _contentSoftLinkRepository = contentSoftLinkRepository;
            _pageHelper = pageHelper;
        }

        public Page IndexPage(ISearchablePage page, string language)
        {
            var mappedPage = page.Map();
            mappedPage.Blocks = new List<Block>();

            var linksReferencedByPage = _contentSoftLinkRepository.Load(((IContent)page).ContentLink, false); // "Travel downwards"
            var washedReferencedByLinks = FixEPiServerBugs(linksReferencedByPage, false);

            foreach (var link in washedReferencedByLinks)
            {
                var block = _contentLoader.Get<IContent>(link.ReferencedContentLink, LanguageSelector.Fallback(language, false));
                if (block is ISearchableBlock)
                {
                    mappedPage.Blocks.Add((block as ISearchableBlock).Map());
                }

                if (block is ISearchableBlockContainer)
                {
                    mappedPage.Blocks.AddRange(IndexNestledBlocks(block, language));
                }
            }

            return mappedPage;
        }

        // Handle block publish event
        // Indexes all the pages that have the current block instance attached/referenced.
        public IEnumerable<Page> IndexBlock(IContent block, string language)
        {
            var searchablePages = new List<Page>();

            var ownerLinks = _contentSoftLinkRepository.Load(block.ContentLink, true); // true = "Travel upwards"
            var washedOwnerLinks = FixEPiServerBugs(ownerLinks, true);

            foreach (var link in washedOwnerLinks)
            {
                var content = _contentLoader.Get<IContent>(link.OwnerContentLink, LanguageSelector.Fallback(language, true));

                if (_pageHelper.PageShouldBeIndexed(content as PageData))
                {
                    searchablePages.Add(IndexPage(content as ISearchablePage, language));
                }

                if (content is ISearchableBlockContainer)
                {
                    searchablePages.AddRange(IndexBlock(content, language));
                }
            }

            return searchablePages;
        }

        private List<Block> IndexNestledBlocks(IContent block, string language)
        {
            var blocks = new List<Block>();

            var referencedByLinks = _contentSoftLinkRepository.Load(block.ContentLink, false); // false = "Travel downwards"
            var washedReferencedByLinks = FixEPiServerBugs(referencedByLinks, false);

            foreach (var link in washedReferencedByLinks)
            {
                var content = _contentLoader.Get<IContent>(link.ReferencedContentLink, LanguageSelector.Fallback(language, false));

                if (content is ISearchableBlock)
                {
                    blocks.Add((content as ISearchableBlock).Map());
                }

                if (content is ISearchableBlockContainer)
                {
                    blocks.AddRange(IndexNestledBlocks(content, language));
                }
            }

            return blocks;
        }

        private List<SoftLink> FixEPiServerBugs(IList<SoftLink> softLinks, bool reversed)
        {
            var washedLinks = new List<SoftLink>();
            foreach (var softlink in softLinks)
            {
                if (reversed)
                {
                    // To fix episerver broken link bug.
                    if (softlink.OwnerContentLink.ID == 0)
                        continue;
                    // To remove dublicate links from contentsoftlinkrepo
                    if (washedLinks.Where(x => x.OwnerContentLink == softlink.OwnerContentLink).Count() > 0)
                        continue;
                }
                else
                {
                    if (softlink.ReferencedContentLink.ID == 0)
                        continue;
                    if (washedLinks.Where(x => x.ReferencedContentLink == softlink.ReferencedContentLink).Count() > 0)
                        continue;
                }

                washedLinks.Add(softlink);
            }

            return washedLinks;
        }
    }
}

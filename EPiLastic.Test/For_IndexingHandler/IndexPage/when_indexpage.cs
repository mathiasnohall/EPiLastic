using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using FakeItEasy;
using NUnit.Framework;
using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using System.Collections.Generic;
using EPiLastic.Indexing.Services;

namespace EpiLastic.Test.For_IndexingHandler.IndexPage
{
    [TestFixture]
    public class when_indexpage
    {
        private PageData _parentPage;
        private IContent _childBlock;
        private IContent _blockContainer;
        private IContent _grandchildBlock;

        private IIndexingHandler _indexingHandler;
        private IPageHelper _pageHelper;
        private IContentLoader _contentLoader;
        private IContentSoftLinkRepository _contentSoftLinkRepo;
        private IObjectMapper _objectMapper;

        public when_indexpage()
        {
            _contentLoader = A.Fake<IContentLoader>();
            _contentSoftLinkRepo = A.Fake<IContentSoftLinkRepository>();
            _pageHelper = A.Fake<IPageHelper>();

            _objectMapper = A.Fake<IObjectMapper>();

            _parentPage = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            _parentPage.ContentLink = new ContentReference(1);
            A.CallTo(() => _pageHelper.PageShouldBeIndexed(A<PageData>.Ignored)).Returns(true);
            A.CallTo(() =>  _objectMapper.Map(((ISearchablePage)_parentPage))).Returns(A.Fake<Page>());

            _childBlock = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlock)));
            _childBlock.ContentLink = new ContentReference(2);
            A.CallTo(() =>  _objectMapper.Map(((ISearchableBlock)_childBlock))).Returns(A.Fake<Block>());
            A.CallTo(() => _contentLoader.Get<IContent>(_childBlock.ContentLink, A<LoaderOptions>.Ignored)).Returns(_childBlock);

            _blockContainer = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlockContainer)));
            _blockContainer.ContentLink = new ContentReference(3);
            A.CallTo(() => _contentLoader.Get<IContent>(_blockContainer.ContentLink, A<LoaderOptions>.Ignored)).Returns(_blockContainer);

            _grandchildBlock = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlock)));
            _grandchildBlock.ContentLink = new ContentReference(4);
            A.CallTo(() => _objectMapper.Map(((ISearchableBlock)_grandchildBlock))).Returns(A.Fake<Block>());
            A.CallTo(() => _contentLoader.Get<IContent>(_grandchildBlock.ContentLink, A<LoaderOptions>.Ignored)).Returns(_grandchildBlock);



            #region Set up contentsoftlinkrepository 
            var _childblock_being_referenced_by_parentpage = A.Fake<SoftLink>();
            var _blockContainer_being_referenced_by_parentpage = A.Fake<SoftLink>();
            var _grandchildblock_being_referenced_by_blockcontainer = A.Fake<SoftLink>();

            _childblock_being_referenced_by_parentpage.LinkMapper = A.Fake<PermanentLinkMapper>();
            _blockContainer_being_referenced_by_parentpage.LinkMapper = A.Fake<PermanentLinkMapper>();
            _grandchildblock_being_referenced_by_blockcontainer.LinkMapper = A.Fake<PermanentLinkMapper>();
            #endregion


            // Look under the parentPage
            _childblock_being_referenced_by_parentpage.ReferencedContentLink = _childBlock.ContentLink;
            _blockContainer_being_referenced_by_parentpage.ReferencedContentLink = _blockContainer.ContentLink;
            A.CallTo(() => _contentSoftLinkRepo.Load(_parentPage.ContentLink, false))
                .Returns(new List<SoftLink>
                    {
                        _childblock_being_referenced_by_parentpage,
                        _blockContainer_being_referenced_by_parentpage
                    });

            // Look under the blockcontainer
            _grandchildblock_being_referenced_by_blockcontainer.ReferencedContentLink = _grandchildBlock.ContentLink;
            A.CallTo(() => _contentSoftLinkRepo.Load((_blockContainer).ContentLink, false))
                .Returns(new List<SoftLink>
                    {
                        _grandchildblock_being_referenced_by_blockcontainer,
                    });

            _indexingHandler = new IndexingHandler(_contentLoader, _contentSoftLinkRepo, _pageHelper, _objectMapper);
        }

        [Test]
        public void IndexingHandler_when_indexpage_it_should_return_a_mappedPage_with_2_blocks()
        {
            var result = _indexingHandler.IndexPage((ISearchablePage)_parentPage, "sv");
            Assert.AreEqual(2, result.Blocks.Count);
        }
    }
}

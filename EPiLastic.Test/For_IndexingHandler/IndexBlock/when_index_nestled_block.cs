using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using FakeItEasy;
using NUnit.Framework;
using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using System.Collections.Generic;
using System.Linq;


namespace EpiLastic.Test.For_IndexingHandler.IndexBlock
{
    [TestFixture]
    public class when_index_nest_block
    {
        private PageData _parentPage;
        private IContent _childBlock;
        private IContent _blockContainer;
        private IContent _grandchildBlock;

        private IIndexingHandler _indexingHandler;
        private IPageHelper _pageHelper;
        private IContentLoader _contentLoader;
        private IContentSoftLinkRepository _contentSoftLinkRepo;

        public when_index_nest_block()
        {
            _contentLoader = A.Fake<IContentLoader>();
            _contentSoftLinkRepo = A.Fake<IContentSoftLinkRepository>();
            _pageHelper = A.Fake<IPageHelper>();

            _parentPage = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            _parentPage.ContentLink = new ContentReference(1);
            A.CallTo(() => _pageHelper.PageShouldBeIndexed(_parentPage)).Returns(true);
            A.CallTo(() => ((ISearchablePage)_parentPage).Map()).Returns(A.Fake<Page>());
            A.CallTo(() => _contentLoader.Get<IContent>(_parentPage.ContentLink, A<LoaderOptions>.Ignored)).Returns(_parentPage);

            _childBlock = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlock)));
            _childBlock.ContentLink = new ContentReference(2);
            A.CallTo(() => ((ISearchableBlock)_childBlock).Map()).Returns(A.Fake<Block>());
            A.CallTo(() => _contentLoader.Get<IContent>(_childBlock.ContentLink, A<LoaderOptions>.Ignored)).Returns(_childBlock);

            _blockContainer = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlockContainer)));
            _blockContainer.ContentLink = new ContentReference(3);
            A.CallTo(() => _contentLoader.Get<IContent>(_blockContainer.ContentLink, A<LoaderOptions>.Ignored)).Returns(_blockContainer);

            _grandchildBlock = A.Fake<IContent>(x => x.Implements(typeof(ISearchableBlock)));
            _grandchildBlock.ContentLink = new ContentReference(4);
            A.CallTo(() => ((ISearchableBlock)_grandchildBlock).Map()).Returns(A.Fake<Block>());
            A.CallTo(() => _contentLoader.Get<IContent>(_grandchildBlock.ContentLink, A<LoaderOptions>.Ignored)).Returns(_grandchildBlock);



            #region Set up contentsoftlinkrepository 
            var _childblock_being_referenced_by_parentpage = A.Fake<SoftLink>();
            var _blockContainer_being_referenced_by_parentpage = A.Fake<SoftLink>();
            var _grandchildblock_being_referenced_by_blockcontainer = A.Fake<SoftLink>();
            var _grandchildBlock_being_owned_by_containerblock = A.Fake<SoftLink>();
            var blockContainer_being_owned_by_parentPage = A.Fake<SoftLink>();

            _childblock_being_referenced_by_parentpage.LinkMapper = A.Fake<PermanentLinkMapper>();
            _blockContainer_being_referenced_by_parentpage.LinkMapper = A.Fake<PermanentLinkMapper>();
            _grandchildblock_being_referenced_by_blockcontainer.LinkMapper = A.Fake<PermanentLinkMapper>();
            _grandchildBlock_being_owned_by_containerblock.LinkMapper = A.Fake<PermanentLinkMapper>();
            blockContainer_being_owned_by_parentPage.LinkMapper = A.Fake<PermanentLinkMapper>();
            #endregion


            // Look over the blockcontainer on the parentpage
            blockContainer_being_owned_by_parentPage.OwnerContentLink = _parentPage.ContentLink;
            A.CallTo(() => _contentSoftLinkRepo.Load(_blockContainer.ContentLink, true))
                .Returns(new List<SoftLink>
                    {
                        blockContainer_being_owned_by_parentPage
                    });

            // Look over the nestblock in the blockcontainer
            _grandchildBlock_being_owned_by_containerblock.OwnerContentLink = _blockContainer.ContentLink;
            A.CallTo(() => _contentSoftLinkRepo.Load(_grandchildBlock.ContentLink, true))
                .Returns(new List<SoftLink>
                    {
                        _grandchildBlock_being_owned_by_containerblock
                    });

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

            _indexingHandler = new IndexingHandler(_contentLoader, _contentSoftLinkRepo, _pageHelper);
        }

        [Test]
        public void IndexingHandler_when_index_bock_it_should_return_only_one_mapped_page()
        {
            var result = _indexingHandler.IndexBlock(_grandchildBlock, "sv");
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void IndexingHandler_when_index_block_it_should_return_a_mappedPage_with_2_blocks()
        {
            var result = _indexingHandler.IndexBlock(_grandchildBlock, "sv");
            Assert.AreEqual(2, result.First().Blocks.Count);
        }
    }
}

using NUnit.Framework;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiLastic.Wrappers;
using FakeItEasy;
using EPiLastic.Models;
using System;
using EPiServer.Core;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Web;
using EPiLastic.Indexing;
using EPiLastic.Indexing.ReIndexJob;
using EPiLastic.Indexing.Services;

namespace EPiLastic.Test.For_ReIndexJobHandler
{
    [TestFixture]
    public class when_reindex
    {
        private IReIndexJobHandler _reIndexJobHandler;

        private IDateTimeWrapper _dateTime;
        private IContentLoader _contentLoader;
        private ILanguageBranchRepository _languageBranches;
        private IIndexClient _indexClient;
        private IPageHelper _pageHelper;
        private IIndexingHandler _indexHandler;
        private SiteDefinition _siteDefinition;

        private int _count = 0;

        public when_reindex()
        {
            _dateTime = A.Fake<IDateTimeWrapper>();
            A.CallTo(() => _dateTime.Now).Returns(new DateTime(2016, 2, 22, 12, 12, 12));

            var swedishPages = new List<IContent>();
            for (int i = 1; i <= 50; i++)
            {
                swedishPages.Add(A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage))));
            }

            var englishPages = new List<IContent>();
            for (int i = 1; i <= 49; i++)
            {
                englishPages.Add(A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage))));
            }

            _languageBranches = A.Fake<ILanguageBranchRepository>();
            var languageBranches = new List<LanguageBranch>();
            languageBranches.Add(new LanguageBranch(new CultureInfo("sv")));
            languageBranches.Add(new LanguageBranch(new CultureInfo("en")));
            A.CallTo(() => _languageBranches.ListEnabled()).Returns(languageBranches);

            _contentLoader = A.Fake<IContentLoader>();
            A.CallTo(() => _contentLoader.GetDescendents(A<ContentReference>.Ignored)).Returns(A.CollectionOfFake<ContentReference>(60));
            A.CallTo(() => _contentLoader.GetItems(A<IEnumerable<ContentReference>>.Ignored, new CultureInfo("sv"))).Returns(swedishPages);
            A.CallTo(() => _contentLoader.GetItems(A<IEnumerable<ContentReference>>.Ignored, new CultureInfo("en"))).Returns(englishPages);

            _indexClient = A.Fake<IIndexClient>();
            var pingResult = A.Fake<Nest.IPingResponse>();
            A.CallTo(() => pingResult.OriginalException).Returns(null);
            A.CallTo(() => _indexClient.Ping()).Returns(pingResult);
            _indexHandler = A.Fake<IIndexingHandler>();
            _pageHelper = A.Fake<IPageHelper>();
            A.CallTo(() => _pageHelper.PageShouldBeIndexed(A<PageData>.Ignored)).Returns(true);

            _siteDefinition = A.Fake<SiteDefinition>();

            _reIndexJobHandler = new ReIndexJobHandler(_dateTime, _contentLoader, _languageBranches, _indexClient, _pageHelper, _indexHandler, _siteDefinition);
            
            _count = _reIndexJobHandler.ReIndex();
        }

        [Test]
        public void It_should_create_an_index_for_each_language_based_on_date()
        {
            A.CallTo(() => _indexClient.CreateIndex("epilastic_sv_20160222_121212")).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _indexClient.CreateIndex("epilastic_en_20160222_121212")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void It_should_index_swedish_pages_to_the_swedish_index()
        {
            A.CallTo(() => _indexClient.IndexDuringReindex(A<Page>.Ignored, "epilastic_sv_20160222_121212")).MustHaveHappened(Repeated.Exactly.Times(50));
        }

        [Test]
        public void It_should_index_english_pages_to_the_english_index()
        {
            A.CallTo(() => _indexClient.IndexDuringReindex(A<Page>.Ignored, "epilastic_en_20160222_121212")).MustHaveHappened(Repeated.Exactly.Times(49));
        }

        [Test]
        public void It_should_swap_the_old_and_the_new_index()
        {
            A.CallTo(() => _indexClient.SwapIndexes("epilastic_sv_20160222_121212", "test_sv")).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _indexClient.SwapIndexes("epilastic_en_20160222_121212", "test_en")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void It_should_count_indexed_pages()
        {
            Assert.AreEqual(99, _count);
        }
    }
}

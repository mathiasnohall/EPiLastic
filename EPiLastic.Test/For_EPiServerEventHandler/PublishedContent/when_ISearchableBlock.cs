using EPiServer;
using EPiServer.Core;
using FakeItEasy;
using NUnit.Framework;
using EPiLastic.Indexing;
using EPiLastic.Indexing.EventHandling;
using EPiLastic.Indexing.Services;
using EPiLastic.Models;
using System;
using System.Globalization;

namespace EPiLastic.Test.For_EPiServerEventHandler.PublishedContent
{
    [TestFixture]
    public class when_ISearchableBlock
    {
        private ISearchableBlock _block;
        private IEPiServerEventHandler _eventHandler;
        private IIndexClient _indexClient;
        private IPageHelper _pageHelper;
        private IIndexingHandler _indexingHandler;
        private ContentEventArgs _event;

        public when_ISearchableBlock()
        {
            _block = A.Fake<ISearchableBlock>(x => x.Implements(typeof(IContent)).Implements(typeof(ILocale)));
            ((ILocale)_block).Language = new CultureInfo("sv");
            _indexClient = A.Fake<IIndexClient>();
            _indexingHandler = A.Fake<IIndexingHandler>();
            _pageHelper = A.Fake<IPageHelper>();

            A.CallTo(() => _pageHelper.PageShouldBeIndexed(A<PageData>.Ignored)).Returns(false);

            _event = A.Fake<ContentEventArgs>();
            _event.Content = (IContent)_block;

            A.CallTo(() => _indexingHandler.IndexBlock(A<IContent>.Ignored, A<string>.Ignored)).Returns(A.CollectionOfFake<Page>(1));

            _eventHandler = new EPiServerEventHandler(_indexClient, _pageHelper, _indexingHandler);
        }

        [Test]
        public void when_publish_ISearchableBlock_it_should_index()
        {
            _eventHandler.PublishedContent(_event);

            A.CallTo(() => _indexClient.IndexAsyncUsingAlias(A<Page>.Ignored, A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _indexClient.DeleteAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();            
            A.CallTo(() => _indexClient.IndexDuringReindex(A<Page>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}

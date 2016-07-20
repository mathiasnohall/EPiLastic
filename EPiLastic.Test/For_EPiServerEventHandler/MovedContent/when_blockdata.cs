using EPiServer;
using EPiServer.Core;
using FakeItEasy;
using NUnit.Framework;
using EPiLastic.Indexing;
using EPiLastic.Indexing.EventHandling;
using EPiLastic.Indexing.Services;
using EPiLastic.Models;
using System;

namespace EPiLastic.Test.For_EPiServerEventHandler.MovedContent
{
    [TestFixture]
    public class when_blockdata
    {
        private BlockData _block;
        private IEPiServerEventHandler _eventHandler;
        private IIndexClient _indexClient;
        private IPageHelper _pageHelper;
        private IIndexingHandler _indexingHandler;
        private ContentEventArgs _event;

        public when_blockdata()
        {
            _block = A.Fake<BlockData>(x => x.Implements(typeof(IContent)));
            _indexClient = A.Fake<IIndexClient>();
            _indexingHandler = A.Fake<IIndexingHandler>();
            _pageHelper = A.Fake<IPageHelper>();

            _event = A.Fake<ContentEventArgs>();
            _event.Content = (IContent)_block;

            _eventHandler = new EPiServerEventHandler(_indexClient, _pageHelper, _indexingHandler);
        }

        [Test]
        public void when_move_blockdata_it_should_do_nothing()
        {
            _eventHandler.MovedContent(_event);

            A.CallTo(() => _indexClient.DeleteAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _indexClient.IndexAsyncUsingAlias(A<Page>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _indexClient.IndexDuringReindex(A<Page>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}

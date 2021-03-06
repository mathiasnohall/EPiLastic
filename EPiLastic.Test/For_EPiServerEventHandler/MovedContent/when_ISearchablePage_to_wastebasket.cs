﻿using EPiServer;
using EPiServer.Core;
using FakeItEasy;
using NUnit.Framework;
using EPiLastic.Indexing;
using EPiLastic.Indexing.EventHandling;
using EPiLastic.Indexing.Services;
using EPiLastic.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EPiLastic.Test.For_EPiServerEventHandler.MovedContent
{
    [TestFixture]
    public class when_ISearchablePage_to_wastebasket
    {
        private PageData _page;
        private IEPiServerEventHandler _eventHandler;
        private IIndexClient _indexClient;
        private IPageHelper _pageHelper;
        private IIndexingHandler _indexingHandler;
        private ContentEventArgs _event;

        public when_ISearchablePage_to_wastebasket()
        {
            _page = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            var languages = new List<CultureInfo>();
            languages.Add(new CultureInfo("sv"));
            languages.Add(new CultureInfo("en"));
            _page.ExistingLanguages = languages;
            _indexClient = A.Fake<IIndexClient>();
            _indexingHandler = A.Fake<IIndexingHandler>();
            _pageHelper = A.Fake<IPageHelper>();

            A.CallTo(() => _pageHelper.PageShouldBeIndexed(A<PageData>.Ignored)).Returns(false);
            A.CallTo(() => _pageHelper.PageShouldBeDeleted(A<PageData>.Ignored)).Returns(true);

            _event = A.Fake<ContentEventArgs>();
            _event.Content = _page;

            _eventHandler = new EPiServerEventHandler(_indexClient, _pageHelper, _indexingHandler);
        }

        [Test]
        public void when_move_ISearchablePag_to_wastebasket_it_should_delete()
        {
            _eventHandler.MovedContent(_event);

            A.CallTo(() => _indexClient.DeleteAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);

            A.CallTo(() => _indexClient.IndexAsyncUsingAlias(A<Page>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _indexClient.IndexDuringReindex(A<Page>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}

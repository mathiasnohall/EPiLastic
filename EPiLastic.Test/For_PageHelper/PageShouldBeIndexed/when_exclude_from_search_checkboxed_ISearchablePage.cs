using EPiServer.Core;
using EPiServer.Web;
using FakeItEasy;
using NUnit.Framework;
using EPiLastic.Indexing.Services;
using EPiLastic.Models;
using EPiLastic.Wrappers;
using System;

namespace EPiLastic.Test.For_PageHelper.PageShouldBeIndexed
{
    [TestFixture]
    public class when_exclude_from_search_checkboxed_ISearchablePage
    {
        private PageData _page;
        private IPageHelper _pageHelper;
        private IDateTimeWrapper _dateTime;
        private SiteDefinition _siteDefinition;

        public when_exclude_from_search_checkboxed_ISearchablePage()
        {
            _page = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            ((ISearchablePage)_page).ExcludeFromSearch = true;
            _dateTime = A.Fake<IDateTimeWrapper>();
            _siteDefinition = A.Fake<SiteDefinition>();

            A.CallTo(() => _dateTime.Now).Returns(new DateTime(2016, 2, 18));

            _pageHelper = new PageHelper(_dateTime, _siteDefinition);
        }

        [Test]
        public void it_should_return_false()
        {
            Assert.That(_pageHelper.PageShouldBeIndexed(_page) == false);
        }

    }
}

using EPiServer.Core;
using EPiServer.Web;
using FakeItEasy;
using NUnit.Framework;
using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EpiLastic.Wrappers;
using System;

namespace EpiLastic.Test.For_PageHelper.PageShouldBeIndexed
{
    [TestFixture]
    public class when_expired_ISearchablePage
    {
        private PageData _page;
        private IPageHelper _pageHelper;
        private IDateTimeWrapper _dateTime;
        private SiteDefinition _siteDefinition;

        public when_expired_ISearchablePage()
        {
            _page = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            _page.StopPublish = new DateTime(2015, 1, 1);
            
            _page.Status = VersionStatus.Published;

            _dateTime = A.Fake<IDateTimeWrapper>();
            A.CallTo(() => _dateTime.Now).Returns(new DateTime(2016, 2, 18));
            _siteDefinition = A.Fake<SiteDefinition>();

            _pageHelper = new PageHelper(_dateTime, _siteDefinition);
        }

        [Test]
        public void it_should_return_false()
        {
            Assert.That(_pageHelper.PageShouldBeIndexed(_page) == false);
        }
    }
}

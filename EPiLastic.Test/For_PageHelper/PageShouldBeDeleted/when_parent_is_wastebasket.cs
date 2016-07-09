using EPiServer.Core;
using EPiServer.Web;
using FakeItEasy;
using NUnit.Framework;
using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EpiLastic.Wrappers;
using System;

namespace EpiLastic.Test.For_PageHelper.PageShouldBeDeleted
{
    [TestFixture]
    public class when_parent_is_wastebasket
    {
        private PageData _page;
        private IPageHelper _pageHelper;
        private IDateTimeWrapper _dateTime;
        private SiteDefinition _siteDefinition;

        public when_parent_is_wastebasket()
        {
            _page = A.Fake<PageData>();
            _page.StartPublish = new DateTime(2015, 1, 1);
            _page.StopPublish = new DateTime(9999, 1, 1); // default epi value
            _page.Status = VersionStatus.Published;
            _dateTime = A.Fake<IDateTimeWrapper>();

            _siteDefinition = A.Fake<SiteDefinition>();
            A.CallTo(() => _siteDefinition.WasteBasket).Returns(new ContentReference(12345));
            _page.ParentLink = new PageReference(12345);

            A.CallTo(() => _dateTime.Now).Returns(new DateTime(2016, 2, 18));

            _pageHelper = new PageHelper(_dateTime, _siteDefinition);
        }

        [Test]
        public void when_parent_is_wastebasket_it_should_return_true()
        {
            Assert.That(_pageHelper.PageShouldBeDeleted(_page) == true);
        }
    }
}

using EPiServer.Core;
using EPiServer.Security;
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
    public class when_nonReadAccess_ISearchablePage
    {
        private PageData _page;
        private IPageHelper _pageHelper;
        private IDateTimeWrapper _dateTime;
        private SiteDefinition _siteDefinition;

        public when_nonReadAccess_ISearchablePage()
        {
            _page = A.Fake<PageData>(x => x.Implements(typeof(ISearchablePage)));
            _page.StartPublish = new DateTime(2015, 1, 1);
            _page.StopPublish = new DateTime(9999, 1, 1); // default epi value
            _page.Status = VersionStatus.Published;
            _dateTime = A.Fake<IDateTimeWrapper>();
            
            var acl = A.Fake<AccessControlList>();
            var entry = new AccessControlEntry("Everyone", AccessLevel.NoAccess, SecurityEntityType.Role);
            acl.Add(entry);

            _page.ACL = acl;

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

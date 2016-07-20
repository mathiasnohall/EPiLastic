using EPiLastic.Indexing.Services;
using EPiLastic.Test.FakeModels;
using EPiServer.Web.Routing;
using FakeItEasy;
using NUnit.Framework;

namespace EPiLastic.Test.For_ObjectMapper
{
    [TestFixture]
    public class when_mapping_SearchablePage_when_no_title_attribute
    {
        private IObjectMapper _objectMapper;
        private FakeSearchablePage _page;

        public when_mapping_SearchablePage_when_no_title_attribute()
        {
            var urlResolver = A.Fake<UrlResolver>();
            var suggestionHelper = A.Fake<ISuggestionHelper>();
            _objectMapper = new ObjectMapper(suggestionHelper, urlResolver);            

            _page = A.Fake<FakeSearchablePage>();
            _page.Language = new System.Globalization.CultureInfo("sv");
            
            _page.Name = "FakeSearchablePage";
        }        

        [Test]
        public void when_mapping_SearchablePage_when_no_title_attribute_it_should_fallback_to_page_name()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("FakeSearchablePage", mappedPage.Name);
        }
    }
}

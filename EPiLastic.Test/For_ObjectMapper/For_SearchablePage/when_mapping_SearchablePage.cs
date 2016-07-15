using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EPiLastic.Attributes;
using EPiLastic.Indexing.Services;
using EPiLatic.Test.For_ObjectMapper.FakeModels;
using EPiServer.Core;
using EPiServer.Web.Routing;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EPiLastic.Test.For_ObjectMapper
{
    [TestFixture]
    public class when_mapping_SearchablePage
    {
        private IObjectMapper _objectMapper;
        private FakeSearchablePage _page;

        public when_mapping_SearchablePage()
        {
            var suggestionHelper = A.Fake<ISuggestionHelper>();
            A.CallTo(() => suggestionHelper.GeneratePageSuggestions("FakeSearchablePage")).Returns(new[] { "FakeSearchablePage" });

            var urlResolver = A.Fake<UrlResolver>();
            
            _objectMapper = new ObjectMapper(suggestionHelper, urlResolver);

            var titleConstructor = typeof(TitleAttribute).GetConstructor(new Type[0]);
            var titleBuilder = new CustomAttributeBuilder(titleConstructor, new object[0]);

            var textConstructor = typeof(TextAttribute).GetConstructor(new Type[0]);
            var textBuilder = new CustomAttributeBuilder(textConstructor, new object[0]);

            var builders = new List<CustomAttributeBuilder>() { titleBuilder, textBuilder };

            _page = A.Fake<FakeSearchablePage>(x => x.WithAdditionalAttributes(builders));
            _page.Language = new System.Globalization.CultureInfo("sv");

            _page.HiddenKeyWord = "hidden keywords";
            _page.Title = "The Page Title";

            var mainbody = A.Fake<XhtmlString>();
            A.CallTo(() => mainbody.ToHtmlString()).Returns("<p>some text in MainBody</p>");
            _page.MainBody = mainbody;

            _page.Type = "MainType";
            _page.SubType = "SubType";

            _page.TeaserText = "Some text in teaser";
            _page.TeaserImage = new ContentReference(999);

            A.CallTo(() => urlResolver.GetUrl(_page.ContentLink, "sv")).Returns("/the-page-url/");
            A.CallTo(() => urlResolver.GetUrl(_page.TeaserImage)).Returns("/the-teaser-img-url/");

            _page.Name = "FakeSearchablePage";
            _page.ContentGuid = new System.Guid("5e49c665-f534-4288-8da7-945cd4b862a1");
            _page.Created = new System.DateTime(2016, 3, 2);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_to_searchable_page()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.IsNotNull(mappedPage);
            Assert.IsAssignableFrom(typeof(Page), mappedPage);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_the_title()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("The Page Title", mappedPage.Name);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_the_url()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("/the-page-url/", mappedPage.NavigateUrl);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_teaserText()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("Some text in teaser", mappedPage.TeaserText);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_teaserImgUrl()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("/the-teaser-img-url/", mappedPage.TeaserImageUrl);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_have_a_unique_id()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("5e49c665-f534-4288-8da7-945cd4b862a1", mappedPage.ContentGuid.ToString());
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_have_a_created_field()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual(_page.Created, mappedPage.Created);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_mainbody()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("some text in MainBody", mappedPage.MainBody);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_HiddenKeywords_for_internal_search()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.AreEqual("hidden keywords", mappedPage.HiddenKeywords);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_autocompletion()
        {
            var mappedPage = _objectMapper.Map(_page);

            Assert.IsNotEmpty(mappedPage.AutoComplete.Input);
            Assert.AreEqual("FakeSearchablePage", mappedPage.AutoComplete.Output);
        }

        [Test]
        public void when_mapping_SearchablePage_it_should_map_type_and_subtype()
        {
            var mappedPage = _objectMapper.Map(_page);
            
            Assert.AreEqual("MainType", mappedPage.Type);
            Assert.AreEqual("SubType", mappedPage.SubType);
        }

    }
}

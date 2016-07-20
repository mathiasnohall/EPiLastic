using EPiLastic.Indexing.Services;
using EPiLastic.Models;
using EPiLastic.Attributes;
using EPiServer.Core;
using EPiServer.Web.Routing;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using EPiLastic.Test.FakeModels;

namespace EPiLastic.Test.For_ObjectMapper
{
    [TestFixture]
    public class when_mapping_SearchableBlock
    {
        private IObjectMapper _objectMapper;
        private FakeSearchableBlock _block;

        public when_mapping_SearchableBlock()
        {
            var titleConstructor = typeof(TitleAttribute).GetConstructor(new Type[0]);
            var titleBuilder = new CustomAttributeBuilder(titleConstructor, new object[0]);

            var textConstructor = typeof(TextAttribute).GetConstructor(new Type[0]);
            var textBuilder = new CustomAttributeBuilder(textConstructor, new object[0]);

            var builders = new List<CustomAttributeBuilder>() { titleBuilder, textBuilder };

            _block = A.Fake<FakeSearchableBlock>(x => x.WithAdditionalAttributes(builders));

            _block.Title = "The Title";

            var xHtmlString = A.Fake<XhtmlString>();
            A.CallTo(() => xHtmlString.ToHtmlString()).Returns("<p>some text on Mainbody</p>");
            _block.MainBody = xHtmlString;

            _objectMapper = new ObjectMapper(A.Fake<ISuggestionHelper>(), A.Fake<UrlResolver>());
        }

        [Test]
        public void when_mapping_SearchableBlock_it_should_map_to_searchable_block()
        {
            var mappedBlock = _objectMapper.Map(_block);

            Assert.NotNull(mappedBlock);
            Assert.IsInstanceOf<Block>(mappedBlock);
        }        

        [Test]
        public void when_mapping_SearchableBlock_it_should_map_title_attribute()
        {
            var mappedBlock = _objectMapper.Map(_block);

            Assert.NotNull(mappedBlock.Title);
            Assert.AreEqual("The Title", mappedBlock.Title);
        }

        [Test]
        public void when_mapping_SearchableBlock_it_should_map_mainbody_attribute()
        {
            var mappedBlock = _objectMapper.Map(_block);

            Assert.NotNull(mappedBlock.MainBody);
            Assert.AreEqual("some text on Mainbody", mappedBlock.MainBody);
        }

    }
}

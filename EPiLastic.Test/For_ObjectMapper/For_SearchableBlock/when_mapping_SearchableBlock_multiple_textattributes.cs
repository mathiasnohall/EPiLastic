using EPiLastic.Indexing.Services;
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
    public class when_mapping_SearchableBlock_multiple_textattributes
    {
        private IObjectMapper _objectMapper;
        private FakeSearchableBlock _block;

        public when_mapping_SearchableBlock_multiple_textattributes()
        {
            var textConstructor = typeof(TextAttribute).GetConstructor(new Type[0]);
            var textBuilder = new CustomAttributeBuilder(textConstructor, new object[0]);

            var builders = new List<CustomAttributeBuilder>() { textBuilder };

            _block = A.Fake<FakeSearchableBlock>(x => x.WithAdditionalAttributes(builders));            

            var xHtmlString = A.Fake<XhtmlString>();
            A.CallTo(() => xHtmlString.ToHtmlString()).Returns("<p>some text on Mainbody</p>");
            _block.MainBody = xHtmlString;

            _block.SecondBody = "second body";

            var xHtmlString2 = A.Fake<XhtmlString>();
            A.CallTo(() => xHtmlString2.ToHtmlString()).Returns("<p>some text on third body</p>");
            _block.ThirdBody = xHtmlString2;

            _objectMapper = new ObjectMapper(A.Fake<ISuggestionHelper>(), A.Fake<UrlResolver>());
        }        

        [Test]
        public void when_mapping_SearchableBlock_multiple_textattributes_it_should_map_and_concatenate_text_bodies()
        {
            var mappedBlock = _objectMapper.Map(_block);

            Assert.NotNull(mappedBlock.MainBody);
            Assert.AreEqual("some text on Mainbody second body some text on third body", mappedBlock.MainBody);
        }

    }
}

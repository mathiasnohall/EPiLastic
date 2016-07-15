using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EPiLastic.Indexing.Services;
using EPiLatic.Test.For_ObjectMapper.FakeModels;
using EPiServer.Core;
using EPiServer.Web.Routing;
using FakeItEasy;
using NUnit.Framework;

namespace EPiLastic.Test.For_ObjectMapper
{
    [TestFixture]
    public class when_mapping_SearchableBlock
    {
        private IObjectMapper _objectMapper;
        private FakeSearchableBlock _block;

        public when_mapping_SearchableBlock()
        {
            _block = A.Fake<FakeSearchableBlock>();
            var xHtmlString = A.Fake<XhtmlString>();
            A.CallTo(() => xHtmlString.ToHtmlString()).Returns("<p>some text on Mainbody</p>");
            _block.MainBody = xHtmlString;

            _block.Title = "The Title";

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

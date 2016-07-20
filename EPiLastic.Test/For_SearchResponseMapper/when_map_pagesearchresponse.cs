using FakeItEasy;
using Nest;
using NUnit.Framework;
using EPiLastic.Models;
using EPiLastic.Models.Responses;
using EPiLastic.Services;
using System.Collections.Generic;
using System.Linq;

namespace EPiLastic.Test.For_SearchResponseMapper
{
    [TestFixture]
    public class when_map_pagesearchresponse
    {
        private ISearchResponseMapper _searchResponseMapper;
        private ISearchResponse<Page> _nestSearchResponse;

        public when_map_pagesearchresponse()
        {
            _nestSearchResponse = A.Fake<ISearchResponse<Page>>();
            A.CallTo(() => _nestSearchResponse.Total).Returns(12);
            A.CallTo(() => _nestSearchResponse.Documents).Returns(A.CollectionOfFake<Page>(12));

            var aggregationDictionary = new Dictionary<string, IAggregate>();

            var typeBucketAggregate = new BucketAggregate();
            typeBucketAggregate.Items = A.CollectionOfFake<KeyedBucket>(5); ;
            aggregationDictionary.Add("type", typeBucketAggregate);

            var subTypeBucketAggregate = new BucketAggregate();
            subTypeBucketAggregate.Items = A.CollectionOfFake<KeyedBucket>(5);
            aggregationDictionary.Add("subType", subTypeBucketAggregate);

            A.CallTo(() => _nestSearchResponse.Aggregations).Returns(aggregationDictionary);            

            _searchResponseMapper = new SearchResponseMapper();
        }

        [Test]
        public void when_map_nest_ISearchResponse_of_Page_it_should_map_to_pagesearchresponse()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PagesSearchResponse>(result);
        }

        [Test]
        public void when_map_nest_ISearchResponse_of_Page_it_should_have_a_list_of_pages()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.NotNull(result.Pages);
            Assert.That(result.Pages.Count == 12);
        }

        [Test]
        public void when_map_nest_ISearchResponse_of_Page_it_should_have_count_property()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.AreEqual(12, result.Count);
            Assert.That(result.Count == result.Pages.Count);
        }

        [Test]
        public void when_map_nest_ISearchResponse_of_page_it_should_have_type_aggregation()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.NotNull(result.Aggregations);
            Assert.That(result.Aggregations.Where(x => x.Key == "type").Count() == 1);

            var typeAggregations = result.Aggregations["type"];
            Assert.AreEqual(5, typeAggregations.Items.Count);

        }

        [Test]
        public void when_map_nest_ISearchResponse_of_page_it_should_have_subType_aggregation()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.NotNull(result.Aggregations);
            Assert.That(result.Aggregations.Where(x => x.Key == "subType").Count() == 1);

            var typeAggregations = result.Aggregations["subType"];
            Assert.AreEqual(5, typeAggregations.Items.Count);
        }

        [Test]
        public void when_map_nest_ISearchResponse_of_page_pages_should_have_ids()
        {
            var result = _searchResponseMapper.Map(_nestSearchResponse);

            Assert.That(result.Pages.Where(x => x.Id != null).Count() == 12);
        }
    }
}

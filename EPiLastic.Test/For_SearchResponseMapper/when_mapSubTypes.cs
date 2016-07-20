using FakeItEasy;
using Nest;
using NUnit.Framework;
using EPiLastic.Models;
using EPiLastic.Services;
using System.Collections.Generic;

namespace EPiLastic.Test.For_SearchResponseMapper
{
    [TestFixture]
    public class when_mapSubTypes
    {
        private ISearchResponseMapper _searchResponseMapper;
        private ISearchResponse<Page> _nestSearchResponse;

        public when_mapSubTypes()
        {
            _nestSearchResponse = A.Fake<ISearchResponse<Page>>();

            var aggregationDictionary = new Dictionary<string, IAggregate>();

            var subTypeBucketAggregate = new BucketAggregate();
            subTypeBucketAggregate.Items = A.CollectionOfFake<KeyedBucket>(5);
            aggregationDictionary.Add("subType", subTypeBucketAggregate);

            A.CallTo(() => _nestSearchResponse.Aggregations).Returns(aggregationDictionary);            

            _searchResponseMapper = new SearchResponseMapper();
        }
        

        [Test]
        public void when_mapSubTypes_it_should_have_subType_aggregation()
        {
            var result = _searchResponseMapper.MapSubTypeResponse(_nestSearchResponse);

            Assert.NotNull(result);
            Assert.AreEqual(5, result.Items.Count);
        }
    }
}

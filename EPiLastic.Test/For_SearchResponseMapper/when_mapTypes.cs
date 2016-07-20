using FakeItEasy;
using Nest;
using NUnit.Framework;
using EPiLastic.Models;
using EPiLastic.Services;
using System.Collections.Generic;

namespace EPiLastic.Test.For_SearchResponseMapper
{
    [TestFixture]
    public class when_mapTypes
    {
        private ISearchResponseMapper _searchResponseMapper;
        private ISearchResponse<Page> _nestSearchResponse;

        public when_mapTypes()
        {
            _nestSearchResponse = A.Fake<ISearchResponse<Page>>();

            var aggregationDictionary = new Dictionary<string, IAggregate>();

            var typeBugetAggregate = new BucketAggregate();
            typeBugetAggregate.Items = A.CollectionOfFake<KeyedBucket>(5);
            aggregationDictionary.Add("Type", typeBugetAggregate);

            A.CallTo(() => _nestSearchResponse.Aggregations).Returns(aggregationDictionary);            

            _searchResponseMapper = new SearchResponseMapper();
        }
        

        [Test]
        public void when_mapTypes_it_should_have_Type_aggregation()
        {
            var result = _searchResponseMapper.MapTypeResponse(_nestSearchResponse);

            Assert.NotNull(result);
            Assert.AreEqual(5, result.Items.Count);
        }
    }
}

using NUnit.Framework;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiLastic.Wrappers;
using FakeItEasy;
using EPiServer.Web;
using EPiLastic.Indexing;
using EPiLastic.Indexing.ReIndexJob;
using EPiLastic.Indexing.Services;
using System;

namespace EPiLastic.Test.For_ReIndexJobHandler
{
    [TestFixture]
    public class when_reindex_when_elastic_fails
    {
        private IReIndexJobHandler _reIndexJobHandler;        
        private IIndexClient _indexClient;
        
        public when_reindex_when_elastic_fails()
        {
            var pingResult = A.Fake<Nest.IPingResponse>();
            var exception = new Exception("Could not connect to elastic");
            A.CallTo(() => pingResult.OriginalException).Returns(exception);
            _indexClient = A.Fake<IIndexClient>();
            A.CallTo(() => _indexClient.Ping()).Returns(pingResult);

            _reIndexJobHandler = new ReIndexJobHandler(A.Fake<IDateTimeWrapper>(), A.Fake<IContentLoader>(), A.Fake<ILanguageBranchRepository>(), _indexClient, A.Fake<IPageHelper>(), A.Fake<IIndexingHandler>(), A.Fake<SiteDefinition>());
        }

        [Test]
        public void for_reindexhandler_when_reindex_when_elastic_fails_it_should_throw_exception()
        {
            Exception ex = null;
            try
            {
                _reIndexJobHandler.ReIndex();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.AreEqual("Could not connect to elastic", ex.Message);
            A.CallTo(() => _indexClient.Ping()).MustHaveHappened();
        }
        
    }
}

using EPiLastic.Models;
using System.Threading.Tasks;
using System;
using Nest;
using EPiLastic.Helpers;
using System.Configuration;

namespace EPiLastic.Indexing
{
    public interface IIndexClient
    {
        Task IndexAsyncUsingAlias(Page page, string language);

        void IndexDuringReindex(Page page, string indexName);

        Task DeleteAsync(Guid id, string language);

        void CreateIndex(string indexName);

        void SwapIndexes(string createdIndex, string alias);

        IPingResponse Ping();
    }


    public class IndexClient : IIndexClient
    {
        private readonly IElasticClient _elasticClient;

        public IndexClient()
        {
            var node = new Uri(ConfigurationManager.AppSettings["ElasticSearchUrl"]);

            var settings = new ConnectionSettings(node);
            settings.DefaultIndex(IndexAlias.GetAlias("sv"));

            //settings.DisableDirectStreaming(true); // enable debuging of elastic request / response bodies
        
            _elasticClient = new ElasticClient(settings);
        }

        public void IndexDuringReindex(Page page, string indexName)
        {
            var result = _elasticClient.Index(page, i => i.Index(indexName));
        }

        public async Task IndexAsyncUsingAlias(Page page, string language)
        {
            var alias = IndexAlias.GetAlias(language);
            var result = await _elasticClient.IndexAsync(page, i => i.Index(alias));
        }

        public async Task DeleteAsync(Guid id, string language)
        {
            var alias = IndexAlias.GetAlias(language);
            var deleteResult = await _elasticClient.DeleteAsync(new DeleteRequest<Page>(alias, id.ToString()));
        }

        public void CreateIndex(string indexName)
        {
            var result = _elasticClient.CreateIndexAsync(indexName, i => i
                .Mappings(m => m.Map<Page>(p => p
                    .AutoMap()
                    .Properties(prop => prop
                        .Completion(selector => selector
                            .Name(n => n.AutoComplete)
                            .Payloads(true)
                            .SearchAnalyzer("simple")
                            .Analyzer("simple")
                            .MaxInputLength(50)
                            .PreservePositionIncrements(true)
                            .PreserveSeparators(true)
                            )
                        )
                    )
                )
            );
        }

        public void SwapIndexes(string createdIndex, string alias)
        {
            var indices = _elasticClient.GetIndicesPointingToAlias(alias);

            _elasticClient.Alias(a => a.Add(add => add
                .Index(createdIndex)
                .Alias(alias))
            );

            foreach (var oldIndex in indices)
            {
                _elasticClient.Alias(a => a.Remove(remove => remove
                    .Index(oldIndex)
                    .Alias(alias)
                ));

                _elasticClient.DeleteIndex(oldIndex);
            }
        }

        public IPingResponse Ping()
        {
            return _elasticClient.Ping();
        }
    }
}

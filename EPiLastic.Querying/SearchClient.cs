using Nest;
using EpiLastic.Filters;
using EpiLastic.Helpers;
using EpiLastic.Models;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace EpiLastic.Querying
{
    public interface ISearchClient
    {
        Task<ISuggestResponse> GetAutoCompleteSuggestionsAsync(AutoCompleteQueryFilter filter);

        Task<ISearchResponse<Page>> GetPagesAsync(PagesQueryFilter filter);

        Task<ISearchResponse<Page>> GetTypesAsync(SubTypesQueryFilter filter);

        Task<ISearchResponse<Page>> GetSubTypesAsync(SubTypesQueryFilter filter);
    }   

    public class SearchClient : ISearchClient
    {
        private readonly IElasticClient _elasticClient;

        public SearchClient()
        {
            var node = new Uri(ConfigurationManager.AppSettings["ElasticSearchUrl"]);

            var settings = new ConnectionSettings(node);
            settings.DefaultIndex(IndexAlias.GetAlias("sv"));

            settings.DisableDirectStreaming(true); // enable debuging of elastic request / response bodies

            _elasticClient = new ElasticClient(settings);
        }

        public async Task<ISuggestResponse> GetAutoCompleteSuggestionsAsync(AutoCompleteQueryFilter filter)
        {
            var alias = IndexAlias.GetAlias(filter.Language);
            var response = await _elasticClient.SuggestAsync<Page>(x => 
                x.Completion("autocomplete", y => y
                    .Text(filter.Q)
                    .Field(f => f.AutoComplete)
                    .Size(10))
            .Index(alias));

            return response;
        }

        public async Task<ISearchResponse<Page>> GetPagesAsync(PagesQueryFilter filter)
        {

            var alias = IndexAlias.GetAlias(filter.Language);

            var response = await _elasticClient.SearchAsync<Page>(x => x
            .Query(q => q     
                .Bool( b => b                    
                    .Filter(f => f.Term(t => t.Type, filter.Type), f => f.Term(t => t.SubType, filter.SubType))
                    .Must(m => m
                        .QueryString(qs => qs                
                            .Query(filter.Query)
                            .Fields(f => f.Field(fs => fs.HiddenKeywords).Field(fs => fs.MainBody).Field(fs => fs.Name).Field("blocks.title").Field("blocks.mainBody"))
                        )
                    )                                           
                )
            )
            .Aggregations(a => a
                .Terms("type", t => t
                    .Field(f => f.Type)
                    .MinimumDocumentCount(2)                    
                )  
                .Terms("subType", t => t
                    .Field(f => f.SubType)
                    .MinimumDocumentCount(2)
                )          
            )          
            .Highlight(h => h
                .Fields(f => f.Field(ff => ff.MainBody), f => f.Field("blocks.mainBody"))            
                .PreTags("<b>")
                .PostTags("</b>")
            )          
            .Index(alias)            
            .Take(filter.Size));

            return response;
        }

        public async Task<ISearchResponse<Page>> GetTypesAsync(SubTypesQueryFilter filter)
        {
            var alias = IndexAlias.GetAlias(filter.Language);

            var response = await _elasticClient.SearchAsync<Page>(x => x            
            .Aggregations(a => a
                .Terms("Type", t => t
                    .Field(f => f.Type)
                    .MinimumDocumentCount(1)
                )
            )
            .Index(alias));

            return response;
        }

        public async Task<ISearchResponse<Page>> GetSubTypesAsync(SubTypesQueryFilter filter)
        {
            var alias = IndexAlias.GetAlias(filter.Language);

            var response = await _elasticClient.SearchAsync<Page>(x => x
            .Query(q => q
                .Bool(b => b
                   .Filter(f => f.Term(t => t.Type, filter.Type))
                )
            )
            .Aggregations(a => a
                .Terms("subType", t => t
                    .Field(f => f.SubType)
                    .MinimumDocumentCount(1)
                )
            )
            .Index(alias));

            return response;
        }
    }
}

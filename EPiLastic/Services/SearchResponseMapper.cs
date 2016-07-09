using Nest;
using EpiLastic.Models;
using EpiLastic.Models.Responses;
using System.Collections.Generic;
using System;

namespace EpiLastic.Services
{
    public interface ISearchResponseMapper
    {
        PagesSearchResponse Map(ISearchResponse<Page> searchResponse);

        AggregationResultContainer MapSubTypeResponse(ISearchResponse<Page> searchResponse);

        List<AutoCompleteResponse> Map(ISuggestResponse suggestResponse);
    }

    public class SearchResponseMapper : ISearchResponseMapper
    {
        public PagesSearchResponse Map(ISearchResponse<Page> searchResponse)
        {
            var result = new PagesSearchResponse();
            result.Pages = new List<PageSearchResponse>();
            result.Aggregations = new Dictionary<string, AggregationResultContainer>();

            foreach(var document in searchResponse.Documents)
            {
                var page = new PageSearchResponse()
                {
                    Id = document.ContentGuid.ToString(),
                    Created = document.Created,
                    Name = document.Name,
                    NavigateUrl = document.NavigateUrl,
                    Type = document.Type,
                    SubType = document.SubType,
                    TeaserDescription = document.TeaserText,
                    TeaserImageurl = document.TeaserImageUrl
                };
                result.Pages.Add(page);
            }

            result.Count = (int)searchResponse.Total;


            if (searchResponse.Aggregations != null)
            {
                foreach (var key in searchResponse.Aggregations.Keys)
                {
                    var aggregationResultContainer = new AggregationResultContainer();
                    var aggregationItems = new List<AggregationItem>();

                    foreach (KeyedBucket item in ((BucketAggregate)searchResponse.Aggregations[key]).Items)
                    {
                        aggregationItems.Add(new AggregationItem()
                        {
                            Count = item.DocCount,
                            Name = item.Key
                        });
                    }
                    aggregationResultContainer.Items = aggregationItems;
                    result.Aggregations.Add(key, aggregationResultContainer);
                }
            }

            return result;
        }

        public List<AutoCompleteResponse> Map(ISuggestResponse suggestResponse)
        {
            var response = new List<AutoCompleteResponse>();

            if (suggestResponse.Suggestions == null || suggestResponse.Suggestions["autocomplete"] == null)
                return response;

            var autoCompleteSuggestions = suggestResponse.Suggestions["autocomplete"];
            
            foreach(var suggestion in autoCompleteSuggestions[0].Options)
            {
                var autoCompleteResponse = new AutoCompleteResponse();

                autoCompleteResponse.Suggestion = suggestion.Text;
                
                autoCompleteResponse.Type = suggestion.Payload<dynamic>().type;

                response.Add(autoCompleteResponse);
            }

            return response;
        }

        public AggregationResultContainer MapSubTypeResponse(ISearchResponse<Page> searchResponse)
        {
            var aggregationResultContainer = new AggregationResultContainer();

            if (searchResponse.Aggregations != null)
            {
                foreach (var key in searchResponse.Aggregations.Keys)
                {
                    var aggregationItems = new List<AggregationItem>();

                    foreach (KeyedBucket item in ((BucketAggregate)searchResponse.Aggregations[key]).Items)
                    {
                        aggregationItems.Add(new AggregationItem()
                        {
                            Count = item.DocCount,
                            Name = item.Key
                        });
                    }
                    aggregationResultContainer.Items = aggregationItems;
                }
            }

            return aggregationResultContainer;
        }
    }
}

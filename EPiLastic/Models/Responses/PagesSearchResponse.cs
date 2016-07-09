using System;
using System.Collections.Generic;

namespace EpiLastic.Models.Responses
{
    public class PagesSearchResponse
    {
        public int Count { get; set; }

        public List<PageSearchResponse> Pages { get; set; }

        public Dictionary<string, AggregationResultContainer> Aggregations { get; set; }
    }

    public class PageSearchResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TeaserDescription { get; set; }

        public string TeaserImageurl { get; set; }

        public string NavigateUrl { get; set; }
        
        public string Type { get; set; }

        public string SubType { get; set; }

        public DateTime Created { get; set; }    
    }
}

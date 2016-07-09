using Nest;
using System;
using System.Collections.Generic;

namespace EpiLastic.Models
{
    [ElasticsearchType(Name = "page", IdProperty = "ContentGuid")]
    public class Page
    {
        [String]
        public Guid ContentGuid { get; set; }

        [String(Index = FieldIndexOption.No)]
        public string NavigateUrl { get; set; }

        [String(Index = FieldIndexOption.No)]
        public string TeaserImageUrl { get; set; }

        [String]
        public string TeaserText { get; set; }

        public string Name { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed)]
        public string Type { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed)]
        public string SubType { get; set; }

        [Date]
        public DateTime Created { get; set; }

        [Nested(IncludeInParent = true)]
        public List<Block> Blocks { get; set; }

        public string MainBody { get; set; }

        public SuggestField AutoComplete { get; set; }

        public string HiddenKeywords { get; set; }
    }
}

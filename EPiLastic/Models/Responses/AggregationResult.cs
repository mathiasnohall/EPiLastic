using System.Collections.Generic;

namespace EpiLastic.Models.Responses
{
    public class AggregationResultContainer
    {
        public List<AggregationItem> Items { get; set; }
    }

    public class AggregationItem
    {
        public string Name { get; set; }

        public long? Count { get; set; }
    }
}

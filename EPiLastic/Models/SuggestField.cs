using System.Collections.Generic;

namespace EpiLastic.Models
{
    public class SuggestField
    {
        public IEnumerable<string> Input { get; set; }
        public string Output { get; set; }
        public object Payload { get; set; }
        public int? Weight { get; set; }
    }
}

namespace EpiLastic.Filters
{
    public class AutoCompleteQueryFilter
    {
        public AutoCompleteQueryFilter()
        {
            Language = "sv";
        }

        public string Q { get; set; }

        public string Language { get; set; }
    }
}

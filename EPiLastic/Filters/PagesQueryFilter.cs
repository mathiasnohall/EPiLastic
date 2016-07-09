namespace EpiLastic.Filters
{
    public class PagesQueryFilter
    {
        public PagesQueryFilter()
        {
            Size = 12;
            Language = "sv";
        }

        public string Query { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public int Size { get; set; }

        public string Language { get; set; }
    }
}

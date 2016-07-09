namespace EpiLastic.Models
{
    public interface ISearchablePage
    {
        string HiddenKeywords { get; set; }

        bool ExcludeFromSearch { get; set; }

        Page Map();
    }
}

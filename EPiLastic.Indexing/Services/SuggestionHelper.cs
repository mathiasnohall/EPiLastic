using System.Collections.Generic;

namespace EpiLastic.Indexing.Services
{
    public interface ISuggestionHelper
    {
        string[] GeneratePageSuggestions(string pageName);
    }

    public class SuggestionHelper : ISuggestionHelper
    {
        public string[] GeneratePageSuggestions(string pageName)
        {
            var result = new List<string>();

            var splitedString = pageName.Split(new [] { ' ' }, 5);

            if (splitedString.Length > 0)
                result.AddRange(splitedString);

            if (splitedString.Length > 1)
                result.Add(pageName);

            return result.ToArray();
        }
    }
}

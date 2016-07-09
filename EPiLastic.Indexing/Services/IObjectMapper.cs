using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using System;

namespace EPiLastic.Indexing.Services
{
    public interface IObjectMapper
    {
        Page Map(ISearchablePage page);

        Block Map(ISearchableBlock block);
    }


    public class ObjectMapper : IObjectMapper
    {
        private readonly ISuggestionHelper _suggestionHelper;

        public ObjectMapper(ISuggestionHelper suggestionHelper)
        {
            _suggestionHelper = suggestionHelper;
        }

        public Block Map(ISearchableBlock block)
        {
            throw new NotImplementedException();
        }

        public Page Map(ISearchablePage page)
        {
            throw new NotImplementedException();
        }
    }
}

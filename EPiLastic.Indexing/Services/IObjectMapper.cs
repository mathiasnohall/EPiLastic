using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EPiServer.Web.Routing;
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
        private readonly UrlResolver _urlResolver;

        public ObjectMapper(ISuggestionHelper suggestionHelper, UrlResolver urlResolver)
        {
            _suggestionHelper = suggestionHelper;
            _urlResolver = urlResolver;
        }

        public Block Map(ISearchableBlock block)
        {
            var mappedBlock = new Block();

            return mappedBlock;
        }

        public Page Map(ISearchablePage page)
        {
            throw new NotImplementedException();
        }
    }
}

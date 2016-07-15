using EpiLastic.Indexing.Services;
using EpiLastic.Models;
using EPiLastic.Attributes;
using EPiLastic.Helpers;
using EPiServer.Core;
using EPiServer.Web.Routing;
using System;
using System.Linq;

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

            var properties = block.GetType().GetProperties().ToDictionary(x => x.Name);

            foreach (var property in properties)
            {
                var propertyInfo = block.GetType().GetProperty(property.Key);
                if (propertyInfo != null)
                {
                    var titleAttribute = propertyInfo.GetCustomAttributes(typeof(TitleAttribute), false).FirstOrDefault();
                    if (titleAttribute != null)
                    {
                        var value = propertyInfo.GetValue(block, null) as string;
                        mappedBlock.Title = value;
                        continue;
                    }

                    var textAttribute = propertyInfo.GetCustomAttributes(typeof(TextAttribute), false).FirstOrDefault();

                    if (textAttribute != null)
                    {
                        var value = propertyInfo.GetValue(block, null) as XhtmlString;
                        if (value != null)
                        {
                            mappedBlock.MainBody = value.ToHtmlString().StripHtml();
                        }
                    }



                }
            }

            return mappedBlock;
        }

        public Page Map(ISearchablePage page)
        {
            throw new NotImplementedException();
        }
    }
}

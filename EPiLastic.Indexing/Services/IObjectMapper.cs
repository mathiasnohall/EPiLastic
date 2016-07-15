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
                        var xhtmlStringValue = propertyInfo.GetValue(block, null) as XhtmlString;
                        if (xhtmlStringValue != null)
                        {
                            mappedBlock.MainBody += mappedBlock.MainBody != null ? " " + xhtmlStringValue.ToHtmlString().StripHtml() : xhtmlStringValue.ToHtmlString().StripHtml();
                        }

                        var stringValue = propertyInfo.GetValue(block, null) as string;
                        if(stringValue != null)
                            mappedBlock.MainBody += mappedBlock.MainBody != null ? " " + stringValue : stringValue;
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

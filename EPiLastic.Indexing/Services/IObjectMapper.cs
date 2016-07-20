using EPiLastic.Models;
using EPiLastic.Attributes;
using EPiLastic.Helpers;
using EPiServer.Core;
using EPiServer.Web.Routing;
using System.Linq;
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

            var blockTypeProxy = block.GetType();
            var blockType = Type.GetType(blockTypeProxy.BaseType.FullName + ", " + blockTypeProxy.BaseType.Assembly.FullName);

            var properties = blockType.GetProperties().ToDictionary(x => x.Name);

            foreach (var property in properties)
            {
                var propertyInfo = blockType.GetProperty(property.Key);
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
            var mappedPage = new Page();

            mappedPage.Name = ((PageData)page).Name;

            mappedPage.NavigateUrl = _urlResolver.GetUrl(((PageData)page).ContentLink, ((PageData)page).Language.TwoLetterISOLanguageName);
            mappedPage.ContentGuid = ((PageData)page).ContentGuid;
            mappedPage.Created = ((PageData)page).Created;

            mappedPage.AutoComplete = new SuggestField();
            mappedPage.AutoComplete.Input = _suggestionHelper.GeneratePageSuggestions(((PageData)page).Name);
            mappedPage.AutoComplete.Output = ((PageData)page).Name;

            var pageTypeProxy = page.GetType();            
            var pageType = Type.GetType(pageTypeProxy.BaseType.FullName + ", " + pageTypeProxy.BaseType.Assembly.FullName);

            var properties = pageType.GetProperties().ToDictionary(x => x.Name);

            foreach (var property in properties)
            {
                var propertyInfo = pageType.GetProperty(property.Key);
                if (propertyInfo != null)
                {

                    var attributes = propertyInfo.GetCustomAttributes(false);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(TitleAttribute))
                        {

                            var value = propertyInfo.GetValue(page, null) as string;
                            if (value != null)
                                mappedPage.Name = value;
                        }

                        if (attribute.GetType() == typeof(TextAttribute))
                        {
                            var xhtmlStringValue = propertyInfo.GetValue(page, null) as XhtmlString;
                            if (xhtmlStringValue != null)
                            {
                                mappedPage.MainBody += mappedPage.MainBody != null ? " " + xhtmlStringValue.ToHtmlString().StripHtml() : xhtmlStringValue.ToHtmlString().StripHtml();
                            }

                            var stringValue = propertyInfo.GetValue(page, null) as string;
                            if (stringValue != null)
                                mappedPage.MainBody += mappedPage.MainBody != null ? " " + stringValue : stringValue;

                        }
                        if (attribute.GetType() == typeof(HiddenKeywordsAttribute))
                        {
                            var keyWords = propertyInfo.GetValue(page, null) as string;
                            if (keyWords != null)
                                mappedPage.HiddenKeywords = keyWords;
                        }

                        if (attribute.GetType() == typeof(TypeAttribute))
                        {
                            var type = propertyInfo.GetValue(page, null) as string;
                            if (type != null)
                                mappedPage.Type = type;
                        }

                        if (attribute.GetType() == typeof(SubTypeAttribute))
                        {
                            var subType = propertyInfo.GetValue(page, null) as string;
                            if (subType != null)
                                mappedPage.SubType = subType;
                        }

                        if (attribute.GetType() == typeof(TeaserImageAttribute))
                        {
                            var teaserImageReference = propertyInfo.GetValue(page, null) as ContentReference;
                            if (teaserImageReference != null)
                                mappedPage.TeaserImageUrl = _urlResolver.GetUrl(teaserImageReference);
                        }

                        if (attribute.GetType() == typeof(TeaserTextAttribute))
                        {
                            var xhtmlStringValue = propertyInfo.GetValue(page, null) as XhtmlString;
                            if (xhtmlStringValue != null)
                            {
                                mappedPage.TeaserText = xhtmlStringValue.ToHtmlString().StripHtml();
                            }

                            var stringValue = propertyInfo.GetValue(page, null) as string;
                            if (stringValue != null)
                                mappedPage.TeaserText = stringValue;

                        }
                    }
                }
            }

            return mappedPage;
        }
    }
}

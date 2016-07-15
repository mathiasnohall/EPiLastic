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
            var mappedPage = new Page();

            mappedPage.NavigateUrl = _urlResolver.GetUrl(((PageData)page).ContentLink, ((PageData)page).Language.TwoLetterISOLanguageName);
            mappedPage.ContentGuid = ((PageData)page).ContentGuid;
            mappedPage.Created = ((PageData)page).Created;

            mappedPage.AutoComplete = new SuggestField();
            mappedPage.AutoComplete.Input = _suggestionHelper.GeneratePageSuggestions(((PageData)page).Name);
            mappedPage.AutoComplete.Output = ((PageData)page).Name;

            var properties = page.GetType().GetProperties().ToDictionary(x => x.Name);

            foreach (var property in properties)
            {
                var propertyInfo = page.GetType().GetProperty(property.Key);
                if (propertyInfo != null)
                {
                    var titleAttribute = propertyInfo.GetCustomAttributes(typeof(TitleAttribute), false).FirstOrDefault();
                    if (titleAttribute != null)
                    {
                        var value = propertyInfo.GetValue(page, null) as string;
                        mappedPage.Name = value;
                        continue;
                    }

                    var textAttribute = propertyInfo.GetCustomAttributes(typeof(TextAttribute), false).FirstOrDefault();

                    if (textAttribute != null)
                    {
                        var xhtmlStringValue = propertyInfo.GetValue(page, null) as XhtmlString;
                        if (xhtmlStringValue != null)
                        {
                            mappedPage.MainBody += mappedPage.MainBody != null ? " " + xhtmlStringValue.ToHtmlString().StripHtml() : xhtmlStringValue.ToHtmlString().StripHtml();
                        }

                        var stringValue = propertyInfo.GetValue(page, null) as string;
                        if (stringValue != null)
                            mappedPage.MainBody += mappedPage.MainBody != null ? " " + stringValue : stringValue;

                        continue;
                    }

                    var hiddenKeyWordsAttribute = propertyInfo.GetCustomAttributes(typeof(HiddenKeywordsAttribute), false).FirstOrDefault();
                    if(hiddenKeyWordsAttribute != null)
                    {
                        var keyWords = propertyInfo.GetValue(page, null) as string;
                        if (keyWords != null)
                            mappedPage.HiddenKeywords = keyWords;
                        continue;
                    }

                    var typeAttribute = propertyInfo.GetCustomAttributes(typeof(TypeAttribute), false).FirstOrDefault();
                    if (typeAttribute != null)
                    {
                        var type = propertyInfo.GetValue(page, null) as string;
                        if (type != null)
                            mappedPage.Type = type;
                        continue;
                    }

                    var subTypeAttribute = propertyInfo.GetCustomAttributes(typeof(SubTypeAttribute), false).FirstOrDefault();
                    if (subTypeAttribute != null)
                    {
                        var subType = propertyInfo.GetValue(page, null) as string;
                        if (subType != null)
                            mappedPage.SubType = subType;
                        continue;
                    }

                    var teaserImageAttribute = propertyInfo.GetCustomAttributes(typeof(TeaserImageAttribute), false).FirstOrDefault();
                    if (teaserImageAttribute != null)
                    {
                        var teaserImageReference = propertyInfo.GetValue(page, null) as ContentReference;
                        if (teaserImageReference != null)
                            mappedPage.TeaserImageUrl = _urlResolver.GetUrl(teaserImageReference);
                        continue;
                    }


                    var teaserTextAttribute = propertyInfo.GetCustomAttributes(typeof(TeaserTextAttribute), false).FirstOrDefault();

                    if (teaserTextAttribute != null)
                    {
                        var xhtmlStringValue = propertyInfo.GetValue(page, null) as XhtmlString;
                        if (xhtmlStringValue != null)
                        {
                            mappedPage.TeaserText = xhtmlStringValue.ToHtmlString().StripHtml();
                        }

                        var stringValue = propertyInfo.GetValue(page, null) as string;
                        if (stringValue != null)
                            mappedPage.TeaserText = stringValue;

                        continue;
                    }
                }
            }

            return mappedPage;
        }
    }
}

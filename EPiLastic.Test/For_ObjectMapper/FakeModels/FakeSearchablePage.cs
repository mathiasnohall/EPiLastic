using EpiLastic.Models;
using EPiLastic.Attributes;
using EPiServer.Core;
using System;

namespace EPiLatic.Test.For_ObjectMapper.FakeModels
{
    public class FakeSearchablePage : PageData, ISearchablePage
    {

        [HiddenKeywords]
        public string HiddenKeyWord { get; set; }

        [Title]
        public string Title { get; set; }

        [Text]
        public XhtmlString MainBody { get; set; }

        [Type]
        public string Type { get; set; }

        [SubType]
        public string SubType { get; set; }


        [TeaserText]
        public string TeaserText { get; set; }

        [TeaserImage]
        public ContentReference TeaserImage { get; set; }


        public bool ExcludeFromSearch { get; set; }
    }
}

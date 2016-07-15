using EpiLastic.Models;
using EPiLastic.Attributes;
using EPiServer.Core;
using System;

namespace EPiLatic.Test.For_ObjectMapper.FakeModels
{
    public class FakeSearchableBlock : BlockData, ISearchableBlock, IContent
    {
        [Title]
        public string Title { get; set; }

        [Text]
        public XhtmlString MainBody { get; set; }

        [Text]
        public string SecondBody { get; set; }

        [Text]
        public XhtmlString ThirdBody { get; set; }


        #region IContent

        public Guid ContentGuid { get; set; }

        public ContentReference ContentLink { get; set; }

        public int ContentTypeID { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        public ContentReference ParentLink { get; set; }

        #endregion
    }
}

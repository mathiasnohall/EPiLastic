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

        #region IContent

        public Guid ContentGuid
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ContentReference ContentLink
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int ContentTypeID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsDeleted
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ContentReference ParentLink
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

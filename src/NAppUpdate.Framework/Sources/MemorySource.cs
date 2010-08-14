using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Sources
{
    public class MemorySource : IUpdateSource
    {
        public MemorySource(string feedXml)
        {
            this.FeedXml = feedXml;
        }

        public string FeedXml { get; set; }

        #region IUpdateSource Members

        public string GetUpdatesFeed()
        {
            return FeedXml;
        }

        public byte[] GetFile(string url)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

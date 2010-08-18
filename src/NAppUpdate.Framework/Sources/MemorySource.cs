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

        public bool GetData(string filePath, string basePath, ref string tempFile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

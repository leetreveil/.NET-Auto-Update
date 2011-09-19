using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Sources
{
    public class MemorySource : IUpdateSource
    {
        public MemorySource(string feedString)
        {
			this.Feed = feedString;
        }

        public string Feed { get; set; }

        #region IUpdateSource Members

        public string GetUpdatesFeed()
        {
            return Feed;
        }

        public bool GetData(string filePath, string basePath, ref string tempFile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

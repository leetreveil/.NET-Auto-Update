using System;
using NAppUpdate.Framework.Common;

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

		public bool GetData(string filePath, string basePath, Action<UpdateProgressInfo> onProgress, ref string tempLocation)
    	{
    		throw new NotImplementedException();
    	}

        #endregion
    }
}

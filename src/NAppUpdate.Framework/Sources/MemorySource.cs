using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAppUpdate.Framework.Sources
{
    public class MemorySource : IUpdateSource
    {
        public MemorySource(string feedXml)
        {
            this._feedXml = feedXml;
        }

        private string _feedXml;

        #region IUpdateSource Members

        public string GetUpdatesFeed()
        {
            return _feedXml;
        }

        public byte[] GetFile(string url)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

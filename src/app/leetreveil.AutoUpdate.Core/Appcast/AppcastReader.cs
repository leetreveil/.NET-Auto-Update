using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace leetreveil.AutoUpdate.Core.Appcast
{
    public class AppcastReader
    {
        private XDocument _appcastDoc;

        public AppcastReader(Uri url)
        {
            _appcastDoc = XDocument.Load(url.AbsolutePath);
        }

        public IEnumerable<AppcastItem> Read()
        {
            throw new NotImplementedException();
        }
    }
}
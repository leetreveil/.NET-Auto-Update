using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace leetreveil.AutoUpdate.Core.Appcast
{
    public class AppcastReader
    {
        private readonly XDocument _appcastDoc;

        public AppcastReader(string url)
        {
            _appcastDoc = XDocument.Load(url);
        }

        public IEnumerable<AppcastItem> Read()
        {
            XNamespace ns = "http://www.adobe.com/xml-namespaces/appcast/1.0";

            return _appcastDoc.Descendants("channel").Descendants("item").Select(
                item => new AppcastItem
                            {
                                Title = item.Element("title").Value,
                                Version = item.Element(ns + "version").Value,
                                FileUrl = item.Element("enclosure").Attribute("url").Value
                            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml;

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

            IEnumerable<AppcastItem> query =
                _appcastDoc.Descendants("channel").Descendants("item").Select(
                    element => new AppcastItem {Title = element.Element("title").Value, 
                                                Version = element.Element(ns + "version").Value,
                                                FileUrl = element.Element("enclosure").Attribute("url").Value
                    });

            return query;
        }
    }
}
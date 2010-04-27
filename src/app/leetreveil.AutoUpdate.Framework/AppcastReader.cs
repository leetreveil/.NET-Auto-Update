using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace leetreveil.AutoUpdate.Framework
{
    public class AppcastReader : IUpdateFeedSource
    {
        public IEnumerable<Update> Read(string url)
        {
            var document = XDocument.Load(url);

            XNamespace ns = "http://www.adobe.com/xml-namespaces/appcast/1.0";

            return document.Descendants("channel").Descendants("item").Select(
                item => new Update
                            {
                                Title = item.Element("title").Value,
                                Version = new Version(item.Element(ns + "version").Value),
                                FileUrl = item.Element("enclosure").Attribute("url").Value,
                                FileLength = Convert.ToInt64(item.Element("enclosure").Attribute("length").Value)
                            });
        }
    }
}
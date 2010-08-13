using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Tasks;
using System.Xml;
using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.FeedReaders
{
    public class AppcastReader : IUpdateFeedSource, IUpdateFeedReader
    {
        // http://learn.adobe.com/wiki/display/ADCdocs/Appcasting+RSS

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

        #region IUpdateFeedReader Members

        public IEnumerable<IUpdateTask> Read(UpdateManager caller, string feed)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(feed);
            XmlNodeList nl = doc.SelectNodes("/rss/channel/item");

            List<IUpdateTask> ret = new List<IUpdateTask>();

            foreach (XmlNode n in nl)
            {
                FileUpdateTask task = new FileUpdateTask();
                task.Description = n["description"].InnerText;
                task.Attributes.Add("remotePath", n["enclosure"].Attributes["url"].Value);

                FileVersionCondition cnd = new FileVersionCondition();
                cnd.Attributes.Add("version", n["appcast:version"].InnerText);
                task.UpdateConditions.AddCondition(cnd, BooleanCondition.ConditionType.AND);

                ret.Add(task);
            }

            return ret;
        }

        #endregion
    }
}
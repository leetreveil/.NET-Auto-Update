using System;
using System.Xml;
using System.Collections.Generic;

using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Tasks;
using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.FeedReaders
{
    public class AppcastReader : IUpdateFeedReader
    {
        // http://learn.adobe.com/wiki/display/ADCdocs/Appcasting+RSS

        #region IUpdateFeedReader Members

        public IList<IUpdateTask> Read(string feed)
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
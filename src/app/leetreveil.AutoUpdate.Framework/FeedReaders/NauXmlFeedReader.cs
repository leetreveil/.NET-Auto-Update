using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using leetreveil.AutoUpdate.Framework.Tasks;
using leetreveil.AutoUpdate.Framework.Conditions;

namespace leetreveil.AutoUpdate.Framework.FeedReaders
{
    public class NauXmlFeedReader : IUpdateFeedReader
    {
        #region IUpdateFeedReader Members

        public IEnumerable<IUpdateTask> Read(UpdateManager caller, string feed)
        {
            List<IUpdateTask> ret = new List<IUpdateTask>();

            // TODO: Version

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(feed);

            XmlNodeList nl = doc.SelectNodes("/tasks/task");
            foreach (XmlNode node in nl)
            {
                // Find the requested task type and create a new instance of it
                if (!caller._updateTasks.ContainsKey(node.Attributes["type"].Value))
                    continue;

                IUpdateTask task = (IUpdateTask)Activator.CreateInstance(caller._updateTasks[node.Attributes["type"].Value]);

                // Store all other task attributes, to be used by the task object later
                foreach (XmlAttribute att in node.Attributes)
                {
                    if ("type".Equals(att.Name))
                        continue;

                    task.Attributes.Add(att.Name, att.Value);
                }

                // TODO: Read description

                // Read update conditions
                IUpdateCondition conditionObject = ReadCondition(caller, node);
                if (conditionObject != null)
                {
                    if (conditionObject is BooleanCondition)
                        task.UpdateConditions = conditionObject as BooleanCondition;
                    else
                        task.UpdateConditions.AddCondition(conditionObject);
                }

                ret.Add(task);
            }
            return ret;
        }

        private IUpdateCondition ReadCondition(UpdateManager caller, XmlNode cnd)
        {
            IUpdateCondition conditionObject = null;
            if (cnd.ChildNodes.Count > 0)
            {
                BooleanCondition bc = new BooleanCondition();
                XmlNodeList conditionNodes = cnd.SelectNodes("/condition");
                foreach (XmlNode child in conditionNodes)
                {
                    IUpdateCondition childCondition = ReadCondition(caller, child);
                    if (childCondition != null)
                        bc.AddCondition(conditionObject, BooleanCondition.ConditionTypeFromString(cnd.Attributes["type"].Value));
                }
                if (bc.ChildConditionsCount > 0)
                    conditionObject = bc;
            }
            else if (caller._updateConditions.ContainsKey(cnd.Attributes["check"].Value))
            {
                conditionObject = (IUpdateCondition)Activator.CreateInstance(caller._updateTasks[cnd.Attributes["check"].Value]);
            }
            return conditionObject;
        }

        #endregion
    }
}

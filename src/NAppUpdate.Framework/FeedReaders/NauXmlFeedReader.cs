using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using NAppUpdate.Framework.Tasks;
using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.FeedReaders
{
    public class NauXmlFeedReader : IUpdateFeedReader
    {
        private Dictionary<string, Type> _updateConditions { get; set; }
        private Dictionary<string, Type> _updateTasks { get; set; }

        public NauXmlFeedReader()
        {
            _updateConditions = new Dictionary<string, Type>();
            _updateTasks = new Dictionary<string, Type>();

            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (typeof(IUpdateTask).IsAssignableFrom(t))
                {
                    _updateTasks.Add(t.Name, t);
                    UpdateTaskAliasAttribute[] tasksAliases = (UpdateTaskAliasAttribute[])t.GetCustomAttributes(typeof(UpdateTaskAliasAttribute), false);
                    foreach (UpdateTaskAliasAttribute alias in tasksAliases)
                    {
                        _updateTasks.Add(alias.Alias, t);
                    }
                }
                else if (typeof(IUpdateCondition).IsAssignableFrom(t))
                {
                    _updateConditions.Add(t.Name, t);
                    UpdateConditionAliasAttribute[] tasksAliases = (UpdateConditionAliasAttribute[])t.GetCustomAttributes(typeof(UpdateConditionAliasAttribute), false);
                    foreach (UpdateConditionAliasAttribute alias in tasksAliases)
                    {
                        _updateConditions.Add(alias.Alias, t);
                    }
                }
            }
        }

        #region IUpdateFeedReader Members

        public IList<IUpdateTask> Read(string feed)
        {
            List<IUpdateTask> ret = new List<IUpdateTask>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(feed);

            // Support for different feed versions
            XmlNode root = doc.SelectSingleNode(@"/Feed[version=""1.0""] | /Feed");
            if (root == null) root = doc;

            XmlNodeList nl = root.SelectNodes("./Tasks/*");
            foreach (XmlNode node in nl)
            {
                // Find the requested task type and create a new instance of it
                if (!_updateTasks.ContainsKey(node.Name))
                    continue;

                IUpdateTask task = (IUpdateTask)Activator.CreateInstance(_updateTasks[node.Name]);

                // Store all other task attributes, to be used by the task object later
                foreach (XmlAttribute att in node.Attributes)
                {
                    if ("type".Equals(att.Name))
                        continue;

                    task.Attributes.Add(att.Name, att.Value);
                }

                if (node.HasChildNodes)
                {
                    if (node["Description"] != null)
                        task.Description = node["Description"].InnerText;

                    // Read update conditions
                    if (node["Conditions"] != null)
                    {
                        IUpdateCondition conditionObject = ReadCondition(node["Conditions"]);
                        if (conditionObject != null)
                        {
                            if (conditionObject is BooleanCondition)
                                task.UpdateConditions = conditionObject as BooleanCondition;
                            else
                                task.UpdateConditions.AddCondition(conditionObject);
                        }
                    }
                }

                ret.Add(task);
            }
            return ret;
        }

        private IUpdateCondition ReadCondition(XmlNode cnd)
        {
            IUpdateCondition conditionObject = null;
            if (cnd.ChildNodes.Count > 0 || "GroupCondition".Equals(cnd.Name))
            {
                BooleanCondition bc = new BooleanCondition();
                foreach (XmlNode child in cnd.ChildNodes)
                {
                    IUpdateCondition childCondition = ReadCondition(child);
                    if (childCondition != null)
                        bc.AddCondition(conditionObject, BooleanCondition.ConditionTypeFromString(child.Attributes["type"] == null ? null : child.Attributes["type"].Value));
                }
                if (bc.ChildConditionsCount > 0)
                    conditionObject = bc.Degrade();
            }
            else if (_updateConditions.ContainsKey(cnd.Name))
            {
                conditionObject = (IUpdateCondition)Activator.CreateInstance(_updateConditions[cnd.Name]);

                // Store all other attributes, to be used by the condition object later
                foreach (XmlAttribute att in cnd.Attributes)
                {
                    if ("type".Equals(att.Name))
                        continue;

                    conditionObject.Attributes.Add(att.Name, att.Value);
                }
            }
            return conditionObject;
        }

        #endregion
    }
}

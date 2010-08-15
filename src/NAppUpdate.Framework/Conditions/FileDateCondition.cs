using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Framework.Conditions
{
    public class FileDateCondition : IUpdateCondition
    {
        public FileDateCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsMet(NAppUpdate.Framework.Tasks.IUpdateTask task)
        {
            DateTime fileDateTime;
            if (!Attributes.ContainsKey("timestamp") || !DateTime.TryParse(Attributes["timestamp"], out fileDateTime))
                return true;

            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            if (!File.Exists(localPath))
                return true;

            DateTime localFileDateTime = File.GetLastWriteTime(localPath);
            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "newer":
                        return localFileDateTime > fileDateTime;
                    case "is":
                        return localFileDateTime.Equals(fileDateTime);
                }
            }
            return localFileDateTime < fileDateTime; // == what="older"
        }

        #endregion
    }
}

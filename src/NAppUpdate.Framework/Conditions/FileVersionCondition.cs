using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace NAppUpdate.Framework.Conditions
{
    [UpdateConditionAlias("version")]
    public class FileVersionCondition : IUpdateCondition
    {
        public FileVersionCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsMet(NAppUpdate.Framework.Tasks.IUpdateTask task)
        {
            if (!Attributes.ContainsKey("version"))
                return true;

            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            if (!System.IO.File.Exists(localPath))
                return true;

            string versionString = FileVersionInfo.GetVersionInfo(localPath).FileVersion.Replace(", ", ".");
            Version localVersion = new Version(versionString);
            Version updateVersion = new Version(Attributes["version"]);

            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "above":
                        return updateVersion < localVersion;
                    case "is":
                        return updateVersion == localVersion;
                }
            }
            return updateVersion > localVersion;
        }

        #endregion
    }
}

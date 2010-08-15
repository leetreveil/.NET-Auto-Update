using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Framework.Conditions
{
    [UpdateConditionAlias("exists")]
    public class FileExistsCondition : IUpdateCondition
    {
        public FileExistsCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsMet(NAppUpdate.Framework.Tasks.IUpdateTask task)
        {
            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            return System.IO.File.Exists(localPath);
        }

        #endregion
    }
}

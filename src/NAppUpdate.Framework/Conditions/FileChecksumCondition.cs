using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Framework.Conditions
{
    public class FileChecksumCondition : IUpdateCondition
    {
        public FileChecksumCondition()
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

            if (!File.Exists(localPath))
                return true;

            if (Attributes.ContainsKey("sha256-checksum"))
            {
                string sha256 = Utils.FileChecksum.GetSHA256Checksum(localPath);
                if (!string.IsNullOrEmpty(sha256) && sha256.Equals(Attributes["sha256-checksum"]))
                    return true;
            }

            // TODO: Support more checksum algorithms (although SHA256 has no collisions, other are more commonly used)

            return false;
        }

        #endregion
    }
}

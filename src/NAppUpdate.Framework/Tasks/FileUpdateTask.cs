using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Framework.Tasks
{
    [UpdateTaskAlias("fileUpdate")]
    public class FileUpdateTask : IUpdateTask
    {
        public FileUpdateTask()
        {
            Attributes = new Dictionary<string, string>();
            UpdateConditions = new NAppUpdate.Framework.Conditions.BooleanCondition();
        }

        internal string tempFile = null;
        private string destinationFile;

        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; set; }

        public NAppUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Prepare(NAppUpdate.Framework.Sources.IUpdateSource source)
        {
            tempFile = null;
            if (!Attributes.ContainsKey("updateTo"))
                return true; // Errorneous case, but there's nothing to prepare...

            try
            {
                string tempFileLocal = Path.Combine(UpdateManager.Instance.TempFolder, Guid.NewGuid().ToString());
                if (!source.GetData(Attributes["updateTo"], UpdateManager.Instance.BaseUrl, ref tempFileLocal))
                    return false;

                tempFile = tempFileLocal;
            }
            catch { return false; }

            if (Attributes.ContainsKey("sha256-checksum"))
            {
                string checksum = Utils.FileChecksum.GetSHA256Checksum(tempFile);
                if (!checksum.Equals(Attributes["sha256-checksum"]))
                    return false;
            }

            return true;
        }

        public bool Execute()
        {
            if (!Attributes.ContainsKey("localPath"))
                return true;

            destinationFile = Path.Combine(Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath), Attributes["localPath"]);

            // Create a backup copy if target exists
            if (File.Exists(destinationFile))
                File.Copy(destinationFile, Path.Combine(UpdateManager.Instance.BackupFolder, Attributes["localPath"]));

            // Only enable execution if the apply attribute was set to hot-swap
            if (Attributes.ContainsKey("apply") && "hot-swap".Equals(Attributes["apply"]))
            {
                try
                {
                    if (File.Exists(destinationFile))
                        File.Delete(destinationFile);
                    File.Move(tempFile, destinationFile);
                }
                catch { return false; }
            }
            return true;
        }

        public bool Rollback()
        {
            if (string.IsNullOrEmpty(destinationFile))
                return true;

            // Copy the backup copy back to its original position
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
            File.Copy(Path.Combine(UpdateManager.Instance.BackupFolder, Attributes["localPath"]), destinationFile);

            return true;
        }

        #endregion
    }
}

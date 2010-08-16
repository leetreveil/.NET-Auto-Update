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

        internal byte[] fileBytes = null;
        private string destinationFile;

        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; set; }

        public NAppUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Prepare(NAppUpdate.Framework.Sources.IUpdateSource source)
        {
            if (!Attributes.ContainsKey("updateTo"))
                return true; // Errorneous case, but there's nothing to prepare...

            try
            {
                fileBytes = source.GetFile(Attributes["updateTo"], UpdateManager.Instance.BaseUrl);
            }
            catch { return false; }

            if (fileBytes == null || fileBytes.Length == 0)
                return false;

            if (Attributes.ContainsKey("sha256-checksum"))
            {
                string checksum = Utils.FileChecksum.GetSHA256Checksum(fileBytes);
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
                    using (FileStream fs = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileBytes, 0, fileBytes.Length);
                    }
                }
                catch
                {
                    return false;
                }
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

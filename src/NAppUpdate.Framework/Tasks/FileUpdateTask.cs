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
            if (!Attributes.ContainsKey("localPath"))
                return true; // Errorneous case, but there's nothing to prepare to...

            string fileName;
            if (Attributes.ContainsKey("updateTo"))
                fileName = Attributes["updateTo"];
            else
                fileName = Attributes["localPath"];

            tempFile = null;

            try
            {
                string tempFileLocal = Path.Combine(UpdateManager.Instance.TempFolder, Guid.NewGuid().ToString());
                if (!source.GetData(fileName, UpdateManager.Instance.BaseUrl, ref tempFileLocal))
                    return false;

                tempFile = tempFileLocal;
            }
            catch (Exception ex)
            {
                throw new UpdateProcessFailedException("Couldn't get Data from source", ex);
            }

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
                	tempFile = null;
                }
                catch (Exception ex)
                {
                    throw new UpdateProcessFailedException("Couldn't move hot-swap file into position", ex);
                }
            }
            return true;
        }

    	public IEnumerator<KeyValuePair<string, object>> GetColdUpdates()
    	{
			if (tempFile != null && Attributes.ContainsKey("localPath"))
				if (!Attributes.ContainsKey("apply") || (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
					yield return new KeyValuePair<string, object>(Attributes["localPath"], tempFile);
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

using System;
using System.IO;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.Tasks
{
	[Serializable]
    [UpdateTaskAlias("fileUpdate")]
    public class FileUpdateTask : IUpdateTask
    {
        public FileUpdateTask()
        {
			ExecutionStatus = TaskExecutionStatus.Pending;
        }

        [NauField("localPath", "The local path of the file to update", true)]
        public string LocalPath { get; set; }

        [NauField("updateTo",
            "File name on the remote location; same name as local path will be used if left blank"
            , false)]
        public string UpdateTo { get; set; }

        [NauField("sha256-checksum", "SHA-256 checksum to validate the file after download (optional)", false)]
        public string Sha256Checksum { get; set; }

        [NauField("hotswap",
            "Default update action is a cold update; check here if a hot file swap should be attempted"
            , false)]
        public bool CanHotSwap { get; set; }

        internal string tempFile;
        private string destinationFile, backupFile;

        #region IUpdateTask Members

        public string Description { get; set; }
		public TaskExecutionStatus ExecutionStatus { get; set; }
		
		[NonSerialized]
		private BooleanCondition _updateConditions;
		public BooleanCondition UpdateConditions
		{
			get { return _updateConditions ?? (_updateConditions = new BooleanCondition()); }
			set { _updateConditions = value; }
		}

		[field:NonSerialized]
    	public event ReportProgressDelegate OnProgress;

    	public bool Prepare(Sources.IUpdateSource source)
        {
            if (string.IsNullOrEmpty(LocalPath))
                return true; // Errorneous case, but there's nothing to prepare to...

            string fileName;
            if (!string.IsNullOrEmpty(UpdateTo))
                fileName = UpdateTo;
            else
                fileName = LocalPath;

            tempFile = null;

            try
            {
                string tempFileLocal = Path.Combine(UpdateManager.Instance.Config.TempFolder, Guid.NewGuid().ToString());
                if (!source.GetData(fileName, UpdateManager.Instance.BaseUrl, 
					p => OnProgress(p),
					ref tempFileLocal))
                    return false;

                tempFile = tempFileLocal;
            }
            catch (Exception ex)
            {
                throw new UpdateProcessFailedException("Couldn't get Data from source", ex);
            }

            if (!string.IsNullOrEmpty(Sha256Checksum))
            {
                string checksum = Utils.FileChecksum.GetSHA256Checksum(tempFile);
                if (!checksum.Equals(Sha256Checksum))
                    return false;
            }

			return tempFile != null;
        }

		public TaskExecutionStatus Execute(bool coldRun)
		{
			if (string.IsNullOrEmpty(LocalPath))
				return TaskExecutionStatus.Successful;

			destinationFile = Path.Combine(Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath), LocalPath);
			if (!Directory.Exists(Path.GetDirectoryName(destinationFile)))
				Utils.FileSystem.CreateDirectoryStructure(Path.GetDirectoryName(destinationFile), false);

			// Create a backup copy if target exists
			if (backupFile == null && File.Exists(destinationFile))
			{
				if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(UpdateManager.Instance.Config.BackupFolder, LocalPath))))
					Utils.FileSystem.CreateDirectoryStructure(
						Path.GetDirectoryName(Path.Combine(UpdateManager.Instance.Config.BackupFolder, LocalPath)), false);
				backupFile = Path.Combine(UpdateManager.Instance.Config.BackupFolder, LocalPath);
				File.Copy(destinationFile, backupFile, true);
			}

			// Only allow execution if the apply attribute was set to hot-swap, or if this is a cold run
			if (CanHotSwap || coldRun)
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
					if (coldRun)
					{
						// TODO: log error
						return TaskExecutionStatus.Failed;
					}

					// Failed hot swap file tasks should now downgrade to cold tasks automatically
					CanHotSwap = false;
				}
			}

			if (coldRun || CanHotSwap)
				// If we got thus far, we have completed execution
				return TaskExecutionStatus.Successful;

			// Otherwise, figure out what restart method to use
			if (File.Exists(destinationFile) && !Utils.PermissionsCheck.HaveWritePermissionsForFileOrFolder(destinationFile))
			{
				return TaskExecutionStatus.RequiresPrivilegedAppRestart;
			}
			return TaskExecutionStatus.RequiresAppRestart;
		}

    	public bool Rollback()
        {
            if (string.IsNullOrEmpty(destinationFile))
                return true;

            // Copy the backup copy back to its original position
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
			File.Copy(backupFile, destinationFile, true);

            return true;
        }

    	#endregion
    }
}

using System;
using System.IO;

namespace NAppUpdate.Framework.Common
{
	[Serializable]
	public class NauConfigurations
	{
		public string TempFolder { get; set; }

		/// <summary>
		/// Path to the backup folder used by the update process
		/// </summary>
		public string BackupFolder
		{
			set
			{
				if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.NotChecked
					|| UpdateManager.Instance.State == UpdateManager.UpdateProcessState.Checked)
				{
					string path = value.TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					_backupFolder = Path.IsPathRooted(path) ? path : Path.Combine(TempFolder, path);
				}
				else
					throw new ArgumentException("BackupFolder can only be specified before update has started");
			}
			get
			{
				return _backupFolder;
			}
		}
		internal string _backupFolder;

		public string UpdateProcessName { get; set; }

		/// <summary>
		/// The name for the executable file to extract and run cold updates with. Default is foo.exe. You can change
		/// it to whatever you want, but pay attention to names like "updater.exe" and "installer.exe" - they will trigger
		/// an UAC prompt in all cases.
		/// </summary>
		public string UpdateExecutableName { get; set; }
	}
}

using System;
using System.IO;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Conditions
{
	public class FileDateCondition : IUpdateCondition
	{
		public FileDateCondition()
		{
			Timestamp = DateTime.MinValue;
		}

		[NauField("localPath",
			"The local path of the file to check. If not set but set under a FileUpdateTask, the LocalPath of the task will be used. Otherwise this condition will be ignored."
			, false)]
		public string LocalPath { get; set; }

		[NauField("timestamp", "Date-time to compare with", true)]
		public DateTime Timestamp { get; set; }

		[NauField("what", "Comparison action to perform. Accepted values: newer, is, older. Default: older.", false)]
		public string ComparisonType { get; set; }

		#region IUpdateCondition Members

		public bool IsMet(Tasks.IUpdateTask task)
		{
			if (Timestamp == DateTime.MinValue)
				return true;

			string localPath = !string.IsNullOrEmpty(LocalPath)
								   ? LocalPath
								   : Utils.Reflection.GetNauAttribute(task, "LocalPath") as string;
			if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
				return true;

			// File timestamps seem to be off by a little bit (conversion rounding?), so the code below
			// gets around that
			DateTime dt = File.GetLastWriteTime(localPath);
			long localPlus = dt.AddSeconds(2).ToFileTimeUtc();
			long localMinus = dt.AddSeconds(-2).ToFileTimeUtc();
			long remoteFileDateTime = Timestamp.ToFileTimeUtc();

			bool result;
			switch (ComparisonType)
			{
				case "newer":
					result = localMinus > remoteFileDateTime;
					break;
				case "is":
					result = localMinus <= remoteFileDateTime && remoteFileDateTime <= localPlus;
					break;
				default:
					result = localPlus < remoteFileDateTime;
					break;
			}
			return result;
		}

		#endregion
	}
}

namespace NAppUpdate.Framework.Common
{
	public delegate void ReportProgressDelegate(UpdateProgressInfo currentStatus);

	public class UpdateProgressInfo
	{
		public int TaskId { get; set; }
		public string TaskDescription { get; set; }

		public string Message { get; set; }
		public int Percentage { get; set; }
		public bool StillWorking { get; set; }
	}

	public class DownloadProgressInfo : UpdateProgressInfo
	{
		public long FileSizeInBytes { get; set; }
		public long DownloadedInBytes { get; set; }
	}
}

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

            long localPlus = File.GetLastWriteTime(localPath).AddSeconds(2).ToFileTimeUtc();
            long localMinus = File.GetLastWriteTime(localPath).AddSeconds(-2).ToFileTimeUtc();
            long localFileDateTime = File.GetLastWriteTime(localPath).ToFileTimeUtc();
            long remoteFileDateTime = Timestamp.ToFileTimeUtc();

            // File timestamps seem to be off by a little bit (conversion rounding?)
            // Does DST need to be taken into account?
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
            //if (result)
            //    MessageBox.Show(string.Format("{0}\n" +
            //        "localFileDateTime:     {1} ({2})\n" +
            //        "remoteFileDateTime: {3} ({4})\n" +
            //        "result: {5} = {6}",
            //        localPath,
            //        localFileDateTime, DateTime.FromFileTimeUtc(localFileDateTime),
            //        remoteFileDateTime, DateTime.FromFileTimeUtc(remoteFileDateTime),
            //        ComparisonType, result));
            return result;
        }

        #endregion
    }
}

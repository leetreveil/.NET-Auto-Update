using System;
using System.IO;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Conditions
{
	[Serializable]
    public class FileSizeCondition : IUpdateCondition
    {
        [NauField("localPath",
            "The local path of the file to check. If not set but set under a FileUpdateTask, the LocalPath of the task will be used. Otherwise this condition will be ignored."
            , false)]
        public string LocalPath { get; set; }

        [NauField("size", "File size to compare with (in bytes)", true)]
        public long FileSize { get; set; }

        [NauField("what", "Comparison action to perform. Accepted values: above, is, below. Default: below.", false)]
        public string ComparisonType { get; set; }

        #region IUpdateCondition Members

        public bool IsMet(Tasks.IUpdateTask task)
        {
            if (FileSize <= 0)
                return true;

            string localPath = !string.IsNullOrEmpty(LocalPath)
                                   ? LocalPath
                                   : Utils.Reflection.GetNauAttribute(task, "LocalPath") as string;
            if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                return true;

            FileInfo fi = new FileInfo(localPath);
            switch (ComparisonType)
            {
                case "above":
                    return FileSize < fi.Length;
                case "is":
                    return FileSize == fi.Length;
            }
            return FileSize > fi.Length;
        }

        #endregion
    }
}

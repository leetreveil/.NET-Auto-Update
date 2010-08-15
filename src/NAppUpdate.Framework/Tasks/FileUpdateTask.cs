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

        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; set; }

        public NAppUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Prepare(NAppUpdate.Framework.Sources.IUpdateSource source)
        {
            if (!Attributes.ContainsKey("updateTo"))
                return false;

            try
            {
                fileBytes = source.GetFile(Attributes["updateTo"]);
            }
            catch { return false; }

            if (fileBytes == null || fileBytes.Length == 0)
                return false;

            return true;
        }

        public bool Execute()
        {
            if (!Attributes.ContainsKey("localPath"))
                return true;

            // Only enable execution if the apply attribute was set to hot-swap
            if (Attributes.ContainsKey("apply") && "hot-swap".Equals(Attributes["apply"]))
            {
                try
                {
                    string appDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    using (FileStream fs = new FileStream(Path.Combine(appDirectory, Attributes["localPath"]), FileMode.Create, FileAccess.Write))
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

        #endregion
    }
}

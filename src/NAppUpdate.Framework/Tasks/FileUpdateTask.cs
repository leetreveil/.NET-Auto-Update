using System;
using System.Collections.Generic;
using System.Text;

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

        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; set; }

        public NAppUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Prepare(NAppUpdate.Framework.Sources.IUpdateSource source)
        {
            if (!Attributes.ContainsKey("updateTo"))
                return false;

            byte[] newFileData;
            try
            {
                newFileData = source.GetFile(Attributes["updateTo"]);
            }
            catch { return false; }

            if (newFileData == null || newFileData.Length == 0)
                return false;

            return true;
        }

        public bool Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

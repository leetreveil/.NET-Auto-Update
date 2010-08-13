using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAppUpdate.Framework.Tasks
{
    [UpdateTaskAlias("fileUpdate")]
    public class FileUpdateTask : IUpdateTask
    {
        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; internal set; }

        public NAppUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

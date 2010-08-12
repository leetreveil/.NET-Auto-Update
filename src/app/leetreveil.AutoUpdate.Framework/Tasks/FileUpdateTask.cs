using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace leetreveil.AutoUpdate.Framework.Tasks
{
    [UpdateTaskAlias("fileUpdate")]
    public class FileUpdateTask : IUpdateTask
    {
        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; internal set; }

        public leetreveil.AutoUpdate.Framework.Conditions.BooleanCondition UpdateConditions { get; set; }

        public bool Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

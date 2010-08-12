using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace leetreveil.AutoUpdate.Framework.Conditions
{
    [UpdateConditionAlias("exists")]
    public class FileExistsCondition : IUpdateCondition
    {
        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsFulfilled()
        {
            string localPath = Attributes["localPath"];
            return File.Exists(localPath);
        }

        #endregion
    }
}

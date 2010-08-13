using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace NAppUpdate.Framework.Conditions
{
    public class FileVersionCondition : IUpdateCondition
    {
        public FileVersionCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsFulfilled()
        {
            // TODO
            FileVersionInfo version = FileVersionInfo.GetVersionInfo("path");
            return new Version(Attributes["version"]) > new Version(version.FileVersion);
        }

        #endregion
    }
}

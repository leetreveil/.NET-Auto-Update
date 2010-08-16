using System;
using System.Collections.Generic;
using System.Text;

using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.Tasks
{
    public interface IUpdateTask
    {
        IDictionary<string, string> Attributes { get; }
        string Description { get; set; }
        BooleanCondition UpdateConditions { get; set; }

        bool Prepare(NAppUpdate.Framework.Sources.IUpdateSource source);
        bool Execute();
        bool Rollback();
    }
}

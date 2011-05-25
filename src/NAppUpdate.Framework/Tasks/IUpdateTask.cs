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

        bool Prepare(Sources.IUpdateSource source);
        bool Execute();
    	IEnumerator<KeyValuePair<string, object>> GetColdUpdates();
        bool Rollback();
    }
}

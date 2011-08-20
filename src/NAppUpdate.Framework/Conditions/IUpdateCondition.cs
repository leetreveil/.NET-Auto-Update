using System.Collections.Generic;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Conditions
{
    public interface IUpdateCondition : INauFieldsHolder
    {
        IDictionary<string, string> Attributes { get; }
        bool IsMet(Tasks.IUpdateTask task);
    }
}

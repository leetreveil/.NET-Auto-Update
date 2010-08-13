using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Conditions
{
    public interface IUpdateCondition
    {
        IDictionary<string, string> Attributes { get; }
        bool IsFulfilled();
    }
}

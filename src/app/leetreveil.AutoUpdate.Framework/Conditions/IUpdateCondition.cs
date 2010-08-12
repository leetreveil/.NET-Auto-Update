using System;
using System.Collections.Generic;
using System.Text;

namespace leetreveil.AutoUpdate.Framework.Conditions
{
    public interface IUpdateCondition
    {
        IDictionary<string, string> Attributes { get; }
        bool IsFulfilled();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Conditions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UpdateConditionAliasAttribute : Attribute
    {
        private readonly string _alias;

        public UpdateConditionAliasAttribute(string alias)
        {
            this._alias = alias;
        }

        public string Alias
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this._alias; }
        }
    }
}

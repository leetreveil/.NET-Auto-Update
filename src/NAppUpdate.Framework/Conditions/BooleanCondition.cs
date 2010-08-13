using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAppUpdate.Framework.Conditions
{
    public sealed class BooleanCondition : IUpdateCondition
    {
        #region Condition types

        [Flags]
        public enum ConditionType : byte
        {
            AND = 1,
            OR = 2,
            NOT = 3,
        }

        public static ConditionType ConditionTypeFromString(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "and":
                        return ConditionType.AND;
                    case "or":
                        return ConditionType.OR;
                    case "not":
                    case "and-not":
                        return ConditionType.AND | ConditionType.NOT;
                    case "or-not":
                        return ConditionType.OR | ConditionType.NOT;
                }
            }

            // Make AND the default condition type
            return ConditionType.AND;
        }
        #endregion

        private class ConditionItem
        {
            public ConditionItem(IUpdateCondition cnd, ConditionType typ)
            {
                this._Condition = cnd;
                this._ConditionType = typ;
            }

            public IUpdateCondition _Condition;
            public ConditionType _ConditionType;
        }

        private LinkedList<ConditionItem> ChildConditions { get; set; }
        public int ChildConditionsCount { get { return ChildConditions.Count; } }

        public void AddCondition(IUpdateCondition cnd)
        {
            AddCondition(cnd, ConditionType.AND);
        }

        public void AddCondition(IUpdateCondition cnd, ConditionType type)
        {
            ChildConditions.AddLast(new ConditionItem(cnd, type));
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsFulfilled()
        {
            bool Passed = true, firstRun = true;
            foreach (ConditionItem item in ChildConditions)
            {
                // If after the first iteration, accept as fulfilled if we are at an OR clause and the conditions
                // before this checked OK
                if (!firstRun)
                {
                    if (Passed && (item._ConditionType & ConditionType.OR) > 0)
                        return true;
                }
                else { firstRun = false; }

                // Skip all ANDed conditions if some of them failed, until we consume all the conditions
                // or we hit an OR'ed one
                if (!Passed)
                {
                    if ((item._ConditionType & ConditionType.OR) > 0)
                    {
                        bool checkResult = item._Condition.IsFulfilled();
                        Passed = (item._ConditionType & ConditionType.NOT) > 0 ? checkResult : !checkResult;
                    }
                }
                else
                {
                    bool checkResult = item._Condition.IsFulfilled();
                    Passed = (item._ConditionType & ConditionType.NOT) > 0 ? checkResult : !checkResult;
                }
            }

            return Passed;
        }

        #endregion
    }
}

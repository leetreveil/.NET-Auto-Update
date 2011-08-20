using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Conditions
{
    public interface IUpdateCondition : INauFieldsHolder
    {
        bool IsMet(Tasks.IUpdateTask task);
    }
}

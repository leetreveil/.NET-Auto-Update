using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Utils
{
    using NAppUpdate.Framework.Tasks;
    using NAppUpdate.Framework.Conditions;

    public static class Reflection
    {
        public static void FindTasksAndConditionsInAssembly(System.Reflection.Assembly assembly,
            Dictionary<string, Type> updateTasks, Dictionary<string, Type> updateConditions)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (typeof(IUpdateTask).IsAssignableFrom(t))
                {
                    updateTasks.Add(t.Name, t);
                    UpdateTaskAliasAttribute[] tasksAliases = (UpdateTaskAliasAttribute[])t.GetCustomAttributes(typeof(UpdateTaskAliasAttribute), false);
                    foreach (UpdateTaskAliasAttribute alias in tasksAliases)
                    {
                        updateTasks.Add(alias.Alias, t);
                    }
                }
                else if (typeof(IUpdateCondition).IsAssignableFrom(t))
                {
                    updateConditions.Add(t.Name, t);
                    UpdateConditionAliasAttribute[] tasksAliases = (UpdateConditionAliasAttribute[])t.GetCustomAttributes(typeof(UpdateConditionAliasAttribute), false);
                    foreach (UpdateConditionAliasAttribute alias in tasksAliases)
                    {
                        updateConditions.Add(alias.Alias, t);
                    }
                }
            }
        }
    }
}

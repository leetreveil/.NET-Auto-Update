using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NAppUpdate.Framework.Utils
{
    using NAppUpdate.Framework.Tasks;
    using NAppUpdate.Framework.Conditions;
    using NAppUpdate.Framework.Common;

    public static class Reflection
    {
        internal static void FindTasksAndConditionsInAssembly(System.Reflection.Assembly assembly,
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

        internal static void SetTaskAttribute(IUpdateTask task, Dictionary<string, string> attributes)
        {
            // Load public non-static properties
            PropertyInfo[] propertyInfos = typeof(IUpdateTask).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            if (propertyInfos != null)
            {
                string attValue = string.Empty;
                foreach (PropertyInfo pi in propertyInfos)
                {
                    object[] atts = pi.GetCustomAttributes(typeof(NauFieldAttribute), false);
                    if (atts == null) continue;

                    NauFieldAttribute nfa = (NauFieldAttribute)atts[0]; // NauFieldAttribute doesn't allow multiples

                    // Get the attribute value, process it, and set the object's property with that value
                    if (attributes.TryGetValue(nfa.Alias, out attValue))
                    {
                        if (pi.PropertyType == typeof(String))
                        {
                            pi.SetValue(task, attValue, null);
                        }
                        else if (pi.PropertyType.IsEnum)
                        {
                            object eObj = Enum.Parse(pi.PropertyType, attValue);
                            if (eObj != null)
                                pi.SetValue(task, eObj, null);
                        }
                        else
                        {
                            MethodInfo mi = pi.PropertyType.GetMethod("Parse", new Type[] { typeof(String) });
                            if (mi == null) continue;
                            object o = mi.Invoke(null, new object[] { attValue });
                        }
                    }
                }
            }
        }


    }
}

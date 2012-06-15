using System;
using NAppUpdate.Framework.Common;
using Microsoft.Win32;
using NAppUpdate.Framework.Conditions;

namespace NAppUpdate.Framework.Tasks
{
	[Serializable]
    [UpdateTaskAlias("registryUpdate")]
    public class RegistryTask : IUpdateTask
    {
        public RegistryTask()
        {
			ExecutionStatus = TaskExecutionStatus.Pending;
        }

        [NauField("keyName", "The full path to the registry key", true)]
        public string KeyName { get; set; }

        [NauField("keyValue", "The value name to set", true)]
        public string KeyValueName { get; set; }

        [NauField("valueKind",
            "Value type; choose one and then set only one value field (leave all blank to remove the key)"
            , true)]
        public RegistryValueKind ValueKind { get; set; }

        [NauField("value", "A String value to set", false)]
        public string StringValue { get; set; }

        [NauField("value", "A DWord value to set", false)]
        public Int32? DWordValue { get; set; }

        [NauField("value", "A QWord value to set", false)]
        public Int64? QWordValue { get; set; }

        // Get the first non-null value
        protected object ValueToSet
        {
            get
            {
                if (StringValue != null)
                    return StringValue;
                if (DWordValue != null)
                    return DWordValue;
                if (QWordValue != null)
                    return QWordValue;
                return null;
            }
        }
        private object originalValue;

        #region IUpdateTask Members

        public string Description { get; set; }
		public TaskExecutionStatus ExecutionStatus { get; set; }

		[NonSerialized]
		private BooleanCondition _updateConditions;
		public BooleanCondition UpdateConditions
		{
			get { return _updateConditions ?? (_updateConditions = new BooleanCondition()); }
			set { _updateConditions = value; }
		}

    	public event ReportProgressDelegate OnProgress;

    	public bool Prepare(Sources.IUpdateSource source)
        {
            // No preparation required
            return true;
        }

		public TaskExecutionStatus Execute(bool coldRun /* unused */)
        {
            if (String.IsNullOrEmpty(KeyName) || String.IsNullOrEmpty(KeyValueName))
				return ExecutionStatus = TaskExecutionStatus.Successful;

            try
            {
                // Get the current value and store in case we need to rollback
                // This is also used to prematurely detect incorrect key and value paths
                originalValue = Registry.GetValue(KeyName, KeyValueName, null);
            }
            catch { return ExecutionStatus = TaskExecutionStatus.Failed; }

            try
            {
                Registry.SetValue(KeyName, KeyValueName, ValueToSet, ValueKind);
            }
			catch { return ExecutionStatus = TaskExecutionStatus.Failed; }

			return ExecutionStatus = TaskExecutionStatus.Successful;
        }

        public bool Rollback()
        {
            try
            {
                Registry.SetValue(KeyName, KeyValueName, originalValue);
            }
            catch { return false; }
            return true;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Conditions;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Framework.Tasks;

namespace WinFormsProgressSample
{
	public class LengthyTask : IUpdateTask
	{
		public LengthyTask()
		{
			UpdateConditions = new BooleanCondition();
			ExecutionStatus = TaskExecutionStatus.Pending;
		}

		public string Description{get; set; }

		public BooleanCondition UpdateConditions { get; set; }

		public TaskExecutionStatus ExecutionStatus { get; set; }

		public event ReportProgressDelegate OnProgress;

		public bool Prepare(IUpdateSource source)
		{
			for (int i = 0; i < 50; i++)
			{
				Thread.Sleep(100);
				OnProgress(new UpdateProgressInfo
				             	{
				             		Message = "Doing some work, cycle " + i,
				             		Percentage = i * 2,
				             		StillWorking = true
				             	});
			}

			OnProgress(new UpdateProgressInfo
			             	{
			             		Message = "Finished preperations",
			             		Percentage = 100,
			             		StillWorking = false,
			             	});

			return true;
		}

		public TaskExecutionStatus Execute(bool coldRun)
		{
			return coldRun ? TaskExecutionStatus.Successful : TaskExecutionStatus.RequiresAppRestart;
		}

		public bool Rollback()
		{
			return true;
		}
	}
}

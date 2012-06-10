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
		}

		public string Description{get; set; }

		public BooleanCondition UpdateConditions { get; set; }
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

		public bool Execute()
		{
			return true;
		}

		public IEnumerator<KeyValuePair<string, object>> GetColdUpdates()
		{
			yield break;
		}

		public bool MustRunPrivileged()
		{
			return false;
		}

		public bool Rollback()
		{
			return true;
		}
	}
}

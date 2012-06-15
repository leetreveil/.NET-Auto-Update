using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Tests.Core
{
	[TestClass]
	public class UpdateStarterTests
	{
		[TestMethod]
		public void UpdaterDeploymentWorks()
		{
			var path = Path.Combine(Path.GetTempPath(), "NAppUpdate-Tests");

			UpdateStarter.ExtractUpdaterFromResource(path, "Foo.exe");

			Assert.IsTrue(Directory.Exists(path));
			Assert.IsTrue(File.Exists(Path.Combine(path, "Foo.exe")));
			Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdate.Framework.dll")));

			// Cleanup test
			NAppUpdate.Framework.Utils.FileSystem.DeleteDirectory(path);
		}

		[TestMethod]
		public void UpdaterDeploymentAndIPCWorks()
		{
			var ud = new UpdateStarter(false);
			ud.SetOptions(true, true);

			var dto = new UpdateStarter.NauDto
			          	{
			          		Configs = UpdateManager.Instance.Config,
			          		Tasks = new List<IUpdateTask>
			          		        	{
											new FileUpdateTask {Description = "Task #1", ExecutionStatus = TaskExecutionStatus.RequiresAppRestart},
			          		        	},
			          		AppPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,
			          		WorkingDirectory = Environment.CurrentDirectory,
			          		RelaunchApplication = false,
			          	};

			var path = dto.Configs.TempFolder;

			if (Directory.Exists(path))
				NAppUpdate.Framework.Utils.FileSystem.DeleteDirectory(path);

			var p = ud.Start(dto, dto.Configs.TempFolder, "NAppUpdate-Tests");
			p.WaitForExit();

			Assert.IsTrue(Directory.Exists(path));
			Assert.IsTrue(File.Exists(Path.Combine(path, "Foo.exe")));
			Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdate.Framework.dll")));

			// Cleanup test
			NAppUpdate.Framework.Utils.FileSystem.DeleteDirectory(path);
		}
	}
}

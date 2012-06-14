using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAppUpdate.Framework;

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
	}
}

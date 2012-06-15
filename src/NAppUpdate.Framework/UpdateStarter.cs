using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

// Used for the named pipes implementation
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Framework
{
	/// <summary>
	/// Starts the cold update process by extracting the updater app from the library's resources,
	/// passing it all the data it needs and terminating the current application
	/// </summary>
	internal class UpdateStarter
	{
		[Serializable]
		internal class NauDto
		{
			public NauConfigurations Configs { get; set; }
			public IList<IUpdateTask> Tasks { get; set; }
			public string AppPath { get; set; }
			public string WorkingDirectory { get; set; }
			public bool RelaunchApplication { get; set; }
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateNamedPipe(
		   String pipeName,
		   uint dwOpenMode,
		   uint dwPipeMode,
		   uint nMaxInstances,
		   uint nOutBufferSize,
		   uint nInBufferSize,
		   uint nDefaultTimeOut,
		   IntPtr lpSecurityAttributes);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int ConnectNamedPipe(
		   SafeFileHandle hNamedPipe,
		   IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateFile(
		   String pipeName,
		   uint dwDesiredAccess,
		   uint dwShareMode,
		   IntPtr lpSecurityAttributes,
		   uint dwCreationDisposition,
		   uint dwFlagsAndAttributes,
		   IntPtr hTemplate);

		//private const uint DUPLEX = (0x00000003);
		private const uint WRITE_ONLY = (0x00000002);
		private const uint FILE_FLAG_OVERLAPPED = (0x40000000);

		const uint GENERIC_READ = (0x80000000);
		//static readonly uint GENERIC_WRITE = (0x40000000);
		const uint OPEN_EXISTING = 3;

		internal static string GetPipeName(string syncProcessName)
		{
			return string.Format("\\\\.\\pipe\\{0}", syncProcessName);
		}

		internal static uint BUFFER_SIZE = 4096;

		private readonly bool _runPrivileged;
		private bool _updaterDoLogging;
		private bool _updaterShowConsole;

		public UpdateStarter(bool runPrivileged)
		{
			_runPrivileged = runPrivileged;
		}

		public void SetOptions(bool updaterDoLogging, bool updaterShowConsole)
		{
			_updaterDoLogging = updaterDoLogging;
			_updaterShowConsole = updaterShowConsole;
		}

		public Process Start(NauDto dto, string updaterPath, string syncProcessName)
		{
			ExtractUpdaterFromResource(updaterPath, UpdateManager.Instance.Config.UpdateExecutableName);

			Process p = null;

			using (var clientPipeHandle = CreateNamedPipe(
				   GetPipeName(syncProcessName),
				   WRITE_ONLY | FILE_FLAG_OVERLAPPED,
				   0,
				   1, // 1 max instance (only the updater utility is expected to connect)
				   BUFFER_SIZE,
				   BUFFER_SIZE,
				   0,
				   IntPtr.Zero))
			{
				//failed to create named pipe
				if (clientPipeHandle.IsInvalid)
					return null;

				var info = new ProcessStartInfo
							{
								UseShellExecute = true,
								WorkingDirectory = Environment.CurrentDirectory,
								FileName = Path.Combine(updaterPath, UpdateManager.Instance.Config.UpdateExecutableName),
								Arguments = string.Format(@"""{0}"" {1} {2}", syncProcessName, _updaterShowConsole ? "-showConsole" : "", _updaterDoLogging ? "-log" : ""),
							};

				if (!_updaterShowConsole)
				{
					info.WindowStyle = ProcessWindowStyle.Hidden;
					info.CreateNoWindow = true;
				}

				// If we can't write to the destination folder, then lets try elevating priviledges.
				if (_runPrivileged || !Utils.PermissionsCheck.HaveWritePermissionsForFolder(Environment.CurrentDirectory)) { info.Verb = "runas"; }

				try
				{
					p = Process.Start(info);
				}
				catch (Win32Exception)
				{
					// Person denied UAC escallation
					return null;
				}

				while (true)
				{
					var success = 0;
					try
					{
						success = ConnectNamedPipe(
						   clientPipeHandle,
						   IntPtr.Zero);
					}
					catch { }

					//failed to connect client pipe
					if (success == 0)
						break;

					//client connection successfull
					using (var fStream = new FileStream(clientPipeHandle, FileAccess.Write, (int)BUFFER_SIZE, true))
					{
						new BinaryFormatter().Serialize(fStream, dto);
						fStream.Close();
					}
				}
			}

			return p;
		}

		internal static object ReadDto(string syncProcessName)
		{
			using (SafeFileHandle pipeHandle = CreateFile(
				GetPipeName(syncProcessName),
				GENERIC_READ,
				0,
				IntPtr.Zero,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED,
				IntPtr.Zero))
			{

				if (pipeHandle.IsInvalid)
					return null;

				using (var fStream = new FileStream(pipeHandle, FileAccess.Read, (int)BUFFER_SIZE, true))
				{
					return new BinaryFormatter().Deserialize(fStream);
				}
			}
		}

		internal static void ExtractUpdaterFromResource(string updaterPath, string hostExeName)
		{
			if (!Directory.Exists(updaterPath))
				Directory.CreateDirectory(updaterPath);

			//store the updater temporarily in the designated folder            
			using (var writer = new BinaryWriter(File.Open(Path.Combine(updaterPath, hostExeName), FileMode.Create)))
				writer.Write(Resources.updater);

			// Now copy the NAU DLL
			var assemblyLocation = typeof(UpdateStarter).Assembly.Location;
			File.Copy(assemblyLocation, Path.Combine(updaterPath, "NAppUpdate.Framework.dll"));

			// And also all other referenced DLLs (opt-in only)
			// TODO typeof(UpdateStarter).Assembly.GetReferencedAssemblies()
		}
	}
}
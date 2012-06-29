using System;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using NAppUpdate.Framework.Tasks;
using NAppUpdate.Framework.Utils;

namespace NAppUpdate.Updater
{
	internal static class AppStart
	{
		private static ArgumentsParser _args;
		private static Logger _logger;
		private static ConsoleForm _console;

		private static void Main()
		{
			//Debugger.Launch();
			string tempFolder = "";
			try
			{
				_args = ArgumentsParser.Get();
				string logFile = System.Reflection.Assembly.GetEntryAssembly().Location;
				logFile = Path.Combine(Path.GetDirectoryName(logFile), "logs");
				logFile = Path.Combine(logFile, "NauUpdate.log");
				_logger = new Logger(logFile);
				_args.ParseCommandLineArgs();
				if (_args.ShowConsole)
				{
					_console = new ConsoleForm();
					_console.Show();
				}

				Log("Starting...");
				if (_args.Log)
					_console.WriteLine("Logging to {0}", logFile);

				// Get the update process name, to be used to create a named pipe and to wait on the application
				// to quit
				string syncProcessName = _args.ProcessName;
				if (string.IsNullOrEmpty(syncProcessName))
					//Application.Exit();
					throw new ArgumentException("The command line needs to specify the mutex of the program to update.", "args");

				Log("Update process name: '{0}'", syncProcessName);

				// Connect to the named pipe and retrieve the updates list
				var dto = NauIpc.ReadDto(syncProcessName) as NauIpc.NauDto;

				// Make sure we start updating only once the application has completely terminated
				bool createdNew;
				using (var mutex = new Mutex(false, syncProcessName + "Mutex", out createdNew))
				{
					try
					{
						if (!createdNew) mutex.WaitOne();
					}
					catch (AbandonedMutexException)
					{
						// An abandoned mutex is exactly what we are expecting...
						Log("The application has terminated (as expected).");
					}
				}

				bool updateSuccessful = true;

				if (dto == null || dto.Configs == null)
				{
					throw new Exception("Invalid DTO received");
				}

				Log("Got {0} task objects", dto.Tasks.Count);

				// Get some required environment variables
				string appPath = dto.AppPath;
				string appDir = dto.WorkingDirectory ?? Path.GetDirectoryName(appPath) ?? string.Empty;
				tempFolder = dto.Configs.TempFolder;
				string backupFolder = dto.Configs.BackupFolder;
				bool relaunchApp = dto.RelaunchApplication;

				if (dto.Tasks == null || dto.Tasks.Count == 0)
				{
					throw new Exception("Could not find the updates list (or it was empty).");
				}

				// Perform the actual off-line update process
				Log("Starting the updates...");

				foreach (var t in dto.Tasks)
				{
					Log("Task \"{0}\": {1}", t.Description, t.ExecutionStatus);

					if (t.ExecutionStatus != TaskExecutionStatus.RequiresAppRestart
						&& t.ExecutionStatus != TaskExecutionStatus.RequiresPrivilegedAppRestart)
						continue;

					Log("\tExecuting...");

					try
					{
						t.ExecutionStatus = t.Execute(true);
					}
					catch (Exception ex)
					{
						// TODO: Log message
						Log("\tFailed: {0}", ex.Message);
						updateSuccessful = false;
						t.ExecutionStatus = TaskExecutionStatus.Failed;
						MessageBox.Show("Update failed: " + ex.Message);
					}

					if (t.ExecutionStatus != TaskExecutionStatus.Successful)
					{
						Log("\tTask execution failed failed");
						updateSuccessful = false;
						break;
					}
				}

				if (updateSuccessful)
				{
					Log("Finished");
					Log("Removing backup folder");
					if (Directory.Exists(backupFolder))
						FileSystem.DeleteDirectory(backupFolder);
				}
				else
				{
					MessageBox.Show("Update Failed.");
					Log("Update failed.");
				}

				// Start the application only if requested to do so
				if (relaunchApp)
				{
					try
					{
						Log("Re-launching process {0} with working dir {1}", appPath, appDir);

						var info = new ProcessStartInfo
						           	{
						           		UseShellExecute = true,
						           		WorkingDirectory = appDir,
						           		FileName = appPath,
						           	};

						var p = NauIpc.LaunchProcessAndSendDto(dto, info, syncProcessName);
						if (p == null)
						{
							Log("Unable to relaunch application");
						}
					}
					catch (Win32Exception e)
					{
						MessageBox.Show(e.ToString());
						Log("Update failed: " + e);
					}
				}

				Log("All done.");
				//Application.Exit();
			}
			catch (Exception ex)
			{
				//supressing catch because if at any point we get an error the update has failed
				Log("*********************************");
				Log("   An error has occurred:");
				Log("   " + ex.Message);
				Log("*********************************");
				if (_args.ShowConsole)
				{
					_console.WriteLine();
					_console.WriteLine("The updater will close when you close this window.");
				}
			}
			finally
			{
				if (_args.ShowConsole)
				{
					_console.WriteLine();
					_console.WriteLine("Press any key or close this window to exit.");
					_console.ReadKey();
				}
				if (!string.IsNullOrEmpty(tempFolder)) SelfCleanUp(tempFolder);
				Application.Exit();
			}
		}

		private static void SelfCleanUp(string tempFolder)
		{
			try
			{
				// Delete the updater EXE and the temp folder)
				Log("Removing updater and temp folder... {0}", tempFolder);
				try
				{
					var Info = new ProcessStartInfo();
					//Application.ExecutablePath
					Info.Arguments = string.Format(@"/C ping 1.1.1.1 -n 1 -w 3000 > Nul & echo Y|del ""{0}\*.*"" & rmdir ""{0}"""
								   , tempFolder);
					Info.WindowStyle = ProcessWindowStyle.Hidden;
					Info.CreateNoWindow = true;
					Info.FileName = "cmd.exe";
					Process.Start(Info);
				}
				catch { /* ignore exceptions thrown while trying to clean up */ }
			}
			catch
			{
			}
		}

		private static void Log(string message, params object[] args)
		{
			message = string.Format(message, args);
			if (_args.Log)
				_logger.Log(message);
			if (_args.ShowConsole)
				_console.WriteLine(message);
			Application.DoEvents();
		}
	}
}
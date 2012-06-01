using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.Serialization.Formatters.Binary;
using NAppUpdate.Updater.Actions;

namespace NAppUpdate.Updater
{
	internal static class AppStart
	{
		const uint GENERIC_READ = (0x80000000);
		//static readonly uint GENERIC_WRITE = (0x40000000);
		const uint OPEN_EXISTING = 3;
		const uint FILE_FLAG_OVERLAPPED = (0x40000000);
		const int BUFFER_SIZE = 4096;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFile(
		   String pipeName,
		   uint dwDesiredAccess,
		   uint dwShareMode,
		   IntPtr lpSecurityAttributes,
		   uint dwCreationDisposition,
		   uint dwFlagsAndAttributes,
		   IntPtr hTemplate);

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
				Log("==========================================");
				Log("Starting...");
				if (_args.Log)
					_console.WriteLine("Logging to {0}", logFile);

				// Get the update process name, to be used to create a named pipe and to wait on the application
				// to quit
				string syncProcessName = _args.ProcessName;


				if (string.IsNullOrEmpty(syncProcessName))
					//Application.Exit();
					throw new ArgumentException("The command line needs to specify the mutex of the program to update.", "args");
				//Log("The command line needs to specify the mutex of the program to update.");
				Log("Object to update: '{0}'", syncProcessName);

				// Connect to the named pipe and retrieve the updates list
				var PIPE_NAME = string.Format("\\\\.\\pipe\\{0}", syncProcessName);
				var o = GetUpdates(PIPE_NAME);

				Log("Connecting to updater pipe: " + PIPE_NAME);
				Log(o != null ? "Connected to updater pipe." : "Failed to read updates from the updater pipe.");
				Log("Waiting for application to terminate.");

				// Make sure we start updating only once the application has completely terminated
				bool createdNew;
				using (var mutex = new Mutex(false, syncProcessName, out createdNew))
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

				bool relaunchApp = true, updateSuccessful = true;
				string appPath, appDir, backupFolder;
				{
					Dictionary<string, object> dict = null;
					if (o is Dictionary<string, object>)
						dict = o as Dictionary<string, object>;

					if (dict == null || dict.Count == 0)
					{
						throw new Exception("Could not find the updates list (or it was empty).");
						//Application.Exit();
						//return;
					}

					// Get some required environment variables
					appPath = dict["ENV:AppPath"].ToString();
					appDir = dict["ENV:WorkingDirectory"] as string ?? Path.GetDirectoryName(appPath);
					tempFolder = dict["ENV:TempFolder"].ToString();
					backupFolder = dict["ENV:BackupFolder"].ToString();
					relaunchApp = dict["ENV:RelaunchApplication"] as bool? ?? true;

					// Perform the actual off-line update process
					Log("Starting the updates...");
					var en = dict.GetEnumerator();
					while (en.MoveNext())
					{
						if (en.Current.Key.StartsWith("ENV:"))
							continue;
						else
						{
							Log("* Updating {0} ({1})", en.Current.Key, en.Current.Value);
							IUpdateAction a = null;
							if (en.Current.Value is string)
							{
								Log("\tCopying {0} {1}", en.Current.Value, Path.Combine(appDir, en.Current.Key));
								a = new FileCopyAction(en.Current.Value.ToString(), Path.Combine(appDir, en.Current.Key));
							}
							else if (en.Current.Value is byte[])
							{
								Log("\tDumping {0}", en.Current.Value);
								a = new FileDumpAction(Path.Combine(appDir, en.Current.Key), (byte[])en.Current.Value);
							}

							if (a == null)
								Log("\tUpdate action: null");
							else
							{
								Log("\tUpdate action: {0}", a.ToString());
								try
								{
									if (!a.Do())
									{
										Log("\tUpdate action failed: {0}", en.Current.Value);
										updateSuccessful = false;
										break;
									}
									else Log("\tUpdate action succeeded: {0}", en.Current.Value);
								}
								catch (Exception e)
								{
									MessageBox.Show("Update failed: " + e.Message);
									Log("\tUpdate failed: {0}", e.Message);
									updateSuccessful = false;
									break;
								}
							}
						}
					}
				}

				if (updateSuccessful)
				{
					if (Directory.Exists(backupFolder))
						Directory.Delete(backupFolder, true);
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
						Log("Relaunching the application...");
						Process.Start(new ProcessStartInfo
										{
											UseShellExecute = true,
											WorkingDirectory = appDir,
											FileName = appPath,
										});
					}
					catch (Win32Exception e)
					{
						MessageBox.Show(e.ToString());
						Log("Update failed: " + e);
					}
				}

				//MessageBox.Show(string.Format("Re-launched process {0} with working dir {1}", appPath, appDir));

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
				CleanUp(tempFolder);
				Application.Exit();
			}
		}

		private static void CleanUp(string tempFolder)
		{
			try
			{
				// Delete the updater EXE and the temp folder)
				Log("Removing updater and temp folder...");
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

		private static object GetUpdates(string PIPE_NAME)
		{
			using (SafeFileHandle pipeHandle = CreateFile(
				PIPE_NAME,
				GENERIC_READ,
				0,
				IntPtr.Zero,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED,
				IntPtr.Zero))
			{

				if (pipeHandle.IsInvalid)
					return null;

				using (var fStream = new FileStream(pipeHandle, FileAccess.Read, BUFFER_SIZE, true))
				{
					return new BinaryFormatter().Deserialize(fStream);
				}
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
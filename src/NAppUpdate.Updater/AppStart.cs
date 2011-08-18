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

        private static void Main()
        {
            //Debugger.Launch();
            try
            {
                // Get the update process name, to be used to create a named pipe and to wait on the application
                // to quit
                string[] args = Environment.GetCommandLineArgs();
                string syncProcessName = args[1];

                if (string.IsNullOrEmpty(syncProcessName))
                    Application.Exit();

                // Connect to the named pipe and retrieve the updates list
                var PIPE_NAME = string.Format("\\\\.\\pipe\\{0}", syncProcessName);
                var o = GetUpdates(PIPE_NAME);

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
                    }
                }

                string appPath, appDir, tempFolder, backupFolder;
                bool relaunchApp = true, updateSuccessful = true;
                {
                    Dictionary<string, object> dict = null;
                    if (o is Dictionary<string, object>)
                        dict = o as Dictionary<string, object>;

					if (dict == null || dict.Count == 0)
					{
						Application.Exit();
						return;
					}

                	// Get some required environment variables
                    appPath = dict["ENV:AppPath"].ToString();
					appDir = dict["ENV:WorkingDirectory"] as string ?? Path.GetDirectoryName(appPath);
                    tempFolder = dict["ENV:TempFolder"].ToString();
                    backupFolder = dict["ENV:BackupFolder"].ToString();
                    relaunchApp = dict["ENV:RelaunchApplication"] as bool? ?? true;

                    // Perform the actual off-line update process
                    var en = dict.GetEnumerator();
                    while (en.MoveNext())
                    {
                        if (en.Current.Key.StartsWith("ENV:"))
                            continue;
                        else
                        {
                            IUpdateAction a = null;
                            if (en.Current.Value is string)
                                a = new FileCopyAction(en.Current.Value.ToString(), Path.Combine(appDir, en.Current.Key));
                            else if (en.Current.Value is byte[])
                                a = new FileDumpAction(Path.Combine(appDir, en.Current.Key), (byte[])en.Current.Value);

                            if (a != null)
                            {
                                try
                                {
                                    if (!a.Do())
                                    {
                                        updateSuccessful = false;
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("Update failed: " + e.Message);
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
                }

                // Start the application only if requested to do so
				if (relaunchApp)
				{
					try
					{
						Process.Start(new ProcessStartInfo
						              	{
						              		UseShellExecute = true,
						              		WorkingDirectory = appDir,
						              		FileName = appPath,
						              	});
					} catch(Win32Exception e) {
						MessageBox.Show(e.ToString());
					}
				}

            	//MessageBox.Show(string.Format("Re-launched process {0} with working dir {1}", appPath, appDir));

            	// Delete the updater EXE and the temp folder)
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

                Application.Exit();
            }
            catch
            {
                //supressing catch because if at any point we get an error the update has failed
            }
            finally
            {
                Application.Exit();
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
    }
}
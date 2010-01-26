using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Threading;

namespace leetreveil.AutoUpdate.Framework
{
    public static class UpdateManager
    {
        private static string _updatePackageUrl;

        public static string UpdateExePath;
        public static string AppFeedUrl;
        public static byte[] UpdateExe;

        public static void CleanUp()
        {
            if (String.IsNullOrEmpty(UpdateExePath))
            {
                //throw exception that the exe path has not been set
            }
            else
            {
                try
                {
                    //clean up updater after it has been extracted (if it has)
                    if (File.Exists(UpdateExePath))
                        File.Delete(UpdateExePath);
                }
                catch
                {
                }
            }
        }

        public static bool CheckForUpdate(out Update availableUpdate)
        {
            if (String.IsNullOrEmpty(AppFeedUrl))
            {
                //throw exception that the url has not been set
            }
            else
            {
                try
                {
                    var results = new AppcastReader().Read(AppFeedUrl);
                    Update update = results.First();


                    var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

                    if (UpdateChecker.CheckForUpdate(assemblyVersion,update.Version))
                    {
                        _updatePackageUrl = update.FileUrl;
                        availableUpdate = update;
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            availableUpdate = null;
            return false;
        }

        public static void ApplyUpdate()
        {
            if (String.IsNullOrEmpty(UpdateExePath) || UpdateExe == null)
            {
                //throw exception that the exe path has not been set
            }
            else
            {
                try
                {
                    new UpdateStarter(UpdateExePath, UpdateExe).Start(_updatePackageUrl);
                    Application.Current.Shutdown();
                }
                catch
                {
                    //error downloading or extracting update, notify user
                }
            }
        }
    }
}
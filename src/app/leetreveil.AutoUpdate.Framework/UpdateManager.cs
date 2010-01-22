using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

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
                catch { }
            }
        }

        public static void CheckForUpdate(Action<Update> actionToPerformIfUpdateIsAvailable)
        {
            if (String.IsNullOrEmpty(AppFeedUrl))
            {
                //throw exception that the url has not been set
            }
            else
            {
      
                    ThreadPool.QueueUserWorkItem(wcb =>
                         {
                             try
                             {
                                 var results = new AppcastReader().Read(AppFeedUrl);
                                 Update update = results.First();


                                 if (
                                     UpdateChecker.CheckForUpdate(
                                         Assembly.GetEntryAssembly().GetName().Version,
                                         update.Version))
                                 {
                                     _updatePackageUrl = update.FileUrl;
                                     actionToPerformIfUpdateIsAvailable(update);
                                 }
                             }
                             catch (Exception e)
                             {
                                 Console.WriteLine(e);
                             }
                         });

     
            }
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
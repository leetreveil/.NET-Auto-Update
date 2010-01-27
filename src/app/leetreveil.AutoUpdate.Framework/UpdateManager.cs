using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace leetreveil.AutoUpdate.Framework
{
    public sealed class UpdateManager
    {
        #region Singleton Stuff

        private static readonly UpdateManager instance = new UpdateManager();

        static UpdateManager()
        {
        }

        private UpdateManager()
        {
        }

        public static UpdateManager Instance
        {
            get { return instance; }
        }

        #endregion

        private string _updatePackageUrl;

        public byte[] UpdateExe { get; set; }
        public string AppFeedUrl { get; set; }
        public string UpdateExePath { get; set; }
        public Update NewUpdate { get; private set; }


        /// <summary>
        /// Removes the updater executable from the directory its in and fails silently
        /// </summary>
        public void CleanUp()
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

        /// <summary>
        /// Checks for the latest update and sets the NewUpdate property if one is available
        /// </summary>
        /// <returns></returns>
        public bool CheckForUpdate()
        {
            if (String.IsNullOrEmpty(AppFeedUrl))
            {
                //throw exception that the url has not been set
            }
            else
            {
                var results = new AppcastReader().Read(AppFeedUrl);
                Update update = results.First();

                var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

                if (UpdateChecker.CheckForUpdate(assemblyVersion, update.Version))
                {
                    _updatePackageUrl = update.FileUrl;
                    NewUpdate = update;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Downloads the update package and starts the updater executable
        /// </summary>
        public void ApplyUpdate()
        {
            if (String.IsNullOrEmpty(UpdateExePath) || UpdateExe == null)
            {
                //throw exception that the exe path has not been set
            }
            else
            {
                new UpdateStarter(UpdateExePath, UpdateExe).Start(_updatePackageUrl);
                Application.Current.Shutdown();
            }
        }
    }
}
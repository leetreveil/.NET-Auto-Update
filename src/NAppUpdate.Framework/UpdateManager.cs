using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using System.Threading;

using NAppUpdate.Framework.Utils;
using NAppUpdate.Framework.Conditions;
using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Framework
{
    public sealed class UpdateManager
    {
        #region Singleton Stuff

        private static readonly UpdateManager instance = new UpdateManager();

        private UpdateManager()
        {
            UpdatesToApply = new LinkedList<IUpdateTask>();
            TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            UpdateProcessName = "NAppUpdateProcess";
        }

        public static UpdateManager Instance
        {
            get { return instance; }
        }

        #endregion

        public string TempFolder { get; set; }
        public string UpdateProcessName { get; set; }

        internal LinkedList<IUpdateTask> UpdatesToApply { get; private set; }
        public int UpdatesAvailable { get { if (UpdatesToApply == null) return 0; return UpdatesToApply.Count; } }
        
        public IUpdateSource UpdateSource { get; set; }
        public IUpdateFeedReader UpdateFeedReader { get; set; }

        #region Step 1 - Check for updates

        public bool CheckForUpdates()
        {
            return CheckForUpdates(UpdateSource, null);
        }

        public bool CheckForUpdates(IUpdateSource source)
        {
            return CheckForUpdates(source, null);
        }

        public bool CheckForUpdates(IUpdateSource source, Action<int> callback)
        {
            if (UpdateFeedReader == null)
                throw new ArgumentException("An update feed reader is required; please set one before checking for updates");

            if (source == null)
                throw new ArgumentException("An update source was not specified");

            lock (UpdatesToApply)
            {
                UpdatesToApply.Clear();
                IList<IUpdateTask> tasks = UpdateFeedReader.Read(source.GetUpdatesFeed());
                foreach (IUpdateTask t in tasks)
                {
                    if (t.UpdateConditions.IsMet(t)) // Only execute if all conditions are met
                        UpdatesToApply.AddLast(t);
                }
            }

            if (callback != null) callback(UpdatesToApply.Count);

            if (UpdatesToApply.Count > 0)
                return true;

            return false;
        }

        public void CheckForUpdateAsync(Action<int> callback)
        {
            CheckForUpdateAsync(UpdateSource, callback);
        }

        public void CheckForUpdateAsync(IUpdateSource source, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem(cb => CheckForUpdates(source, callback));
        }

        #endregion

        #region Step 2 - Prepare to execute update tasks

        public bool PrepareUpdates()
        {
            return PrepareUpdates(null);
        }

        public bool PrepareUpdates(Action<bool> callback)
        {
            if (!Directory.Exists(TempFolder))
                Directory.CreateDirectory(TempFolder);

            lock (UpdatesToApply)
            {
                if (UpdatesToApply.Count == 0)
                {
                    if (callback != null) callback(false);
                    return false;
                }

                foreach (IUpdateTask t in UpdatesToApply)
                {
                    t.Prepare(UpdateSource);
                }
            }

            if (callback != null) callback(true);
            return true;
        }

        public void PrepareUpdatesAsync(Action<bool> callback)
        {
            ThreadPool.QueueUserWorkItem(cb => PrepareUpdates(callback));
        }

        #endregion

        /// <summary>
        /// Starts the updater executable and sends update data to it
        /// </summary>
        public bool ApplyUpdates()
        {
            Dictionary<string, object> executeOnAppRestart = new Dictionary<string, object>();
            foreach (IUpdateTask task in UpdatesToApply)
            {
                if (!task.Execute())
                {
                    // TODO: notify about task execution failure using exceptions
                }

                // This is the only place where we have non-generalized code in UpdateManager.
                // The reason for that is the updater currently only supports replacing bytes by path, which
                // only FileUpdaterTask does - so there's no reason to generalize this portion too.
                else if (task is FileUpdateTask && task.Attributes.ContainsKey("localPath"))
                {
                    if (task.Attributes.ContainsKey("apply") && "app-restart".Equals(task.Attributes["apply"]))
                    {
                        FileUpdateTask fut = (FileUpdateTask)task;
                        executeOnAppRestart[task.Attributes["localPath"]] = fut.fileBytes;
                    }
                }
            }

            // If an application restart is required
            if (executeOnAppRestart.Count > 0)
            {
                // Add some environment variables to the dictionary object which will be passed to the updater
                executeOnAppRestart["ENV:AppPath"] = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                executeOnAppRestart["ENV:TempFolder"] = TempFolder;

                UpdateStarter updStarter = new UpdateStarter(Path.Combine(TempFolder, "updater.exe"), executeOnAppRestart, UpdateProcessName);
                bool createdNew;
                using (Mutex mutex = new Mutex(true, UpdateProcessName, out createdNew))
                {
                    updStarter.Start();

                    Environment.Exit(0);
                }
            }

            return true;
        }

        /// <summary>
        /// Delete the temp folder as a whole and fail silently
        /// </summary>
        public void CleanUp()
        {
            try
            {
                Directory.Delete(TempFolder, true);
            }
            catch { }
        }

        /*
        public void DownloadUpdateAsync(Action<bool> finishedCallback)
        {
            FileDownloader fileDownloader = GetFileDownloader();

            fileDownloader.DownloadAsync(downloadedData =>
                                             {
                                                 //validate that the downloaded data is actually valid and not erroneous
                                                 this.UpdateData = downloadedData;
                                                 finishedCallback(true);
                                             });
        }

        public void DownloadUpdateAsync(Action<bool> finishedCallback, Action<int> progressPercentageCallback)
        {
            FileDownloader fileDownloader = GetFileDownloader();

            fileDownloader.DownloadAsync(downloadedData =>
            {
                //TODO: validate that the downloaded data is actually valid and not erroneous
                this.UpdateData = downloadedData;
                finishedCallback(true);
            },
            (arg1, arg2) => progressPercentageCallback((int)(100 * (arg1) / arg2)));
        }
         */
    }
}
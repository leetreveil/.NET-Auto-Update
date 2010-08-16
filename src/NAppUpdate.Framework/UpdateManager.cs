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
        
        internal string BaseUrl { get; set; }
        internal LinkedList<IUpdateTask> UpdatesToApply { get; private set; }
        public int UpdatesAvailable { get { if (UpdatesToApply == null) return 0; return UpdatesToApply.Count; } }
        
        public IUpdateSource UpdateSource { get; set; }
        public IUpdateFeedReader UpdateFeedReader { get; set; }

        private Thread _updatesThread;
        private volatile bool _shouldStop;
        public bool IsWorking { get { return _updatesThread != null && _updatesThread.IsAlive; } }

        #region Step 1 - Check for updates

        public bool CheckForUpdates()
        {
            return CheckForUpdates(UpdateSource, null);
        }

        public bool CheckForUpdates(IUpdateSource source)
        {
            return CheckForUpdates(source, null);
        }

        private bool CheckForUpdates(IUpdateSource source, Action<int> callback)
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
                    if (_shouldStop) return false;
                    if (t.UpdateConditions.IsMet(t)) // Only execute if all conditions are met
                        UpdatesToApply.AddLast(t);
                }
            }

            if (_shouldStop) return false;
            if (callback != null) callback.BeginInvoke(UpdatesToApply.Count, null, null);

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
            if (!IsWorking)
            {
                _updatesThread = new Thread(delegate()
                    {
                        try
                        {
                            CheckForUpdates(source, callback);
                        }
                        catch { callback.BeginInvoke(0, null, null); /* TODO: Handle error */ }
                    });
                _updatesThread.IsBackground = true;
                _updatesThread.Start();
            }
        }

        #endregion

        #region Step 2 - Prepare to execute update tasks

        public bool PrepareUpdates()
        {
            return PrepareUpdates(null);
        }

        private bool PrepareUpdates(Action<bool> callback)
        {
            // TODO: Support progress updates

            lock (UpdatesToApply)
            {
                if (UpdatesToApply.Count == 0)
                {
                    if (callback != null) callback.BeginInvoke(false, null, null);
                    return false;
                }

                if (!Directory.Exists(TempFolder))
                    Directory.CreateDirectory(TempFolder);

                foreach (IUpdateTask t in UpdatesToApply)
                {
                    if (_shouldStop) return false;
                    t.Prepare(UpdateSource);
                }
            }

            if (_shouldStop) return false;

            if (callback != null) callback.BeginInvoke(true, null, null);
            return true;
        }

        public void PrepareUpdatesAsync(Action<bool> callback)
        {
            if (!IsWorking)
            {
                _updatesThread = new Thread(delegate()
                {
                    try
                    {
                        PrepareUpdates(callback);
                    }
                    catch { callback.BeginInvoke(false, null, null); /* TODO: Handle error */ }
                });
                _updatesThread.IsBackground = true;
                _updatesThread.Start();
            }
        }

        #endregion

        #region Step 3 - Apply updates

        /// <summary>
        /// Starts the updater executable and sends update data to it, and relaunch the caller application as soon as its done
        /// </summary>
        /// <returns>True if successful (unless a restart was required</returns>
        public bool ApplyUpdates()
        {
            return ApplyUpdates(true);
        }

        /// <summary>
        /// Starts the updater executable and sends update data to it
        /// </summary>
        /// <param name="RestartApplication">true if relaunching the caller application is required; false otherwise</param>
        /// <returns>True if successful (unless a restart was required</returns>
        public bool ApplyUpdates(bool RelaunchApplication)
        {
            lock (UpdatesToApply)
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
                        if (!task.Attributes.ContainsKey("apply") ||
                            (task.Attributes.ContainsKey("apply") && "app-restart".Equals(task.Attributes["apply"])))
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
                    executeOnAppRestart["ENV:RelaunchApplication"] = RelaunchApplication;

                    UpdateStarter updStarter = new UpdateStarter(Path.Combine(TempFolder, "updater.exe"), executeOnAppRestart, UpdateProcessName);
                    bool createdNew;
                    using (Mutex mutex = new Mutex(true, UpdateProcessName, out createdNew))
                    {
                        updStarter.Start();

                        Environment.Exit(0);
                    }
                }

                UpdatesToApply.Clear();
            }

            return true;
        }

        #endregion

        public void Abort()
        {
            _shouldStop = true;
        }

        /// <summary>
        /// Delete the temp folder as a whole and fail silently
        /// </summary>
        public void CleanUp()
        {
            Abort();
            if (_updatesThread != null && _updatesThread.IsAlive)
                _updatesThread.Join();

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
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
            _updateConditions = new Dictionary<string, Type>();
            _updateTasks = new Dictionary<string, Type>();

            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (typeof(IUpdateTask).IsAssignableFrom(t))
                {
                    UpdateTaskAliasAttribute[] tasksAliases = (UpdateTaskAliasAttribute[])t.GetCustomAttributes(typeof(UpdateTaskAliasAttribute), false);
                    foreach (UpdateTaskAliasAttribute alias in tasksAliases)
                    {
                        _updateTasks.Add(alias.Alias, t);
                    }
                }
                else if (typeof(IUpdateCondition).IsAssignableFrom(t))
                {
                    UpdateConditionAliasAttribute[] tasksAliases = (UpdateConditionAliasAttribute[])t.GetCustomAttributes(typeof(UpdateConditionAliasAttribute), false);
                    foreach (UpdateConditionAliasAttribute alias in tasksAliases)
                    {
                        _updateConditions.Add(alias.Alias, t);
                    }
                }
            }

            UpdatesToApply = new LinkedList<IUpdateTask>();
            TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public static UpdateManager Instance
        {
            get { return instance; }
        }

        #endregion

        public byte[] UpdateExeBinary { get; set; }
        public string UpdateExePath { get; set; }
        public byte[] UpdateData { get; private set; }

        public string TempFolder { get; set; }

        internal Dictionary<string, Type> _updateConditions { get; private set; }
        internal Dictionary<string, Type> _updateTasks { get; private set; }
        
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
                IList<IUpdateTask> tasks = UpdateFeedReader.Read(this, source.GetUpdatesFeed());
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
        /// <returns>true if a restart is required (the update process will wait for the application to quit)</returns>
        public bool ApplyUpdates()
        {
            /*if (String.IsNullOrEmpty(UpdateExePath))
                throw new ArgumentException("The UpdateExePath has not been set");

            if (UpdateExeBinary == null || UpdateExeBinary.Length == 0)
                throw new ArgumentException("UpdateExeBinary has not been set");*/

            foreach (IUpdateTask task in UpdatesToApply)
            {
                if (!task.Execute())
                {
                    // TODO: notify about task execution failure
                }
            }

            new UpdateStarter(UpdateExePath, UpdateExeBinary, UpdateData).Start();

            //Application.Current.Shutdown();
            Environment.Exit(0);
            // TODO: Use mutex / named-pipes notifications

            return true;
        }

        /// <summary>
        /// Removes the updater executable from the directory its in and fails silently
        /// </summary>
        public void CleanUp()
        {
            if (String.IsNullOrEmpty(UpdateExePath))
                throw new ArgumentException("The UpdateExePath has not been set");

            try
            {
                //clean up updater after it has been extracted (if it has)
                if (File.Exists(UpdateExePath))
                    File.Delete(UpdateExePath);
            }
            catch{}

            try
            {
                Directory.Delete(TempFolder);
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
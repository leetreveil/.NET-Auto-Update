using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
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

        //static UpdateManager(){}

        private UpdateManager()
        {
            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (t is IUpdateTask && !t.IsInterface)
                {
                    UpdateTaskAliasAttribute[] tasksAliases = (UpdateTaskAliasAttribute[])t.GetCustomAttributes(typeof(UpdateTaskAliasAttribute), false);
                    foreach (UpdateTaskAliasAttribute alias in tasksAliases)
                    {
                        _updateTasks.Add(alias.Alias, t);
                    }
                }
                else if (t is IUpdateCondition && !t.IsInterface)
                {
                    UpdateConditionAliasAttribute[] tasksAliases = (UpdateConditionAliasAttribute[])t.GetCustomAttributes(typeof(UpdateTaskAliasAttribute), false);
                    foreach (UpdateConditionAliasAttribute alias in tasksAliases)
                    {
                        _updateConditions.Add(alias.Alias, t);
                    }
                }
            }
        }

        public static UpdateManager Instance
        {
            get { return instance; }
        }

        #endregion

        public byte[] UpdateExeBinary { get; set; }
        public string AppFeedUrl { get; set; }
        public string UpdateExePath { get; set; }
        public Update NewUpdate { get; private set; }
        public byte[] UpdateData { get; private set; }

        internal Dictionary<string, Type> _updateConditions { get; private set; }
        internal Dictionary<string, Type> _updateTasks { get; private set; }
        internal LinkedList<IUpdateTask> UpdatesToApply { get; private set; }
        
        public IUpdateSource UpdateSource { get; set; }
        public IUpdateFeedReader UpdateFeedReader { get; set; }

        public void CheckForUpdates()
        {
            CheckForUpdates(UpdateSource);
        }

        public bool CheckForUpdates(IUpdateSource source)
        {
            if (UpdateFeedReader == null)
                throw new ArgumentException("An update feed reader is required; please set one before checking for updates");

            if (source == null)
                throw new ArgumentException("An update source was not specified");

            UpdatesToApply.Clear();
            IEnumerable<IUpdateTask> tasks = UpdateFeedReader.Read(this, source.GetUpdatesFeed());
            foreach (IUpdateTask t in tasks)
            {
                if (t.UpdateConditions.IsFulfilled())
                    UpdatesToApply.AddLast(t);
            }
            
            if (UpdatesToApply.Count > 0)
                return true;

            return false;
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
        }

        /// <summary>
        /// Checks for the latest update and sets the NewUpdate property if one is available
        /// </summary>
        /// <returns></returns>
        public bool CheckForUpdate()
        {
            if (String.IsNullOrEmpty(AppFeedUrl))
                throw new ArgumentException("The AppFeedUrl has not been set");

            IEnumerable<Update> results = new AppcastReader().Read(AppFeedUrl);

            return GetLatestUpdateFromUpdates(results);
        }

        private bool GetLatestUpdateFromUpdates(IEnumerable<Update> results)
        {
            if (results.Count() <= 0) return false;

            Update update = results.First();

            var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

            if (update.Version > assemblyVersion)
            {
                NewUpdate = update;
                return true;
            }

            return false;
        }

        public bool CheckForUpdate(IUpdateFeedSource updateReader)
        {
            if (String.IsNullOrEmpty(AppFeedUrl))
                throw new ArgumentException("The AppFeedUrl has not been set");

            IEnumerable<Update> results = updateReader.Read(this.AppFeedUrl);

            return GetLatestUpdateFromUpdates(results);
        }

        public void CheckForUpdateAsync(Action<bool> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => CheckForUpdate());
        }

        public bool DownloadUpdate()
        {
            FileDownloader fileDownloader = GetFileDownloader();

            byte[] update = fileDownloader.Download();

            if (update == null)
                return false;

            if (update.Length == 0)
                return false;

            this.UpdateData = update;

            return true;
        }

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

        private FileDownloader GetFileDownloader()
        {
            if (NewUpdate == null)
                throw new ArgumentException("NewUpdate has not been set");

            if (String.IsNullOrEmpty(NewUpdate.FileUrl))
                throw new ArgumentException("NewUpdate.FileUrl is not valid");

            return new FileDownloader(this.NewUpdate.FileUrl);
        }

        /// <summary>
        /// Starts the updater executable and sends update data to it
        /// </summary>
        public void ApplyUpdate()
        {
            if (String.IsNullOrEmpty(UpdateExePath))
                throw new ArgumentException("The UpdateExePath has not been set");

            if (UpdateExeBinary == null || UpdateExeBinary.Length == 0)
                throw new ArgumentException("UpdateExeBinary has not been set");

            new UpdateStarter(UpdateExePath, UpdateExeBinary, UpdateData).Start();

            Application.Current.Shutdown();
        }
    }
}
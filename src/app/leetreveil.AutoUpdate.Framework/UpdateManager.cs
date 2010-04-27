using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Threading;

namespace leetreveil.AutoUpdate.Framework
{
    public sealed class UpdateManager
    {
        #region Singleton Stuff

        private static readonly UpdateManager instance = new UpdateManager();

        static UpdateManager(){}

        private UpdateManager() {}

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

            var results = new AppcastReader().Read(AppFeedUrl);

            if (results.Count() <= 0) return false;

            Update update = results.First();

            var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

            if (UpdateChecker.CheckForUpdate(assemblyVersion, update.Version))
            {
                NewUpdate = update;
                return true;
            }

            return false;
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
            (arg1, arg2) => progressPercentageCallback((int) (100 * (arg1) / arg2)));
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
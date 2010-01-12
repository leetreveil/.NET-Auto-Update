using System;
using System.Diagnostics;
using System.Windows;
using leetreveil.AutoUpdate.Core.FileDownload;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using Path=System.IO.Path;
using System.IO;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Update _newUpdate;

        public UpdateWindow(Update newUpdate)
        {
            _newUpdate = newUpdate;
            InitializeComponent();

            this.DataContext = this;
        }

        public string UpdateVersion
        {
            get { return _newUpdate.Version.ToString(); }
        }

        private void InstallNow_Click(object sender, RoutedEventArgs e)
        {
            var fDownloader = new FileDownloader(_newUpdate.FileUrl);

            var filePathToUpdate = fDownloader.Download();

            if (!String.IsNullOrEmpty(filePathToUpdate))
            {
                ExtractExecutableFromResource();
                StartUpdaterExeAndShutdown(filePathToUpdate);
            }
            else
            {
                //TODO: error downloading the update, display message to user
            }

        }

        private void ExtractExecutableFromResource()
        {
            using (var writer = new BinaryWriter(File.Open("updater.exe", FileMode.Create)))
                writer.Write(Properties.Resources.updater);
        }

        /// <summary>
        /// Starts the update executable, shuts down the current application so the update can start
        /// </summary>
        /// <param name="compressedUpdateFileName">The name of the compressed file that should be extracted and used to update existing files</param>
        private void StartUpdaterExeAndShutdown(string compressedUpdateFileName)
        {
            var thisAppsFileName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process.Start("updater.exe", thisAppsFileName + " " + Path.Combine(Environment.CurrentDirectory, compressedUpdateFileName));

            Application.Current.Shutdown();
        }
    }
}
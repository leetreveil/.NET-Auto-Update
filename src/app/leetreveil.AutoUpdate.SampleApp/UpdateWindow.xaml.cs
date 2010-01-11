using System.Diagnostics;
using System.Windows;
using leetreveil.AutoUpdate.Core.FileDownload;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using Path=System.IO.Path;

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
        }

        private void InstallNow_Click(object sender, RoutedEventArgs e)
        {
            var fDownloader = new FileDownloader(_newUpdate.FileUrl);

            var filePathToUpdate = fDownloader.Download();

            StartUpdaterExeAndShutdown(filePathToUpdate);
        }

        /// <summary>
        /// Starts the update executable, shuts down the current application so the update can start
        /// </summary>
        /// <param name="filePathToUpdate">The path to the compressed file that should be extracted and used to update existing files</param>
        private void StartUpdaterExeAndShutdown(string filePathToUpdate)
        {
            var thisAppsFileName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process.Start("updater.exe", thisAppsFileName + " " + filePathToUpdate);

            Application.Current.Shutdown();
        }
    }
}

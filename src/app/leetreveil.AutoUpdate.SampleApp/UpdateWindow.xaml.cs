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

            var thisAppsFileName2 = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process.Start("updater.exe", thisAppsFileName2 + " " + filePathToUpdate);


            Application.Current.Shutdown();
        }
    }
}

using System;
using System.Reflection;
using System.Windows;
using NAppUpdate.Framework;
using Path=System.IO.Path;
using System.IO;

namespace NAppUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AppVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        private readonly string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                  "ltupdater.exe");

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateManager updManager = UpdateManager.Instance;

            //update configuration
            updManager.UpdateFeedReader = new NAppUpdate.Framework.FeedReaders.AppcastReader();
            updManager.UpdateSource = new NAppUpdate.Framework.Sources.SimpleWebSource();

            updManager.UpdateExePath = updaterPath;
            updManager.UpdateExeBinary = Properties.Resources.ltupdater;

            //always clean up at the beginning of the exe because we cant do it at the end
            updManager.CleanUp();

            if (updManager.CheckForUpdates(new NAppUpdate.Framework.Sources.MemorySource(File.ReadAllText("sampleappupdatefeed.xml"))))
                    new UpdateWindow(updManager).Show();
        }
    }
}

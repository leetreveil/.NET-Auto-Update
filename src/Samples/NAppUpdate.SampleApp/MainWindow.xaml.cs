using System;
using System.Reflection;
using System.Windows;
using NAppUpdate.Framework;
using Path=System.IO.Path;

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
            updManager.UpdateExePath = updaterPath;
            updManager.AppFeedUrl = "sampleappupdatefeed.xml";
            updManager.UpdateExeBinary = Properties.Resources.ltupdater;

            //always clean up at the beginning of the exe because we cant do it at the end
            updManager.CleanUp();

            if (updManager.CheckForUpdate())
                    new UpdateWindow(updManager).Show();
        }
    }
}

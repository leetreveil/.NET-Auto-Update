using System;
using System.Reflection;
using System.Windows;
using leetreveil.AutoUpdate.Framework;
using Path=System.IO.Path;

namespace leetreveil.AutoUpdate.SampleApp
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
            var updManager = UpdateManager.Instance;

            //update configuration
            updManager.UpdateExePath = updaterPath;
            updManager.AppFeedUrl = "sampleappupdatefeed.xml";
            updManager.UpdateExe = Properties.Resources.ltupdater;
            //always clean up at the beginning of the exe because we cant do it at the end
            updManager.CleanUp();

            if (updManager.CheckForUpdate())
                if (updManager.NewUpdate != null)
                    new UpdateWindow(updManager.NewUpdate).Show();
        }
    }
}

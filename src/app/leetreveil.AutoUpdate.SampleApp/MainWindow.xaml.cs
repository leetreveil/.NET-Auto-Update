using System;
using System.IO;
using System.Linq;
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
            UpdateManager.UpdateExePath = updaterPath;
            UpdateManager.AppFeedUrl = "sampleappupdatefeed.xml";
            UpdateManager.UpdateExe = Properties.Resources.ltupdater;

            UpdateManager.CleanUp();
            UpdateManager.CheckForUpdate(update => new UpdateWindow(update).Show());
        }
    }
}

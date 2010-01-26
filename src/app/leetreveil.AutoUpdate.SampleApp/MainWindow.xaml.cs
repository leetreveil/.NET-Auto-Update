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
            //update configuration
            UpdateManager.UpdateExePath = updaterPath;
            UpdateManager.AppFeedUrl = "sampleappupdatefeedx.xml";
            UpdateManager.UpdateExe = Properties.Resources.ltupdater;


            //always clean up at the beginning of the exe because we cant do it at the end
            UpdateManager.CleanUp();


            Update newUpd;
            if (UpdateManager.CheckForUpdate(out newUpd))
                if (newUpd != null)
                    new UpdateWindow(newUpd).Show();
        }
    }
}

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
            try
            {
                //clean up updater after its been extracted
                if (File.Exists(updaterPath))
                    File.Delete(updaterPath);
            }
            catch{ }

            try
            {
                ////TODO: check for update asyncronously
                ////TODO: fix it so we dont have to download file updates from the internet and just point to a file on disk in the xml file (easier to test)
                var results = new AppcastReader().Read("sampleappupdatefeed.xml");
                Update update = results.First();


                if (UpdateChecker.CheckForUpdate(Assembly.GetEntryAssembly().GetName().Version,update.Version))
                {
                    //ask user if he wants to update or not
                    new UpdateWindow(update).Show();
                }
            }
            catch  {}
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using Path=System.IO.Path;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public string AppVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        private string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                  "ltupdater.exe");

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
                //TODO: check for update asyncronously
                var updateChecker = new UpdateChecker();

                //TODO: fix it so we dont have to download file updates from the internet and just point to a file on disk in the xml file (easier to test)
                if (updateChecker.CheckForUpdate("sampleappupdatefeed.xml"))
                {
                    //ask user if he wants to update or not
                    new UpdateWindow(updateChecker.Update).Show();
                }
            }
            catch  {}
        }
    }
}

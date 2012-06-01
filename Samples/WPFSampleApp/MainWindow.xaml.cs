using System;
using System.IO;
using System.Reflection;
using System.Windows;
using NAppUpdate.Framework;

namespace NAppUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AppVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //CheckForUpdates();
        }

        private void btnCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            //UpdateManager updManager = UpdateManager.Instance;
            //updManager.Abort();
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            UpdateManager updManager = UpdateManager.Instance;

            //update configuration
            updManager.UpdateFeedReader = new NAppUpdate.Framework.FeedReaders.AppcastReader();
            //updManager.UpdateSource = new NAppUpdate.Framework.Sources.SimpleWebSource();
            updManager.UpdateSource = new NAppUpdate.Framework.Sources.MemorySource(File.ReadAllText("SampleAppUpdateFeed.xml"));

            updManager.CheckForUpdateAsync(
                updatesCount =>
                {
                    try
                    {
                        Action showUpdateAction = () => new UpdateWindow(UpdateManager.Instance).Show();

                        if (updatesCount > 0)
                        {
                            if (Dispatcher.CheckAccess())
                                showUpdateAction();
                            else
                                Dispatcher.Invoke(showUpdateAction);
                        }
                        else if (updatesCount == 0)
                        {
                            MessageBox.Show("You already have the latest software.\n\n" +
                                "   Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
                        }
                    }
                    catch (Exception ex) 
                    { 
                        MessageBox.Show("Could not start UpdateWindow: \n" + ex.Message); 
                    }
                });
        }
    }
}

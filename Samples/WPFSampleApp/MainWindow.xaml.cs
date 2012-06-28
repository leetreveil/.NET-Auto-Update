using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
    	public string AppVersion
    	{
    		get
    		{
    			if (File.Exists("CurrentVersion.txt"))
    				return File.ReadAllText("CurrentVersion.txt");
    			return "1.0";
    		}
    	}

    	private bool applyUpdates;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private NAppUpdate.Framework.Sources.IUpdateSource PrepareUpdateSource()
        {
            // Normally this would be a web based source.
            // But for the demo app, we prepare an in-memory source.
            var source = new NAppUpdate.Framework.Sources.MemorySource(File.ReadAllText("SampleAppUpdateFeed.xml"));
            source.AddTempFile(new Uri("http://SomeSite.com/Files/NewVersion.txt"), "NewVersion.txt");

            return source;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        	var updManager = UpdateManager.Instance;
			updManager.UpdateSource = PrepareUpdateSource();
			updManager.ReinstateIfRestarted();
        }

        private void btnCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            UpdateManager updManager = UpdateManager.Instance;

			updManager.CheckForUpdateAsync(hasUpdates =>
			{
				Action showUpdateAction = ShowUpdateWindow;

				if (!hasUpdates)
				{
					// No updates were found, or an error has occured. We might want to check that...
					if (updManager.LatestError == Errors.NoUpdatesFound)
					{
						MessageBox.Show("All is up to date!");
						return;
					}

					MessageBox.Show(updManager.LatestError);
				}

				applyUpdates = true;

				if (Dispatcher.CheckAccess())
					showUpdateAction();
				else
					Dispatcher.Invoke(showUpdateAction);
			});
		}

        private void ShowUpdateWindow()
        {
            var updateWindow = new UpdateWindow();

            updateWindow.Closed += (sender, e) =>
            {
                if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.AppliedSuccessfully)
                {
                    applyUpdates = false;

                    // Update the app version
                    OnPropertyChanged("AppVersion");
                }
            };

            updateWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Do any updates.
            if (applyUpdates)
            {
                if (UpdateManager.Instance.PrepareUpdates())
                    UpdateManager.Instance.ApplyUpdates(false);
                else
                    UpdateManager.Instance.CleanUp();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

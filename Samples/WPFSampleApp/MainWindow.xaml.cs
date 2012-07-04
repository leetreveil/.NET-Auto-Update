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

        	updManager.BeginCheckForUpdates(asyncResult =>
        	                                	{
        	                                		Action showUpdateAction = ShowUpdateWindow;

        	                                		if (asyncResult.IsCompleted)
        	                                		{
        	                                			// still need to check for caught exceptions if any and rethrow
        	                                			((UpdateProcessAsyncResult) asyncResult).EndInvoke();

        	                                			// No updates were found, or an error has occured. We might want to check that...
        	                                			if (updManager.UpdatesAvailable == 0)
        	                                			{
        	                                				MessageBox.Show("All is up to date!");
        	                                				return;
        	                                			}
        	                                		}

        	                                		applyUpdates = true;

        	                                		if (Dispatcher.CheckAccess())
        	                                			showUpdateAction();
        	                                		else
        	                                			Dispatcher.Invoke(showUpdateAction);
        	                                	}, null);
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
				try
				{
					UpdateManager.Instance.PrepareUpdates();
				}
				catch
				{
					UpdateManager.Instance.CleanUp();
					return;
				}
            	UpdateManager.Instance.ApplyUpdates(false);
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

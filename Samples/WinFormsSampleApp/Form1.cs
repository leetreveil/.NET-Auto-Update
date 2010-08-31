using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NAppUpdate.Framework;
using NAppUpdate.Framework.Sources;

namespace WinFormsSampleApp
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Replacement for VB InputBox, returns user input string.
        ///
        ///
        ///
        ///
        /// <returns>string
        public static string InputBox(string prompt,
          string title, string defaultValue)
        {
            InputBoxDialog ib = new InputBoxDialog();
            ib.FormPrompt = prompt;
            ib.FormCaption = title;
            ib.DefaultValue = defaultValue;
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        } // method: InputBox

        private Timer _resetCheckedState;

        public Form1()
        {
            InitializeComponent();

            // UpdateManager initialization
            UpdateManager updManager = UpdateManager.Instance;
            updManager.UpdateFeedReader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
            updManager.UpdateSource = new NAppUpdate.Framework.Sources.SimpleWebSource(/* update feed URL */);
            updManager.TempFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NAppUpdateWinFormsSample\\Updates");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Create a timer for reseting the update state
            _resetCheckedState = new Timer();
            _resetCheckedState.Interval = 60000;
            _resetCheckedState.Tick += new EventHandler((object sender, EventArgs ea) =>
            {
                if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.Checked)
                {
                    UpdateManager.Instance.CleanUp();
                    lblStatus.Text = DateTime.Now.ToString() + " - Update state was reset to NotChecked";
                }
            });
            _resetCheckedState.Start();
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            // For the purpose of this demonstration, we are loading the update feed from a local file and passing
            // it to UpdateManager using MemorySource.
            // Without passing this IUpdateSource object to CheckForUpdates, it will attempt to retrieve an
            // update feed from the feed URL specified in SimpleWebSource (which we did not provide)
            string feedXml = System.IO.File.ReadAllText("SampleUpdateFeed.xml");
            IUpdateSource feedSource = new MemorySource(feedXml);
            CheckForUpdates(feedSource);
        }

        private void btnCheckForUpdatesCustom_Click(object sender, EventArgs e)
        {
            string feedUrl = InputBox("Please enter a Url for a valid NauXml feed", "Feed Url required", "");

            if (string.IsNullOrEmpty(feedUrl))
                return;

            IUpdateSource source = UpdateManager.Instance.UpdateSource;
            if (source is SimpleWebSource)
            {
                // All we need to do is set the feed url and we are all set, no need to create new objects etc
                ((SimpleWebSource)source).FeedUrl = feedUrl;
                CheckForUpdates(null);
            }
            else
            {
                // No idea what we had there, so we create a new feed source and pass it along - note the
                // source for retreiving the actual updates will keep intact, and will be used when preparing
                // the updates
                source = new SimpleWebSource(feedUrl);
                CheckForUpdates(source);
            }
        }

        private void CheckForUpdates(IUpdateSource source)
        {
            // Get a local pointer to the UpdateManager instance
            UpdateManager updManager = UpdateManager.Instance;

            // Only check for updates if we haven't done so already
            if (updManager.State != UpdateManager.UpdateProcessState.NotChecked)
            {
                MessageBox.Show("Update process has already initialized; current state: " + updManager.State.ToString());
                return;
            }

            try
            {
                // Check for updates - returns true if relevant updates are found (after processing all the tasks and
                // conditions)
                // Throws exceptions in case of bad arguments or unexpected results
                if (updManager.CheckForUpdates(source))
                {
                    DialogResult dr = MessageBox.Show(
                        string.Format("Updates are available to your software ({0} total). Do you want to download and prepare them now? You can always do this at a later time.",
                        updManager.UpdatesAvailable),
                        "Software updates available",
                         MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        updManager.PrepareUpdatesAsync(OnPrepareUpdatesCompleted);
                    }
                }
                else
                {
                    MessageBox.Show("Your software is up to date");
                }
            }
            catch (Exception ex)
            {
                if (ex is NAppUpdateException)
                { 
                    // This indicates a feed or network error; ex will contain all the info necessary
                    // to deal with that
                }
                else MessageBox.Show(ex.ToString());
            }
        }

        private void btnPrepareUpdates_Click(object sender, EventArgs e)
        {
            UpdateManager updManager = UpdateManager.Instance;

            if (updManager.State != UpdateManager.UpdateProcessState.Checked)
            {
                MessageBox.Show("Cannot prepare updates at the current state: " + updManager.State.ToString());
                return;
            }

            if (updManager.UpdatesAvailable == 0)
            {
                MessageBox.Show("There are no updates to prepare");
                return;
            }

            updManager.PrepareUpdatesAsync(OnPrepareUpdatesCompleted);
        }

        private void btnInstallUpdates_Click(object sender, EventArgs e)
        {
            UpdateManager updManager = UpdateManager.Instance;

            if (updManager.State != UpdateManager.UpdateProcessState.Prepared)
            {
                MessageBox.Show("Cannot install updates at the current state, they need to be prepared first. Current state is " + updManager.State.ToString());
                return;
            }

            if (updManager.ApplyUpdates())
                MessageBox.Show("Error while trying to install software updates");
        }

        private void OnPrepareUpdatesCompleted(bool succeeded)
        {
            if (!succeeded)
            {
                MessageBox.Show("Updates preperation failed. Check the feed and try again.");
            }
            else
            {
                // Get a local pointer to the UpdateManager instance
                UpdateManager updManager = UpdateManager.Instance;

                DialogResult dr = MessageBox.Show(
                        "Updates are ready to install. Do you wish to install them now?",
                        "Software updates ready",
                         MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                {
                    // This is a synchronous method by design, make sure to save all user work before calling
                    // it as it might restart your application
                    if (!updManager.ApplyUpdates())
                        MessageBox.Show("Error while trying to install software updates");
                }
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            /*
             * Here's how to trigger the application update on quit, after it has 
             * been prepared by a worker thread before the user quit:

            // Check to see if there is a pending update
            UpdateManager updManager = UpdateManager.Instance;
            if (updManager.State == UpdateManager.UpdateProcessState.Prepared)
            {
                // Just in case there is a working thread...
                UpdateManager.Instance.Abort();
                while (UpdateManager.Instance.IsWorking) ;

                // Execute updates, asking the updater not to relaunch the app
                UpdateManager.Instance.ApplyUpdates(false);
            }

            // Do some cleanup work if necessary
            UpdateManager.Instance.CleanUp();
            */
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            if (UpdateManager.Instance.State != UpdateManager.UpdateProcessState.RollbackRequired)
            {
                MessageBox.Show("There is no failed update process to rollback; current state: " + UpdateManager.Instance.State.ToString());
                return;
            }

            UpdateManager.Instance.RollbackUpdates();
        }
    }
}

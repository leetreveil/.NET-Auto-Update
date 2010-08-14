using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NAppUpdate.Framework;
using NAppUpdate.Framework.Sources;

namespace WinFormsSampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // UpdateManager initialization
            UpdateManager updManager = UpdateManager.Instance;
            updManager.UpdateFeedReader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
            updManager.UpdateSource = new NAppUpdate.Framework.Sources.SimpleWebSource(/* update feed URL */);
            updManager.TempFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NAppUpdateWinFormsSample\\Updates");
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            // Get a local pointer to the UpdateManager instance
            UpdateManager updManager = UpdateManager.Instance;

            // For the purpose of this demonstration, we are loading the update feed from a local file and pass
            // it using MemorySource.
            // Without passing this IUpdateSource object to CheckForUpdates, it will attempt to retrieve an
            // update feed from the feed URL specified in SimpleWebSource (which we did not provide)
            string feedXml = System.IO.File.ReadAllText("SampleUpdateFeed.xml");
            IUpdateSource feedSource = new MemorySource(feedXml);

            // Check for updates - returns true if relevant updates are found (after processing all the tasks and
            // conditions)
            if (updManager.CheckForUpdates(feedSource))
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

        private void btnPrepareUpdates_Click(object sender, EventArgs e)
        {
            UpdateManager.Instance.PrepareUpdatesAsync(OnPrepareUpdatesCompleted);
        }

        private void btnInstallUpdates_Click(object sender, EventArgs e)
        {

        }

        private void OnPrepareUpdatesCompleted(bool succeeded)
        {
            if (!succeeded)
            {
                MessageBox.Show("Preparing the updates failed. Check the feed and try again.");
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
                }
            }
        }
    }
}

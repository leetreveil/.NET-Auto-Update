using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using leetreveil.AutoUpdate.Core.FileDownload;
using System.Diagnostics;
using System.Reflection;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //check for update asyncronously

            var updateChecker = new UpdateChecker();

            if (updateChecker.CheckForUpdate("zunesocialtagger.xml"))
            {
                //ask user if he wants to update or not
                new UpdateWindow(updateChecker.Update).Show();
            }
        }
    }
}
